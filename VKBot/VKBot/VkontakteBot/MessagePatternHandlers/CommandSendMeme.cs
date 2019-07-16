using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class CommandSendMeme : Models.MessagePatternHandler
    {
        public CommandSendMeme(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var peerId = match.Groups[1].Value;
            var memeId = match.Groups[2].Value;
            var text = match.Groups[3].Value;
            if (text == "random")
            {
                await bot.processRandomBestMemeAsync(new Models.UpdateMessage() { @object = new Models.UpdateMessageData() { peer_id = int.Parse(peerId) } });
            }
            else
            {
                await bot.processMemeByIdAsync(peerId, memeId, text);
            }
        }
    }
}
