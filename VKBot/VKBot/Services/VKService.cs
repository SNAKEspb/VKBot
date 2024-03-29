﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Reflection;
using VKBot.VkBotLogic.Models;

namespace VKBot.Services
{
    public class VKService
    {
        static string _url { get; set; } = "https://api.vk.com/";
        static HttpClient _httpClient = new HttpClient();
        private NLog.Logger _logger;

        public VKService(NLog.Logger logger)
        {
            _logger = logger;
        }

        public async Task<RegisterResponse> groupsGetLongPollServerAsync(string groupId, string token, string apiVersion)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "method/groups.getLongPollServer",
                Query = $"group_id={groupId}&access_token={token}&v={apiVersion}"
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);
            var response = await _httpClient.GetStringAsync(urlBuilder.Uri);
            _logger.Log(NLog.LogLevel.Info, response);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterResponse>(response);
        }

        public async Task<UpdateResponse> checkLongPollUpdatesAsync(string server, string ts, string key, string wait, string apiVersion)
        {
            var url = $"{server}?act=a_check&ts={ts}&key={key}&wait={wait}&version={apiVersion}";
            _logger.Log(NLog.LogLevel.Info, url);
            var response = await _httpClient.GetStringAsync(url);
            _logger.Log(NLog.LogLevel.Info, response);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateResponse>(response);
        }

        public async Task<string> sendRequest(IOutgoingMessage message, string method, VKBotOptions options)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = $"method/{method}",
                Query = $"group_id={options.groupId}&access_token={options.token}&v={options.apiVersion}"
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);
            var values = message.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(t => new { key = t.Name, value = t.GetValue(message, null) })
                .Where(t => t.value != null)
                .ToDictionary(t => t.key, t => t.value.ToString());
            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync(urlBuilder.Uri, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.Log(NLog.LogLevel.Info, responseBody);
            return responseBody;
        }


        public async Task<bool> messagesSendAsync(OutgoingMessage message, string groupId, string token, string apiVersion)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "method/messages.send",
                Query = $"group_id={groupId}&access_token={token}&v={apiVersion}"
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);
            var values = new Dictionary<string, string>
                {
                    { "random_id", message.random_id},
                    { "peer_id", message.peer_id.ToString()},
                    { "message", message.message},
                    { "attachment", message.attachment }
                };

            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync(urlBuilder.Uri, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.Log(NLog.LogLevel.Info, responseBody);
            return true;
        }

        public async Task<RegisterResponse> photosGetMessagesUploadServerAsync(string peer_id, string token, string apiVersion)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "method/photos.getMessagesUploadServer",
                Query = $"peer_id={peer_id}&access_token={token}&v={apiVersion}"
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);

            var responseBody = await _httpClient.GetStringAsync(urlBuilder.Uri);
            _logger.Log(NLog.LogLevel.Info, responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterResponse>(responseBody);
        }
        
        public async Task<PhotoUploadResponse> uploadPictureAsync(string upload_url, string pictureName, byte[] pictureBytes)
        {
            //would work with Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //check Program.cs

            _logger.Log(NLog.LogLevel.Info, upload_url);
            MultipartFormDataContent form = new MultipartFormDataContent();
            using (var imageContent = new ByteArrayContent(pictureBytes))
            {
                imageContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(imageContent, "file", pictureName);
                HttpResponseMessage response = await _httpClient.PostAsync(upload_url, form);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.Log(NLog.LogLevel.Info, responseBody);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoUploadResponse>(responseBody);
                return result;
            }
        }

        public async Task<PhotoUploadResponse> uploadPictureByUrlAsync(string upload_url, string peer_id, string photoUrl)
        {
            var uri = new Uri(photoUrl);
            _logger.Log(NLog.LogLevel.Info, uri);
            using (WebClient client = new WebClient())
            {
                byte[] photoBytes = await client.DownloadDataTaskAsync(uri);

                return await uploadPictureAsync(upload_url, System.IO.Path.GetFileName(uri.LocalPath), photoBytes);
            }
        }

        public async Task<PhotoSaveResponse> photosSaveMessagesPhotoAsync(PhotoUploadResponse photoParams, string token, string apiVersion)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "method/photos.saveMessagesPhoto",
                Query = $"access_token={token}&v={apiVersion}"
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);

            var values = new Dictionary<string, string>
                {
                    { "photo", photoParams.photo},
                    { "server", photoParams.server},
                    { "hash", photoParams.hash }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(urlBuilder.Uri, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.Log(NLog.LogLevel.Info, responseBody);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoSaveResponse>(responseBody);
        }

        public async Task<string> savePhotoByUrl(string url, string peer_id, string token, string apiVersion)
        {
            var uploadRegisterResponse = await photosGetMessagesUploadServerAsync(peer_id, token, apiVersion);
            //mb upload_url should be stored
            var photoUploadResponse = await uploadPictureByUrlAsync(uploadRegisterResponse.response.upload_url, peer_id, url);
            var photoSaveResponse = await photosSaveMessagesPhotoAsync(photoUploadResponse, token, apiVersion);
            var photoSaveData = photoSaveResponse.response.FirstOrDefault();
            return $"photo{photoSaveData.owner_id}_{photoSaveData.id}_{photoSaveData.access_key}";

        }
    }
}
