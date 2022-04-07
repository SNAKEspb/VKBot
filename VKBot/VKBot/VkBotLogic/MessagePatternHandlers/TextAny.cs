using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MemeTextTranslator;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class TextAny : Models.MessagePatternHandler
    {
        static Random _random = new Random();

        public TextAny(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            if (KhaleesiService.calculateChance(message.text))
            {
                var outgoingMessage = new Models.OutgoingMessage()
                {
                    peer_id = message.peer_id,
                    message = _random.NextDouble() >= 0.5 ? KhaleesiService.generate(message.text) : CheemsService.generate(message.text)
                };
                await bot.SendMessageAsync(outgoingMessage);
            }
        }
    }
}
