﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.MessagePatternHandlers
{
    public class UserCommandMeme : Models.MessagePatternHandler
    {
        public UserCommandMeme(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            await bot.processMemeAsync(message, match.Groups[1].Value);
        }
    }
}
