using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using VKBot.VkontakteBot.Models;

namespace VKBot.VkontakteBot.Services
{
    public class ImgFlipService
    {
        static string _url { get; set; } = "https://api.imgflip.com/";
        static HttpClient _httpClient = new HttpClient();
        private NLog.Logger _logger;

        public ImgFlipService(NLog.Logger logger)
        {
            _logger = logger;
        }

        public async Task<string> imgFlipCaptionImage(string id, string text, string username, string password)
        {
            return await imgFlipCaptionImage(id, new string[] { text }, username, password);
        }

        public async Task<string> imgFlipCaptionImage(string id, string[] text, string username, string password)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "caption_image",
            };

            var values = new Dictionary<string, string>
            {
                { "template_id",  id},
                { "username", username },
                { "password", password },
                { "text0", text[0] },
            };

            if (text.Length > 2)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    values.Add($"boxes[{i}][text]", text[i]);
                }
            }
            else if (text.Length > 1)
            {
                values.Add("text1", text[1]);
            }

            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync(urlBuilder.Uri, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.Log(NLog.LogLevel.Info, responseBody);

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CaptionImageResponse>(responseBody);
            if (result.success)
            {
                return result.data.url;
            }
            throw new Exception($"Imgflip error:{result.error_message}");
        }

        public async Task<List<GetMemesMemes>> imgFlipGetMemes(string username, string password)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "get_memes"
            };

            var response = await _httpClient.GetAsync(urlBuilder.Uri);
            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.Log(NLog.LogLevel.Info, responseBody);

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetMemesResponse>(responseBody);
            if (result.success)
            {
                return result.data.memes;
            }
            throw new Exception($"Imgflip error:{result.error_message}");
        }
    }
}
