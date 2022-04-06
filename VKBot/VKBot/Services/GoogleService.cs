using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;

namespace VKBot.Services
{
    public class GoogleService
    {
        private NLog.Logger _logger;
        public GoogleService(NLog.Logger logger)
        {
            _logger = logger;
        }
        //todo: move to google service
        public async Task<string> flacToText(byte[] audio)
        {
            //send to google
            var speechClient = SpeechClient.Create();
            var recognitionConfig = new RecognitionConfig()
            {
                //EnableAutomaticPunctuation = true,
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                LanguageCode = "ru-Ru",
                Model = "default",
                SampleRateHertz = 48000,
            };
            var recognitionAudio = RecognitionAudio.FromBytes(audio);
            var response = await speechClient.RecognizeAsync(recognitionConfig, recognitionAudio);

            _logger.Log(NLog.LogLevel.Info, response);

            return response.Results != null ? response.Results.SelectMany(t => t.Alternatives).Select(t => t.Transcript).FirstOrDefault() : null;
        }

         public async Task<string> wavToText(byte[] audio)
        {
            var speechClient = SpeechClient.Create();
            var recognitionConfig = new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 48000,
                LanguageCode = "ru-RU",
            };
            var recognitionAudio = RecognitionAudio.FromBytes(audio);
            var response = await speechClient.RecognizeAsync(recognitionConfig, recognitionAudio);

            _logger.Log(NLog.LogLevel.Info, response);

            return response.Results != null ? response.Results.SelectMany(t => t.Alternatives).Select(t => t.Transcript).FirstOrDefault() : null;
        }
    }
}
