using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class PhotoMessageHandler : IUpdatesHandler<IIncomingMessage>
    {
        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return message.attachments.Any(x => x.type == "photo");
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            bot.processPhoto(message);

            return new HandlerResult() { message = "ok" };
        }
    }
}
