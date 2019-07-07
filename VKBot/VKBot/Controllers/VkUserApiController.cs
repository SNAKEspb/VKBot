using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VKBot.Controllers
{
    public class VkUserApiController : Controller
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Index()
        {
            return "Nothing is here!";
        }

        public RedirectResult Auth()
        {
            string _url = "https://oauth.vk.com/";
            string client_id = "7030347";
            string display = "page";
            string redirect_uri = "https://vityabot.herokuapp.com/vkuserapi/authcallback";
            string response_type = "token";
            string version = "5.95";
            string state = "vkbot";
            var urlBuilder = new UriBuilder(_url)
            {
                Path = "authorize",
                Query = $"client_id={client_id}&display={display}&redirect_uri={redirect_uri}&response_type={response_type}&v={version}&state={state}"
            };

            _logger.Log(NLog.LogLevel.Info, urlBuilder.Uri.ToString());
            return Redirect(urlBuilder.Uri.ToString());
        }

        public IActionResult AuthCallBack()
        {
            var test = Request;
            return View(); //"vkauthorization page";
        }

        public string AuthConfirm(string access_token, string expires_in, string user_id, string state)
        {
            //var _url = "https://api.vk.com/";
            //var urlBuilder = new UriBuilder(_url)
            //{
            //    Path = "method/messages.getHistory",
            //    Query = $"access_token={access_token}&v={"5.95"}"
            //};
            //_logger.Log(NLog.LogLevel.Info, urlBuilder);

            //var values = new Dictionary<string, string>
            //    {
            //        { "offset", "0"},
            //        { "count", "200"},
            //        { "filter", "all" },
            //        { "extended", "0" },
            //        { "start_message_id", "0" },
            //    };

            //var content = new System.Net.Http.FormUrlEncodedContent(values);

            //System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

            //var response = _httpClient.PostAsync(urlBuilder.Uri, content).Result;

            //var responseBody = response.Content.ReadAsStringAsync();

            //_logger.Log(NLog.LogLevel.Info, responseBody);

            //var request = new VkontakteBot.Services.VKService.HistoryRequest
            //{
            //    offset = 0,
            //    count = 200,
            //    user_id = "1556462",
            //    peer_id = "1500589",
            //    start_message_id = "0",
            //    rev = "1",
            //    //extended = 
            //    //fields = 
            //    group_id = _groupId,
            //};
            //var vkService = new VkontakteBot.Services.VKService(_logger);
            //vkService.messagesGetHistory(request, access_token, "5.95");

            return access_token;
        }
    }
}