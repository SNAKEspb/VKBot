using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class WallMessageHandler : IUpdatesHandler<IIncomingMessage>
    {
        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return message.MessageType == "wall" && message.attachments != null && message.attachments.Any(x => x.type == "wall");
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            foreach (var attach in message.attachments.Where(x => x.type == "wall"))
            {
                await bot.SendMessageAsync(new OutgoingMessage() { peer_id = message.peer_id, message = string.Format("Опять всякое говно репостим") });
            }

            return new HandlerResult() { message = "ok" };
        }
    }
}
