using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VKBot.VkontakteBot.Models;
using Google.Cloud.Speech.V1;

namespace VKBot
{
    public class VkBot : IVityaBot
    {
        static HttpClient _httpClient = new HttpClient();
        public static Random _random = new Random();

        static string _url { get; set; } = "https://api.vk.com/";
       
        static string _token { get; set; } = "0c29646fefcc442729f323eaf428f999dba1bcc95abfe3da03d0459c7b55fe6b965a59585b14c7a1c24af";
        static string _groupId { get; set; } = "179992947";
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

        public bool isTest { get; set; } = false;


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
            try
            {
                //todo: post cred command
                //Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("Cloud Project-a047b1f98427.json");
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "Cloud Project-a047b1f98427.json");
            }
            catch(Exception ex)
            {
                _logger.Log(NLog.LogLevel.Info, "VkBot Error");
                _logger.Log(NLog.LogLevel.Info, ex);
            }
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
            _logger.Log(NLog.LogLevel.Info, uri);
            using (WebClient client = new WebClient())
            {
                byte[] image = await client.DownloadDataTaskAsync(uri);

                MultipartFormDataContent form = new MultipartFormDataContent();
                using (var imageContent = new ByteArrayContent(image))
                {

                    imageContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
                    form.Add(imageContent, "file", System.IO.Path.GetFileName(uri.LocalPath));
                    HttpResponseMessage response = await _httpClient.PostAsync(upload_url, form);
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();

                    _logger.Log(NLog.LogLevel.Info, responseBody);

                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoUploadResponse>(responseBody);
                    return result;
                }
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

        //todo: move to separate service
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

        public async Task processMemeAsync(IIncomingMessage message, string text)
        {
            _logger.Log(NLog.LogLevel.Info, $"Process meme for peer_id: {message.peer_id} text:{text}");
            var memeUrl = await imgFlipCaptionImage(text);
            var photoId = await savePhotoByUrl(memeUrl, message.peer_id);
            var outgoingMessage = new OutgoingMessage()
            {
                peer_id = message.peer_id,
                //message = text,
                attachment = photoId,
                //group_id = message.
            };
            await SendMessageAsync(outgoingMessage);
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
                    byte[] audioSource = await client.DownloadDataTaskAsync(uri);
                    //System.IO.File.WriteAllBytes("test.mp3", audioSource);
                    byte[] audioGoogle = ConvertAudio(audioSource);
                    //System.IO.File.WriteAllBytes("test.wav", audioGoogle);
                    //ConvertMp3ToWav("test.mp3", "test.wav");

                    //send to google
                    var speechClient = SpeechClient.Create();
                    var recognitionConfig = new RecognitionConfig()
                    {
                        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRateHertz = 48000,
                        LanguageCode = "ru-RU",
                    };
                    var recognitionAudio = RecognitionAudio.FromBytes(audioGoogle);
                    var response = await speechClient.RecognizeAsync(recognitionConfig, recognitionAudio);

                    _logger.Log(NLog.LogLevel.Info, response);

                    return response.Results != null ? response.Results.SelectMany(t => t.Alternatives).Select(t => t.Transcript).FirstOrDefault() : null;
                }
            }
            catch(Exception ex)
            {
                //_logger.Log(NLog.LogLevel.Error, e, "getMessagesUploadServer Error");
                _logger.Log(NLog.LogLevel.Info, "audioToText Error");
                _logger.Log(NLog.LogLevel.Info, ex);
                return null;
            }
        }
        //todo: move to separate audio service
        public byte[] ConvertAudio(byte[] sourceBytes) {
            using (System.IO.Stream sourceStream = new System.IO.MemoryStream(sourceBytes)) {
                using (System.IO.Stream destinationStream = new System.IO.MemoryStream())
                {
                    ConvertMp3ToWav(sourceStream, destinationStream);
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        destinationStream.CopyTo(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        //todo: move to separate audio service
        private static void ConvertMp3ToWav(System.IO.Stream sourceStream, System.IO.Stream destinationStream)
        {
            using (var mp3 = new NAudio.Wave.Mp3FileReader(sourceStream))
            {
                using (NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    NAudio.Wave.WaveFileWriter.WriteWavFileToStream(destinationStream, pcm);
                   // NAudio.Wave.WaveFileWriter.CreateWaveFile("test.wav", pcm);
                }
            }
        }

        //private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        //{
        //    using (NAudio.Wave.Mp3FileReader mp3 = new NAudio.Wave.Mp3FileReader(_inPath_))
        //    {
        //        using (NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(mp3))
        //        {
        //            NAudio.Wave.WaveFileWriter.CreateWaveFile(_outPath_, pcm);
        //        }
        //    }
        //}
    }
}
