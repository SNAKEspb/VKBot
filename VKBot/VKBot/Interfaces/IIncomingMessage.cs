using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKBot.VkontakteBot.Models;

namespace VKBot
{
    public interface IIncomingMessage
    {
        string peer_id { get; }
        string MessageType { get; }
        string from_id { get; }
        string date { get; }

        string text { get; }
        List<Attachment> attachments { get; }

    }
  
}
