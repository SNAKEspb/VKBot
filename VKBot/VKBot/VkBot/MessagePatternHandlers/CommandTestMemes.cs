using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class CommandTestMemes : Models.MessagePatternHandler
    {
        public CommandTestMemes(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var memes = VkontakteBot.Services.DataService.activeMemes;
            foreach (var meme in memes)
            {
                await bot.processMemeByIdAsync(message.peer_id, meme.Id, meme.title + ";" + meme.description);
            }
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                message = $"test memes finished"
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
