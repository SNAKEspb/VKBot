using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VKBot.VkontakteBot.Models;

namespace VKBot
{
    public class VkBot : IVityaBot
    {
        static HttpClient _httpClient = new HttpClient();
        static Random _random = new Random();

        static string _url { get; set; } = "https://api.vk.com/";
       
        static string _token { get; set; } = "5d711c0cd2689e386490cae8d08d38e8186480330cd1f7a5de4e8fdfb2e02ec4da3881ac991bb0d2fb63e";
        static string _groupId { get; set; } = "180024929";
        static string _apiVersion { get; set; } = "5.92";
        static string _wait { get; set; } = "25";
        static string _confirmationCode { get; set; } = "821df2ec";

        static string _imgFlipUrl { get; set; } = "https://api.imgflip.com/caption_image";
        static string _imgFlipUsername = "VityaBot";
        static string _imgFlipPassword = "vityaBot1!";

        static List<string> _memeIds = new List<string>{
            "140165357",
            "131429347",
            "136969882",
            "149017264",
            "156217874",
            "164453195",
            "155944363",
            "159322555",
            "154375976",
            "162536150",
            "167838922",
            "143051342",
            "150065404",
            "156519791",
            "175797735",//new
            "175803022",
            "175803056",
            "175803091",
            "175803124",
            "175803143",
            "175803170"
        };

        static string _key { get; set; }
        static string _ts { get; set; }
        static string _server { get; set; }

        //static string _key { get; set; }
        //static string _ts { get; set; }
        //static string _server { get; set; }


        private NLog.Logger _logger;

        private static VkBot _instanse;
        public static VkBot getinstanse(NLog.Logger logger)
        {
            if (_instanse == null)
            {
                _instanse = new VkBot(logger);
            }
            return _instanse;
        }

        public VkBot(NLog.Logger logger)
        {
            _logger = logger;
        }

        public string confimationCode => _confirmationCode;
        public async Task<IRegisterResponse> AuthorizeAsync()
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, "registerStart");
                var urlBuilder = new UriBuilder(_url)
                {
                    Path = "method/groups.getLongPollServer",
                    Query = $"group_id={_groupId}&access_token={_token}&v={_apiVersion}"
                };
                var response = await _httpClient.GetStringAsync(urlBuilder.Uri);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterResponse>(response);

                _key = result.response.key;
                _ts = result.response.ts;
                _server = result.response.server;

                result.Success = !string.IsNullOrEmpty(_key);
                _logger.Log(NLog.LogLevel.Info, "registerEnd");

