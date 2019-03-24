using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class TextMessageHandler : IUpdatesHandler<IIncomingMessage>  
    {
        //todo:move statics to bot
        static string[] _messageTypes = new[] { "message_new", "message_reply" };
        static string[] _memePhrases = new[] { "!", "говорит:" };
        static string[] _userIds = new[]
        {
            "212515973",//vitya
            //"1556462"//me
        };

        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return _messageTypes.Contains(message.MessageType.ToLowerInvariant())
                && !string.IsNullOrWhiteSpace(message.text)
                && (_userIds.Contains(message.from_id) || checkCommand(message.text));
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {         
            var text = tryParseCommand(message);

            await processMemeAsync(message, bot, text);
            return new HandlerResult() { message = "ok" };
        }
        //todo: command handler
        private bool checkCommand(string text)
        {
            return _memePhrases.Any(t => text.ToLowerInvariant().StartsWith(t) && text.Length > t.Length);
        }

        private string tryParseCommand(IIncomingMessage message)
        {
            //todo: regexp or any command parser
            if (_userIds.Contains(message.from_id))
            {
                return message.text;
            }
            foreach (var memeCommand in _memePhrases.Where(t => message.text.ToLowerInvariant().StartsWith(t)))
            {
                int index = message.text.ToLowerInvariant().IndexOf(memeCommand);
                if (index != -1)
                {
                    var result = message.text.Substring(index + memeCommand.Length);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                } 
            }
            throw new ArgumentException($"Message text is empty");
        }

        //todo: move to bot
        private async Task processMemeAsync(IIncomingMessage message, IVityaBot bot, string text)
        {
            var memeUrl = await bot.imgFlipCaptionImage(text);
            var photoId = await bot.savePhotoByUrl(memeUrl, message.peer_id);
            var outgoingMessage = new OutgoingMessage()
            {
                peer_id = message.peer_id,
                //message = text,
                attachment = photoId,
                //group_id = message.
            };
            await bot.SendMessageAsync(outgoingMessage);
        }
    }
}
