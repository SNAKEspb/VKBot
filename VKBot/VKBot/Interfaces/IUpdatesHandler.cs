using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKBot.VkontakteBot.Models;

namespace VKBot
{
    public interface IUpdatesHandler<T>
    {
        bool CanHandle(T message, IVityaBot bot);
        Task<HandlerResult> HandleAsync(T message, IVityaBot bot);
    }
}
