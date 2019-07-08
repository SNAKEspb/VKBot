using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class CommandSend : Models.MessagePatternHandler
    {
        public CommandSend(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var peerId = match.Groups[1].Value;
            var text = match.Groups[2].Value;
            if (!string.IsNullOrEmpty(text))
            {
                var outgoingMessage = new Models.OutgoingMessage()
                {
                    peer_id = peerId,
                    message = text
                };
                await bot.SendMessageAsync(outgoingMessage);
            }
        }
    }
}
