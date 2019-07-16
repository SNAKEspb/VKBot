using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class CommandMemeList : Models.MessagePatternHandler
    {
        public CommandMemeList(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var peers = VkontakteBot.Services.DataService.activeMemes.Select(t => t.Id + " - " + t.title + " - " + t.description);
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = string.Join("\n", peers)
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
