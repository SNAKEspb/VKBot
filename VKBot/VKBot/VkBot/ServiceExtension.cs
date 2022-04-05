using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StirkaBot.VKBot.Models;
using System.Collections.Generic;

namespace StirkaBot.VKBot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddVKBot(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Models.VKBotOptions>(configuration.GetSection("VKBotOptions"));
            //services.AddSingleton<IVKBot, VKBot>();
            //services.AddSingleton<StirkaBot.Models.Flow>(new StirkaBot.Services.FlowService(null).initFlow());

            //services.AddSingleton<List<IUpdatesHandler<IIncomingMessage>>>((p) => new List<IUpdatesHandler<IIncomingMessage>>()
            //{
            //    new TextMessageHandler(p.GetService<StirkaBot.Models.Flow>()),
            //    new MenuMessageHandler(p.GetService<StirkaBot.Models.Flow>()),
            //});

            //services.AddSingleton<List<IUpdatesResultHandler<IIncomingMessage>>>((p) => new List<IUpdatesResultHandler<IIncomingMessage>>()
            //{
            //    new ConfirmationHandler(),
            //});
            return services;
        }
    }
}