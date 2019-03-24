using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class TextMessageHandler : IUpdatesHandler<IIncomingMessage>  
    {
        static string[] messageTypes = new[] { "message_new", "message_reply" };
        static string[] userIds = new[]
        {
            //"212515973",//vitya
            "1556462"//me
        };
        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return messageTypes.Contains(message.MessageType.ToLowerInvariant())
                && userIds.Contains(message.from_id)
                && !string.IsNullOrEmpty(message.text);
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            var memeUrl = await bot.imgFlipCaptionImage(message.text);
            var photoId = await bot.savePhotoByUrl(memeUrl, message.peer_id);
            var outgoingMessage = new OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = string.Format("Все говорят {0}, а ты пошел на хуй", message.text),
                attachment = photoId
            };
            await bot.SendMessageAsync(outgoingMessage);
            return new HandlerResult() { message = "ok" };
        }
    }
}
