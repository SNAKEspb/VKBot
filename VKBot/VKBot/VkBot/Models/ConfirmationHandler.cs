using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class ConfirmationHandler : IUpdatesResultHandler<IIncomingMessage>
    {
        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return message.MessageType == "confirmation";
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            return new HandlerResult() { message = bot.confimationCode };
        }

    }
}
