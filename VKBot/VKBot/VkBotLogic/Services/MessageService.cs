using SwearWordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VKBot.VkBotLogic.Models;

namespace VKBot.VkBotLogic.Services
{
    public class MessageService
    {
        //public class MessagePattern
        //{
        //    public string pattern;
        //    public byte access; // 0 - common text, 1 - user command, 2 - special, 4 - admin
        //    public int priority;//todo: set priority
        //    public List<string> userIds;
        //    public Regex regex;
        //    public Func<IIncomingMessage, IVityaBot, Match, Task> func;
        //    public MessagePattern(string pattern, byte access, List<string> userIds = null)
        //    {
        //        this.pattern = pattern;
        //        this.regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //        this.access = access;
        //        this.userIds = userIds ?? new List<string>();
        //    }
        //}

        private NLog.Logger _logger;

        static List<MessagePatternHandler> patternHandlers = new List<MessagePatternHandler>()
        {
            new MessagePatternHandlers.CommandMemeList(@"^\/memelist", 10, 4),
            new MessagePatternHandlers.CommandMode(@"^\/mode\s*(.*)", 10, 4),
            new MessagePatternHandlers.CommandPeerList(@"^\/peerlist", 10, 4),
            new MessagePatternHandlers.CommandSend(@"^\/send\s*(\d{1,20})\s(.*)", 10, 4),
            new MessagePatternHandlers.CommandSendMeme(@"^\/sendmeme\s*(\d{1,20})\s*(\d{1,20})\s(.*)", 10, 4),
            new MessagePatternHandlers.CommandStatus(@"^\/status", 10, 4),
            new MessagePatternHandlers.CommandTestMemeById(@"^\/test\s*memeid\s*(.*)", 10, 4),
            new MessagePatternHandlers.CommandTestMemes(@"^\/test\s*memes", 10,  4),
            new MessagePatternHandlers.CommandTestInfo(@"^\/test\s*info", 10,  4),
            new MessagePatternHandlers.TextAnyVitya(@".*", 9, 2, new List<string>(){ DataService.vityaId}),
            new MessagePatternHandlers.TextBatman(@"б[еэ]тм[ае]н|b[ae]tm[ae]n", 9, 0),
            new MessagePatternHandlers.TextBot(@"(^|\.|\,|\s)бот(\w{0,2})?", 9, 0),
            new MessagePatternHandlers.TextHttp(@"^http", 10, 0),
            new MessagePatternHandlers.TextJoke(@"((?:заеб|залуп|говн|плох|хуй|хуе).*шутк[a|и]?)|((?:^)?шутк[a|и]?.*(?:заеб|залуп|говн|плох|хуй|хуе|заеб))|(шутит|шутишь|шутканул|пошутил)", 9, 0),
            new MessagePatternHandlers.TextRespect(@"уважение|увожение", 9, 0),
            new MessagePatternHandlers.UserCommandMeme(@"(?:^!|(?:говорит|сказал):?\s(?:что)?)\s*(.*)",9, 1),
            new MessagePatternHandlers.UserCommandRandom(@"^!random", 10, 1),
        }.OrderByDescending(t => t.priority).ThenByDescending(t => t.access).ThenByDescending(t => t.pattern.Length).ToList();//priority > access >  pattern lenght;

        public MessageService(NLog.Logger logger)
        {
            _logger = logger;
        }

        public async Task processMessage(IIncomingMessage message, IVityaBot bot)
        {
            //get user access rights
            var userAccess = Services.DataService.userAccess(message.from_id);
            //get user patterns
            var userPatternHandlers = patternHandlers.Where(t => (t.access != 2 && t.access <= userAccess) || (t.access == 2 && t.userIds.Contains(message.from_id)));
            foreach (var pattern in userPatternHandlers)
            {
                Match match = pattern.regex.Match(message.text);
                if (match.Success)
                {
                    await pattern.handleAsync(message, bot, match);
                    break;
                }
            }
        }
    }
}
