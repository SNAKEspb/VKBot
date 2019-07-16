using SwearWordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VKBot.VkontakteBot.Services;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class TextBot : Models.MessagePatternHandler
    {
        static Random _random = new Random();
        public TextBot(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            //todo: random event
            var randInt = _random.Next(0, 100);
            if (randInt < 80)
            {
                await funcs[0](message, bot);
            }
            else if (randInt < 90)
            {
                await funcs[1](message, bot);
            }
            else if (randInt < 100)
            {
                await funcs[2](message, bot);
            }
        }

        public List<Func<IIncomingMessage, IVityaBot, Task>> funcs = new List<Func<IIncomingMessage, IVityaBot, Task>>()
        {
            async (message, bot) => {
                await bot.processRandomBestMemeAsync(message);
            },
            async (message, bot) => {
                var outgoingMessage = new Models.OutgoingMessage()
                {
                    peer_id = message.peer_id,
                    message = $"{Generator.generate(SwearWordGenerator.Type.Adjective, 2, Sex.M, Case.I)} {Generator.generate(SwearWordGenerator.Type.Noun, 2, Sex.M, Case.I)}!"
                };
                await bot.SendMessageAsync(outgoingMessage);
            },
            async (message, bot) => {
                var outgoingMessage = new Models.OutgoingMessage()
                {
                    peer_id = message.peer_id,
                    //message = text,
                    attachment = Services.DataService.pictures["respect"],
                    //group_id = message.
                };
                await bot.SendMessageAsync(outgoingMessage);
            }
    };

        private void swear(IIncomingMessage message, IVityaBot bot)
        {
            
        }
    }
}
