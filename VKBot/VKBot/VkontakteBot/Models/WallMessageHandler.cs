using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class WallMessageHandler : IUpdatesHandler<IIncomingMessage>
    {
        //todo:move statics to bot
        static string[] _messageTypes = new[] { "message_new", "message_reply" };
        static string[] _userIds = new[]
        {
            "212515973",//vitya
        };

        

        static string _wallPick = "photo-179992947_456239019";

        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return bot.canProcess(message.from_id)
                &&_messageTypes.Contains(message.MessageType.ToLowerInvariant()) 
                && _userIds.Contains(message.from_id)
                && message.attachments != null
                && message.attachments.Any(x => x.type == "wall");
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            var outgoingMessage = new OutgoingMessage()
            {
                peer_id = message.peer_id,
                //message = text,
                attachment = _wallPick,
                //group_id = message.
            };
            await bot.SendMessageAsync(outgoingMessage);
            return new HandlerResult() { message = "ok" };
        }
    }
}
