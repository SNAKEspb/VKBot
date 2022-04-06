using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class CommandTestMemeById : Models.MessagePatternHandler
    {
        public CommandTestMemeById(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var memeId = match.Groups[1].Value;
            var memes = VKBot.VkBotLogic.Services.DataService.activeMemes;
            var meme = memes.FirstOrDefault(t => t.Id == memeId);
            if (meme != null)
            {
                await bot.processMemeByIdAsync(message.peer_id, meme.Id, meme.title + ";" + meme.description);
            }
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = $"test {memeId} finished"
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
