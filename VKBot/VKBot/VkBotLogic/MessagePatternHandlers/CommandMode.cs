using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class CommandMode : Models.MessagePatternHandler
    {
        public CommandMode(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            bot.mode = Enum.Parse<VkBot.Mode>(match.Groups[1].Value);
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = $"bot test mode is {bot.mode.ToString()}"
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
