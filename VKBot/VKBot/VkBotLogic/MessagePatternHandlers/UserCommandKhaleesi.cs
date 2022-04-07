using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MemeTextTranslator;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class UserCommandKhaleesi : Models.MessagePatternHandler
    {
        static Random _random = new Random();

        public UserCommandKhaleesi(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = KhaleesiService.generate(match.Groups[1].Value)
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
