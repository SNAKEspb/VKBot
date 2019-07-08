using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class TextHttp : Models.MessagePatternHandler
    {
        public TextHttp(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            //do nothing
        }
    }
}
