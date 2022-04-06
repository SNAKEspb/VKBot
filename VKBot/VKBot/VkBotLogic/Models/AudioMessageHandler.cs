using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.Models
{
    public class AudioMessageHandler : IUpdatesHandler<IIncomingMessage>
    {
        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return message.attachments.Any(x => x.type == "audio");
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            //foreach (var attach in message.attachments.Where(x => x.type == "audio"))
            //{
            //    await bot.SendMessageAsync(new OutgoingMessage() { peer_id = message.peer_id, message = string.Format("{0} говно", attach.audio.artist) });
            //}

            return new HandlerResult() { message = "ok" };
        }
    }
}
