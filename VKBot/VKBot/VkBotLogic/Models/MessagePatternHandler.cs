using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.Models
{
    public class MessagePatternHandler
    {
        /// <summary>
        /// Regex patter
        /// </summary>
        public string pattern { get; }
        /// <summary>
        /// User access level: 0 - common text, 1 - user command, 2 - special, 4 - admin
        /// </summary>
        public byte access { get; }
        /// <summary>
        /// Handler execution priority
        /// </summary>
        public int priority { get; }//todo: set priority
        /// <summary>
        /// User ids for access = 2
        /// </summary>
        public List<string> userIds { get; }
        /// <summary>
        /// Regex with pattern
        /// </summary>
        public Regex regex { get; }
        /// <summary>
        /// Message handler
        /// </summary>
        /// <param name="">message</param>
        /// <param name="">bot</param>
        /// <param name="">regex match results</param>
        /// <returns></returns>
        public virtual Task handleAsync(IIncomingMessage message, IVityaBot bot, Match match)
        {
            throw new NotImplementedException();
        }

        public MessagePatternHandler(string pattern, int priority, byte access, List<string> userIds = null)
        {
            this.pattern = pattern;
            this.priority = priority;
            this.regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            this.access = access;
            this.userIds = userIds ?? new List<string>();
        }
    }
}
