using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VKBot.VkontakteBot.Models;

namespace VKBot
{
    public class VkBot : IVityaBot
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private VkontakteBot.Services.VKService vkService;
        private VkontakteBot.Services.ImgFlipService imgflipService;
        private VkontakteBot.Services.OnlineConverterService onlineConverterService;
        private VkontakteBot.Services.GoogleService googleService;
        private VkontakteBot.Services.MessageService messageService;
        private VkontakteBot.Services.ImageService imageService;
        private VKBotOptions _options;

        public VkBot(IOptions<VKBotOptions> options)
        {
            _options = options.Value;

            vkService = new VkontakteBot.Services.VKService(_logger);
            imgflipService = new VkontakteBot.Services.ImgFlipService(_logger);
            onlineConverterService = new VkontakteBot.Services.OnlineConverterService(_logger, _onlineConverterApiKey);
            googleService = new VkontakteBot.Services.GoogleService(_logger);
            messageService = new VkontakteBot.Services.MessageService(_logger);
            imageService = new VkontakteBot.Services.ImageService(_logger);
            mode = Mode.Release;
            try
            {
                //todo: post cred command
                //Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("Cloud Project-a047b1f98427.json");

            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Info, "VkBot Error");
                _logger.Log(NLog.LogLevel.Info, ex);
            }
        }

        public string confimationCode => _options.confirmationCode;


        public enum Mode
        {
            Release,
            Debug,
            DebugOnly
        }

        public static Random _random = new Random();

        static string _imgFlipUrl { get; set; } = "https://api.imgflip.com/caption_image";
        static string _imgFlipUsername = "VityaBot";
        static string _imgFlipPassword = "vityaBot1!";

        static string _onlineConverterApiKey = "877a7639b9c340c976881ef851ce7a47";

        static List<VkontakteBot.Services.DataService.Meme> _memes = VkontakteBot.Services.DataService.activeMemes;

        //for longpoll
        static string _key { get; set; }
        static string _ts { get; set; }
        static string _server { get; set; }

        public Mode mode { get; set; }

        public bool canProcess(string userId)
        {
            return mode != Mode.DebugOnly || VkontakteBot.Services.DataService.adminsIds.Contains(userId);
        }

        public async Task<IRegisterResponse> AuthorizeAsync()
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, "registerStart");

                var result = await vkService.groupsGetLongPollServerAsync(_options.groupId, _options.token, _options.apiVersion);

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

                var result = await vkService.checkLongPollUpdatesAsync(_server, _ts, _key, _options.wait, _options.apiVersion);

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

                return await vkService.messagesSendAsync(tmessage, _options.groupId, _options.token, _options.apiVersion);
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
                var meme = _memes[_random.Next(0, _memes.Count())];
                await processMemeByIdAsync(message.peer_id, meme.Id, text);
            }
            catch(Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "getMessagesUploadServer Error");
            }
        }

        public async Task processMemeByIdAsync(string peerId, string memeId, string text)
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, $"Process meme for peer_id: {peerId}, meme id:{memeId}, text:{text}");
                var memeUrl = await imgflipService.imgFlipCaptionImage(memeId, text, _imgFlipUsername, _imgFlipPassword);
                var photoId = await vkService.savePhotoByUrl(memeUrl, peerId, _options.token, _options.apiVersion);
                var outgoingMessage = new OutgoingMessage()
                {
                    peer_id = peerId,
                    //message = text,
                    attachment = photoId,
                    //group_id = message.
                };
                await SendMessageAsync(outgoingMessage);
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "getMessagesUploadServer Error");
            }
        }

        public async Task processTextMessage(IIncomingMessage message)
        {
            try
            {
                await messageService.processMessage(message, this);
            }
            catch (Exception ex)
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

        public async Task processBestMemeAsync(IIncomingMessage message, GetMemesMemes bestMeme, List<string> memeText)
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, $"processBestMemeAsync for peer_id: {message.peer_id}");
                

                var memeUrl = await imgflipService.imgFlipCaptionImage(bestMeme.id, memeText, _imgFlipUsername, _imgFlipPassword);
                var photoId = await vkService.savePhotoByUrl(memeUrl, message.peer_id, _options.token, _options.apiVersion);
                var outgoingMessage = new OutgoingMessage()
                {
                    peer_id = message.peer_id,
                    //message = text,
                    attachment = photoId,
                    //group_id = message.
                };
                await SendMessageAsync(outgoingMessage);
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "getMessagesUploadServer Error");
            }
        }

        public async Task processRandomBestMemeAsync(IIncomingMessage message)
        {
            try
            {
                _logger.Log(NLog.LogLevel.Info, $"processRandomBestMemeAsync for peer_id: {message.peer_id}");
                var bestMemes = await imgflipService.bestMemes(_imgFlipUsername, _imgFlipPassword);
                var randomMeme = bestMemes[_random.Next(0, bestMemes.Count)];

                var randomMessages = new List<string>();
                for (int i = 0; i < int.Parse(randomMeme.box_count); i++)
                {
                    randomMessages.Add(VkontakteBot.Services.DataService.vityaMessages[_random.Next(0, VkontakteBot.Services.DataService.vityaMessages.Count)]);
                }

                await processBestMemeAsync(message, randomMeme, randomMessages);
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "getMessagesUploadServer Error");
            }
        }

        [Obsolete("no need")]
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
                    group_id = _options.groupId,
                };
                await vkService.messagesGetHistory(request, _options.token, _options.apiVersion);

            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "audioToText Error");
            }
            return false;
        }

        public async Task processPhoto(IIncomingMessage message)
        {
            try
            {
                var photo = message.attachments.Select(t => t.photo.sizes.FirstOrDefault(size => size.type == "w")).FirstOrDefault();
                if (photo == null) {
                    photo = message.attachments.Select(t => t.photo.sizes.OrderByDescending(size => size.width).ThenBy(size => size.height).FirstOrDefault()).FirstOrDefault();
                }
                var url = photo.url;
                if (string.IsNullOrWhiteSpace(url)) {
                    return;
                }

                var uploadRegisterResponse = await vkService.photosGetMessagesUploadServerAsync(message.peer_id, _options.token, _options.apiVersion);
                //mb upload_url should be stored

                using (WebClient client = new WebClient())
                {
                    byte[] photoBytes = await client.DownloadDataTaskAsync(url);

                    var text = VkontakteBot.Services.DataService.vityaShortMessages[_random.Next(0, VkontakteBot.Services.DataService.vityaShortMessages.Count)];

                    var newPhotoBytes = imageService.addTextToImage(photoBytes, text);

                    var photoUploadResponse = await vkService.uploadPictureAsync(uploadRegisterResponse.response.upload_url, message.peer_id, System.IO.Path.GetFileName(url), newPhotoBytes);
                    var photoSaveResponse = await vkService.photosSaveMessagesPhotoAsync(photoUploadResponse, _options.token, _options.apiVersion);
                    var photoSaveData = photoSaveResponse.response.FirstOrDefault();

                    var photoId = $"photo{photoSaveData.owner_id}_{photoSaveData.id}_{photoSaveData.access_key}";

                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        //message = text,
                        attachment = photoId,
                        //group_id = message.
                    };
                    await SendMessageAsync(outgoingMessage);
                }

                
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "audioToText Error");
            }
        }

        //public async Task<String> getConfirmationMessage(IIncomingMessage message) {
        //    var codeDict = new Dictionary<int, string>();
        //    codeDict.Add(179992947, "f04bcc93");
        //    codeDict.Add(212459550, "88cb2a80");
        //    string result = "";
        //    codeDict.TryGetValue(message.group_id, out result);
        //    return result;
        //}


    }
}
