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
        Task<string> audioToText(string url);
        Task processMemeAsync(IIncomingMessage message, string text);
        Task processMemeByIdAsync(string peerId, string memeId, string text);
        VKBot.VkBot.Mode mode { get; set; }
        Task<bool> getChatHistory(IIncomingMessage message);
        bool canProcess(string userId);
        Task processTextMessage(IIncomingMessage message);
        Task processBestMemeAsync(IIncomingMessage message, VkontakteBot.Models.GetMemesMemes bestMeme, List<string> memeText);
        Task processRandomBestMemeAsync(IIncomingMessage message);

        Task processPhoto(IIncomingMessage message);
    }
}
