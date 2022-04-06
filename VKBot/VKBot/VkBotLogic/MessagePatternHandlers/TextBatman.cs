using SwearWordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class TextBatman : Models.MessagePatternHandler
    {
        public TextBatman(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = $"Бэтмен {Generator.generate(SwearWordGenerator.Type.Adjective, 2, Sex.M, Case.I)} {Generator.generate(SwearWordGenerator.Type.Noun, 2, Sex.M, Case.I)}"
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
