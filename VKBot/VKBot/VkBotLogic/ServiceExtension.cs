using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VKBot.VkBotLogic.Models;
using System.Collections.Generic;

namespace VKBot.VkBotLogic
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddVKBot(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<VKBotOptions>(configuration.GetSection("VKBotOptions"));
            services.AddSingleton<IVityaBot, VkBot>();

            services.AddSingleton<List<IUpdatesHandler<IIncomingMessage>>>((p) => new List<IUpdatesHandler<IIncomingMessage>>()
            {
                //new AudioMessageHandler(),
                //new CommandMessageHandler(),
                new PhotoMessageHandler(),
                new TextMessageHandler(),
                new VoiceMessageHandler(),
                new WallMessageHandler(),
            });

            services.AddSingleton<List<IUpdatesResultHandler<IIncomingMessage>>>((p) => new List<IUpdatesResultHandler<IIncomingMessage>>()
            {
                new ConfirmationHandler(),
            });
            return services;
        }
    }
}