using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class CommandStatus : Models.MessagePatternHandler
    {
        public CommandStatus(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = $"bot test mode is {bot.mode.ToString()}"
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
