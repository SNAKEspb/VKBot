using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class CommandMessageHandler : IUpdatesHandler<IIncomingMessage>
    {
        //todo:move statics to bot
        static string[] _messageTypes = new[] { "message_new", "message_reply" };
        static Dictionary<string, Func<IIncomingMessage, IVityaBot, Task>> _commands = new Dictionary<string, Func<IIncomingMessage, IVityaBot, Task>> ()
        {
            {"/status", async  (message, bot ) =>
                {
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"bot test mode is {bot.isTest.ToString()}"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }},
            {"/test", async  (message, bot ) => 
                {
                    bot.isTest = !bot.isTest;
                    var outgoingMessage = new OutgoingMessage()
                    {
                        peer_id = message.peer_id,
                        message = $"bot test mode is {bot.isTest.ToString()}"
                    };
                    await bot.SendMessageAsync(outgoingMessage);
                }},
             {"/history", async  (message, bot ) =>
                {
                    bot.getChatHistory(message);
                    //await bot.SendMessageAsync(outgoingMessage);
                }},
        };

        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return !string.IsNullOrWhiteSpace(message.text)
                && _commands.ContainsKey(message.text.ToLowerInvariant());
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            await _commands[message.text.ToLowerInvariant()](message, bot);
            return new HandlerResult() { message = "ok" };
        }
    }
}
