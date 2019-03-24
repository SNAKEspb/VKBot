using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class CaptionImageResponse
    {
        public bool success;
        public string error_message;
        public CaptionImageData data;
    }

    public class CaptionImageData
    {
        public string url;
        public string page_url;
    }
}
