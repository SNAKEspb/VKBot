using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot
{
    public interface IVityaBot
    {
        string confimationCode { get; }
        Task<IRegisterResponse> AuthorizeAsync();
        Task<IUpdatesResponse> GetUpdatesAsync();
        Task<bool> SendMessageAsync(IOutgoingMessage message);
        //сюда добавить методы заливания аттачей? картинок / аудио или хуйнують логику прямо в сенд мессаж
        Task<string> savePhotoByUrl(string url, string peer_id);
        Task<string> imgFlipCaptionImage(string text);
        Task<string> audioToText(string url);
        Task processMemeAsync(IIncomingMessage message, string text);
        bool isTest { get; set; }
    }
}
