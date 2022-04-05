using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Http;

namespace VKBot.VkontakteBot.Services
{
    public class AwakerService
    {
        public static Timer aTimer = initTimer();
        //private static string url = "https://vityabot.herokuapp.com/api/heroku";
        private static string url = "http://localhost:5000/api/heroku";
        static HttpClient _httpClient = new HttpClient();
        private NLog.Logger _logger;

        public AwakerService(NLog.Logger logger) {
            _logger = logger;
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Log(NLog.LogLevel.Info, $"The Elapsed event was raised at {e.SignalTime}");
            var urlBuilder = new UriBuilder(url);
            logger.Log(NLog.LogLevel.Info, urlBuilder);
            var response = _httpClient.GetStringAsync(urlBuilder.Uri);
            logger.Log(NLog.LogLevel.Info, $"response {response}");
        }

        private static Timer initTimer()
        {
            var result = new System.Timers.Timer();
            result.Interval = 10000;//20*60*1000 //20min
            result.Elapsed += OnTimedEvent;
            result.AutoReset = true;
            result.Enabled = true;
            return result;
        }

        public void switchTimer(bool switcher)
        {
            aTimer.Enabled = switcher;
        }
    }
}
