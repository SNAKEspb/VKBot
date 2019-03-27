using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class VoiceMessageHandler : IUpdatesHandler<IIncomingMessage>
    {
        //todo:move statics to bot
        static string[] _userIds = new[]
        {
            "212515973",//vitya
        };
        static string _symbols = "АБВЛ ";

        public bool CanHandle(IIncomingMessage message, IVityaBot bot)
        {
            return (_userIds.Contains(message.from_id) || bot.isTest) && message.attachments.Any(x => x.type == "audio_message");
        }
        public async Task<HandlerResult> HandleAsync(IIncomingMessage message, IVityaBot bot)
        {
            foreach (var attach in message.attachments.Where(x => x.type == "audio_message"))
            {
                //у гугла бесплатный лимит на 60 минту в месяц, если больше то надо бабки платить
                //я там поставил на 120 секунд в день квоту, что бы за пределы не вылезало
                //у меня вроде бесплатный режим стоит в аккаунте, но я хз как у них там сразу бабки снимает
                if (attach.audio_message.duration < 10)
                {
                    var audioText = await bot.audioToText((string)attach.audio_message.link_mp3);
                    if (!string.IsNullOrWhiteSpace(audioText))
                    {
                        await bot.processMemeAsync(message, audioText);
                    }
                    else
                    {
                        await bot.processMemeAsync(message, generateRandom());
                    }
                }
                else
                {
                    await bot.processMemeAsync(message, generateRandom());
                }
            }

            return new HandlerResult() { message = "ok" };
        }
        //todo: move to bot service
        private static string generateRandom()
        {
            var length = VkBot._random.Next(8, 15);
            return new string(Enumerable.Repeat(_symbols, length).Select(s => s[VkBot._random.Next(s.Length)]).ToArray());
        }
    }
}
