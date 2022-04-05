using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace StirkaBot.VKBot.Models
{
    public class VKBotOptions
    {
        public string token { get; set; }
        public string groupId { get; set; }
        public string apiVersion { get; set; }
        public string confirmationCode { get; set; }
    }
}
