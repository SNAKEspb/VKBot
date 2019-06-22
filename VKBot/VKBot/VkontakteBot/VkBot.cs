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
        //static HttpClient _httpClient = new HttpClient();
        public static Random _random = new Random();

        //static string _url { get; set; } = "https://api.vk.com/";
       
        static string _token { get; set; } = "0c29646fefcc442729f323eaf428f999dba1bcc95abfe3da03d0459c7b55fe6b965a59585b14c7a1c24af";
        static string _groupId { get; set; } = "179992947";
        static string _apiVersion { get; set; } = "5.92";
        static string _wait { get; set; } = "25";
        static string _confirmationCode { get; set; } = "821df2ec";

        static string _imgFlipUrl { get; set; } = "https://api.imgflip.com/caption_image";
        static string _imgFlipUsername = "VityaBot";
        static string _imgFlipPassword = "vityaBot1!";

        static string _onlineConverterApiKey = "877a7639b9c340c976881ef851ce7a47";

        static string[] _adminIds = new[]
        {
            "1556462"//me
        };

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
        //todo: different modes
        public bool isTest { get; set; }


        private NLog.Logger _logger;
        private VkontakteBot.Services.VKService vkService;
        private VkontakteBot.Services.ImgFlipService imgflipService;
        private VkontakteBot.Services.OnlineConverterService onlineConverterService;
        private VkontakteBot.Services.GoogleService googleService;


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
            vkService = new VkontakteBot.Services.VKService(logger);
            imgflipService = new VkontakteBot.Services.ImgFlipService(logger);
            onlineConverterService = new VkontakteBot.Services.OnlineConverterService(logger, _onlineConverterApiKey);
            googleService = new VkontakteBot.Services.GoogleService(logger);
            isTest = true;
            try
            {
                //todo: post cred command
                //Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("Cloud Project-a047b1f98427.json");
                
            }
            catch(Exception ex)
            {
                _logger.Log(NLog.LogLevel.Info, "VkBot Error");
                _logger.Log(NLog.LogLevel.Info, ex);
            }
        }

        public bool couldProcess(string userId)
        {
            return _adminIds.Contains(userId) || !isTest;
        }

        public string confimationCode => _confirmationCode;
        public async Task<IRegisterResponse> AuthorizeAsync()
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, "registerStart");

                var result = await vkService.groupsGetLongPollServerAsync(_groupId, _token, _apiVersion);

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

                var result = await vkService.checkLongPollUpdatesAsync(_server, _ts, _key, _wait, _apiVersion);

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
                _logger.Log(NLog.LogLevel.Error, e, "GetUpdatesStart error");
                return null;
            }


            throw new NotImplementedException();
        }


        public async Task<bool> SendMessageAsync(IOutgoingMessage message)
        {
            try
            {
                var tmessage = (OutgoingMessage)message;

                return await vkService.messagesSendAsync(tmessage, _groupId, _token, _apiVersion);
            }
            catch (Exception e)
            {
                _logger.Log(NLog.LogLevel.Error, e, "SendMessageAsync error");
                return false;
            }

        }

        public async Task processMemeAsync(IIncomingMessage message, string text)
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, $"Process meme for peer_id: {message.peer_id} text:{text}");
                var memeUrl = await imgflipService.imgFlipCaptionImage(_memeIds[_random.Next(0, _memeIds.Count())], text, _imgFlipUsername, _imgFlipPassword);
                var photoId = await vkService.savePhotoByUrl(memeUrl, message.peer_id, _token, _apiVersion);
                var outgoingMessage = new OutgoingMessage()
                {
                    peer_id = message.peer_id,
                    //message = text,
                    attachment = photoId,
                    //group_id = message.
                };
                await SendMessageAsync(outgoingMessage);
            }
            catch(Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "getMessagesUploadServer Error");
            }
        }

        //todo: move to separate audio/google service
        public async Task<string> audioToText(string url)
        {

            try
            {
                var uri = new Uri(url);
                _logger.Log(NLog.LogLevel.Info, uri);
                using (WebClient client = new WebClient())
                {
                    //get audio
                    byte[] audioMP3 = await client.DownloadDataTaskAsync(uri);
                    byte[] audioWav = VkontakteBot.Services.AudioService.decodeMP3ToWavMono(audioMP3);
                    return await googleService.wavToText(audioWav);
                }
                
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "audioToText Error");
                return null;
            }
        }

        //todo: move convert logic to service
        public async Task<string> audioToTextOnlineConverter(string url)
        {
            try
            {
                var uri = new Uri(url);
                _logger.Log(NLog.LogLevel.Info, uri);
                using (WebClient client = new WebClient())
                {
                    var flacUrl = await onlineConverterService.convertMp3ToFlac(url);
                    byte[] audioFlac = await client.DownloadDataTaskAsync(uri);
                    return await googleService.flacToText(audioFlac);
                }
            }
            catch(Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "audioToText Error");
                return null;
            }
        }

        public async Task<bool> getChatHistory(IIncomingMessage message)
        {
            try
            {
                //var uri = new Uri(url);
                //_logger.Log(NLog.LogLevel.Info, uri);
                var request = new VkontakteBot.Services.VKService.HistoryRequest
                {
                    offset = 0,
                    count = 200,
                    user_id = "1556462",
                    peer_id = message.peer_id,
                    start_message_id = "0",
                    rev = "1",
                    //extended = 
                    //fields = 
                    group_id = _groupId,
                };
                await vkService.messagesGetHistory(request, _token, _apiVersion);
                
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "audioToText Error");
            }
            return false;
        }


    }
}
