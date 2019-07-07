using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class TextMessageHandler : IUpdatesHandler<IIncomingMessage>  
    {
        //todo:move statics to bot
        static string[] _messageTypes = new[] { "message_new", "message_reply" };

        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return bot.canProcess(message.from_id)
                && _messageTypes.Contains(message.MessageType.ToLowerInvariant())
                && !string.IsNullOrWhiteSpace(message.text);
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            await bot.processTextMessage(message);
            return new HandlerResult() { message = "ok" };
        }
    }
}
