using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class CommandPeerList : Models.MessagePatternHandler
    {
        public CommandPeerList(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var peers = VKBot.VkBotLogic.Services.DataService.peers.Select(t => t.Value + " - " + t. Key);
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = string.Join("\n", peers)
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
