﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.MessagePatternHandlers
{
    public class TextJoke : Models.MessagePatternHandler
    {
        public TextJoke(string pattern, int priority, byte access, List<string> userIds = null) : base(pattern, priority, access, userIds) { }

        public override async Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            var outgoingMessage = new Models.OutgoingMessage()
            {
                peer_id = message.peer_id,
                //message = text,
                attachment = Services.DataService.pictures["joke"],
                //group_id = message.
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