                return result;
            }
            catch (Exception e)
            {
                _logger.Log(NLog.LogLevel.Error, e, "registerErr");
                return new RegisterResponse() { Success = false };
            }
        }

        public async Task<IUpdatesResponse> GetUpdatesAsync()
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, " GetUpdatesStart");

                int ts = 1;
                int.TryParse(_ts, out ts);
                //ts--;

                var url = $"{_server}?act=a_check&ts={ts}&key={_key}&wait={_wait}&version={_apiVersion}";

                var response = await _httpClient.GetStringAsync(url);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateResponse>(response);

                if (result.failed != 0)
                {
                    _logger.Log(NLog.LogLevel.Error, " Error received");
                    switch (result.failed)
                    {
                        case 1:
                            _ts = result.new_ts ?? result.ts;
                            _logger.Log(NLog.LogLevel.Info, " Ts updated");
                            break;
                        case 2:
                        case 3:
                            throw new Exception("Session expired. Reconnect to service.");
                        default:
                            throw new Exception("Unknown error");
                    }
                }
                _ts = result.new_ts ?? result.ts;

                _logger.Log(NLog.LogLevel.Info, " GetUpdatesEnd");

                return result;
            }
            catch (Exception e)
            {
                _logger.Log(NLog.LogLevel.Error, e, "GetUpdatesErr");
                return null;
            }


            throw new NotImplementedException();
        }


        public async Task<bool> SendMessageAsync(IOutgoingMessage message)
        {
            try
            {
                var tmessage = (OutgoingMessage)message;

                var urlBuilder = new UriBuilder(_url)
                {
                    Path = "method/messages.send",
                    Query = $"group_id={_groupId}&access_token={_token}&v={_apiVersion}"
                };

                var values = new Dictionary<string, string>
            {
                { "random_id", tmessage.random_id},
                { "peer_id", tmessage.peer_id.ToString()},
                { "message", tmessage.message},
                { "attachment", tmessage.attachment }
            };

                var content = new FormUrlEncodedContent(values);
                var response = await _httpClient.PostAsync(urlBuilder.Uri, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.Log(NLog.LogLevel.Error, e, "send error");
                return false;
            }

        }

        public async Task<string> getMessagesUploadServer(string peer_id)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "method/photos.getMessagesUploadServer",
                Query = $"peer_id={peer_id}&access_token={_token}&v={_apiVersion}"
            };
            try
            {
                _logger.Log(NLog.LogLevel.Info, $"getMessagesUploadServer {urlBuilder}");

                var responseBody = await _httpClient.GetStringAsync(urlBuilder.Uri);

                _logger.Log(NLog.LogLevel.Info, responseBody);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterResponse>(responseBody);
                return result.response.upload_url;
            }
            catch (Exception e)
            {
                _logger.Log(NLog.LogLevel.Error, e, "getMessagesUploadServer Error");
                return null;
            }
        }

        public async Task<PhotoUploadResponse> uploadPhoto(string upload_url, string url, string peer_id)
        {
            var uri = new Uri(url);
            using (WebClient client = new WebClient())
            {
                byte[] image = await client.DownloadDataTaskAsync(new Uri(url));

                MultipartFormDataContent form = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(image);
                imageContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(imageContent, "file", System.IO.Path.GetFileName(uri.LocalPath));
                HttpResponseMessage response = await _httpClient.PostAsync(upload_url, form);
                response.EnsureSuccessStatusCode();
                var responseBody = response.Content.ReadAsStringAsync().Result;

                _logger.Log(NLog.LogLevel.Info, responseBody);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoUploadResponse>(responseBody);
                return result;
            }
        }

        public async Task<PhotoSaveData> savePhoto(PhotoUploadResponse photoParams)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "method/photos.saveMessagesPhoto",
                Query = $"access_token={_token}&v={_apiVersion}"
            };

            var values = new Dictionary<string, string>
                {
                    { "photo", photoParams.photo},
                    { "server", photoParams.server},
                    { "hash", photoParams.hash }
                };

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(urlBuilder.Uri, content);

            var responseBody = response.Content.ReadAsStringAsync().Result;

            _logger.Log(NLog.LogLevel.Info, responseBody);
            
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoSaveResponse>(responseBody);
            return result.response.FirstOrDefault();
        }

        public async Task<string> savePhotoByUrl(string url, string peer_id)
        {
            var upload_url = await getMessagesUploadServer(peer_id);//mb upload_url should be stored
            var PhotoSaveData = await uploadPhoto(upload_url, url, peer_id);
            var result = await savePhoto(PhotoSaveData);
            return $"photo{result.owner_id}_{result.id}_{result.access_key}";

        }

        public async Task<string> imgFlipCaptionImage(string text)
        {
            var values = new Dictionary<string, string>
            {
                { "template_id",  _memeIds[_random.Next(0,_memeIds.Count())]},
                { "username", _imgFlipUsername },
                { "password", _imgFlipPassword },
                { "text0", text }
                //,{ "text1", "" }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(_imgFlipUrl, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.Log(NLog.LogLevel.Info, responseBody);
            Console.WriteLine(responseBody);

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptionImageResponse>(responseBody);
            if (result.success)
            {
                return result.data.url;
            }
            throw new Exception($"Imgflip error:{result.error_message}");
        }
    }
}
