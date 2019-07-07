﻿using SwearWordGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VKBot.VkontakteBot.Models;

namespace VKBot.VkontakteBot.Services
{
    public class MessageService
    {
        public class MessagePattern
        {
            public string pattern;
            public byte access; // 0 - common text, 1 - user command, 2 - special, 4 - admin
            public int priority;//todo: set priority
            public List<string> userIds;
            public Regex regex;
            public Func<IIncomingMessage, IVityaBot, Match, Task> func;
            public MessagePattern(string pattern, byte access, List<string> userIds = null)
            {
                this.pattern = pattern;
                this.regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                this.access = access;
                this.userIds = userIds ?? new List<string>();
            }
        }

        private NLog.Logger _logger;

        static string _vitya = "212515973";

        static Dictionary<string, string> pictures = new Dictionary<string, string>()
        {
            { "joke", "photo-179992947_456239020"},
            { "respect", "photo-179992947_456239021"},
        };

        static Dictionary<string, int> usersRights = new Dictionary<string, int>()
        {
            {_vitya, 2 },//vitya
            {"1556462", 4 }, //me
        };

        //todo: add interface and separate classes for each pattern
        static List<MessagePattern> patterns = new List<MessagePattern>()
        {
            new MessagePattern(@"^\/status", 4) { func = async  (message, bot, match) =>
                {
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"bot test mode is {bot.mode.ToString()}"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"^\/mode\s*(.*)", 4) { func = async  (message, bot, match) =>
                {
                    bot.mode = Enum.Parse<VKBot.VkBot.Mode>(match.Groups[1].Value);
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"bot test mode is {bot.mode.ToString()}"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"^\/test\s*memes", 4) { func = async  (message, bot, match) =>
                {
                    var memes = VkontakteBot.Services.DataService.activeMemes;
                    foreach(var meme in memes)
                    {
                        await bot.processMemeByIdAsync(message, meme.Id, meme.title + ";" + meme.description);
                    }
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"test memes finished"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"^\/test\s*memeid\s*(.*)", 4) { func = async  (message, bot, match) =>
                {
                    var memeId = match.Groups[1].Value;
                    var memes = VkontakteBot.Services.DataService.activeMemes;
                    var meme = memes.FirstOrDefault(t => t.Id == memeId);
                    if (meme != null)
                    {
                        await bot.processMemeByIdAsync(message, meme.Id, meme.title + ";" + meme.description);
                    }
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"test {memeId} finished"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"^\/send\s*(\d{1,20})\s(.*)", 4) { func = async  (message, bot, match) =>
                {
                    var peerId = match.Groups[1].Value;
                    var text = match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(text))
                    {
                        var outgoingMessage = new OutgoingMessage()
                        {
                            peer_id = peerId,
                            message = text
                        };
                        await bot.SendMessageAsync(outgoingMessage);
                    }
                }
            },
            //new MessagePattern("^\\/history", 4){ func = async  (message, bot, match) =>
            //    {
            //        await bot.getChatHistory(message);
            //        //await bot.SendMessageAsync(outgoingMessage);
            //    }
            //},
            new MessagePattern(@".*", 2, new List<string>(){ _vitya}) { func = async  (message, bot, match) =>
                {
                    await bot.processMemeAsync(message, match.Value);
                }
            },
            new MessagePattern(@"(?:^!|(?:говорит|сказал):?\s(?:что)?)\s*(.*)", 1){ func = async  (message, bot, match) =>
                {
                    await bot.processMemeAsync(message, match.Groups[1].Value);
                }
            },
            new MessagePattern(@"бетмен", 0){ func = async  (message, bot, match) =>
                {
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"Бетмен {Generator.generate(SwearWordGenerator.Type.Adjective, 2, Sex.M, Case.I)} {Generator.generate(SwearWordGenerator.Type.Noun, 2, Sex.M, Case.I)}"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"^http", 0){ func = async  (message, bot, match) =>
                {
                    //todo: url picture or random phrase
                    //опять хуйню постишь?
                }
            },
            new MessagePattern(@"уважение|увожение", 0){ func = async  (message, bot, match) =>
                {
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        //message = text,
                        attachment = pictures["respect"],
                        //group_id = message.
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"((?:заеб|залуп|говн|плох|хуй|хуе).*шутк[a|и]?)|((?:^)?шутк[a|и]?.*(?:заеб|залуп|говн|плох|хуй|хуе|заеб))|(шутит|шутишь|шутканул|пошутил)", 0){ func = async  (message, bot, match) =>
                {
                     var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        //message = text,
                        attachment = pictures["joke"],
                        //group_id = message.
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }
            },
            new MessagePattern(@"(^|\.|\,|\s)бот(\w{0,2})?", 0){ func = async  (message, bot, match) =>
                {
                    //todo: random event
                }
            },
            
        }.OrderByDescending(t => t.access).ThenByDescending(t => t.pattern.Length).ToList();//access > pattern lenght

        public MessageService(NLog.Logger logger)
        {
            _logger = logger;
        }

        public async Task processMessage(IIncomingMessage message, IVityaBot bot)
        {
            //get user access rights
            var userAccess = usersRights[message.from_id];
            //get user patterns
            var userPatterns = patterns.Where(t => (t.access != 2 && t.access <= userAccess) || (t.access == 2 && t.userIds.Contains(message.from_id)));
            foreach (var pattern in userPatterns)
            {
                Match match = pattern.regex.Match(message.text);
                if (match.Success)
                {
                    await pattern.func(message, bot, match);
                    break;
                }
            }
        }
    }
}
