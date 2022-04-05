using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class CommandTestInfo : Models.MessagePatternHandler
    {
        public CommandTestInfo(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }
        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = SixLabors.Fonts.SystemFonts.Collection.Families.Where(t => !string.IsNullOrEmpty(t.Name)).Aggregate("", (res, t) => res += "\n " + t.Name)
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
