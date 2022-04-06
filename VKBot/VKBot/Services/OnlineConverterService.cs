using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using VKBot.VkBotLogic.Models;

namespace VKBot.Services
{
    public class OnlineConverterService
    {
        static string _url { get; set; } = "https://api2.online-convert.com/";
        static HttpClient _httpClient = new HttpClient();
        private NLog.Logger _logger;

        public OnlineConverterService(NLog.Logger logger, string apiKey)
        {
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("X-Oc-Api-Key", apiKey);
        }

        public async Task<ConvertOnlineResponse> startConvertMp3ToFlac(string sourceUrl)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "jobs",
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);

            string jsonInString = $"{{\"input\":[{{\"type\":\"remote\",\"source\":\"{sourceUrl}\"}}],\"conversion\":[{{\"category\":\"audio\",\"target\":\"flac\", \"options\": {{\"channels\": \"mono\"}} }}] }}";

            var response = await _httpClient.PostAsync(urlBuilder.Uri, new StringContent(jsonInString, System.Text.Encoding.UTF8, "application/json"));

            var responseBody = response.Content.ReadAsStringAsync().Result;

            _logger.Log(NLog.LogLevel.Info, responseBody);
            Console.WriteLine(responseBody);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ConvertOnlineResponse>(responseBody);
        }

        public async Task<ConvertOnlineResponse> checkStatus(string jobId)
        {
            var urlBuilder = new UriBuilder(_url)
            {
                Path = $"jobs/{jobId}",
            };
            _logger.Log(NLog.LogLevel.Info, urlBuilder);

            var response = await _httpClient.GetStringAsync(urlBuilder.Uri);
            _logger.Log(NLog.LogLevel.Info, response);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ConvertOnlineResponse>(response);
        }

        public async Task<string> convertMp3ToFlac(string url)
        {
            var result = await startConvertMp3ToFlac(url);
            ConvertOnlineResponse checkStatusResult = null;
            for (int i = 0; i < 5; i++)
            {
                checkStatusResult = await checkStatus(result.id);
                if (checkStatusResult.status.code == "competed" || checkStatusResult.status.code == "failed")
                {
                    break;
                }
                await Task.Delay(1000);

            }
            return checkStatusResult.output.FirstOrDefault().uri;
        }
    }
}
