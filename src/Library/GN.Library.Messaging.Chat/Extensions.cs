using GN.Library.Messaging.Chat.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using GN.Library.Messaging.Chat.Storage;
using GN.Library.Messaging.Chat.Storage.LiteDb;
using Microsoft.Extensions.Hosting;
using GN.Library.Shared.Chats.UserMessages;

namespace GN.Library.Messaging.Chat
{
    public static class ChatExtensions
    {
        static bool Enabled;
        public static ChatOptions GetChatOptions(this IConfiguration configuration)
        {
            return configuration?
                    .GetSection("messaging")?
                    .GetSection("chat")?
                    .Get<ChatOptions>() ?? new ChatOptions();


        }
        public static IServiceCollection AddChat(this IServiceCollection services, IConfiguration configuration, Action<ChatOptions> configure)
        {
            var options = configuration.GetChatOptions();
            configure?.Invoke(options);
            services.AddSingleton(options.Validate());
            if (options.Enabled)
            {
                Enabled = true;
                services.AddSingleton<ConnectionManager>();
                //services.AddSingleton<PingService>();
                //services.AddTransient<IHostedService>(s => s.GetService<PingService>());

                services.AddSingleton<ChannelServices>();
                services.AddSingleton<IChannelServices>(s => s.GetServiceEx<ChannelServices>());
                services.AddSingleton<IHostedService>(s => s.GetServiceEx<ChannelServices>());
                services.AddSingleton<ChannelLiteDBRepository>();
                services.AddSingleton<IChannelRepository>(s => s.GetServiceEx<ChannelLiteDBRepository>());
                services.AddMessagingServices(Cfg);

                //services.AddScoped<IStore, Store>();
                //services.AddScoped<IEntityManager, EntityManager>();
                ////services.AddScoped<IEntityRepository, LiteDbEntityRepository>();
                ////services.AddTransient<IAuthorizationService, AuthorizationService>();
                //services.AddTransient<ISubscriptionRepository, SubscriptionsRepository>();
                //bus?.ChatConfigureBus();
                //services.AddMediator(x =>
                //{
                //	services.AddMassTransit(c => ConfigureBus(options, services, x, c));
                //});
                if (1==1 || !options.DoNotAddSignalR)
                    services.AddSignalR();
                services.AddCors(cfg =>
                {
                    cfg.AddPolicy("chat_cors_policy", p =>
                    {
                        p.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
                });
            }
            return services;
        }
        public static void UseChatService(this IApplicationBuilder app)
        {
            if (Enabled)
            {
                app.UseCors("chat_cors_policy");
#if !NETCOREAPP3_1 && !NET5_0
            app.UseSignalR(cfg =>
            {
                cfg.MapHub<ChatHub>("/chat");
            });

#else
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<ChatHub>("/chat");
                });
#endif
                //return app;
                //}
                //app.UseSignalR(cfg => { 
                //	cfg.MapHub<ChatHub>("/chatHub");
                //});
            }

        }
        internal static void Cfg(this GN.Library.Messaging.Internals.IMessageBusConfigurator config)
        {
            config.Register(s =>
            {
                s.UseTopic(LibraryConstants.Subjects.UserRequests.WhoAmI)
                .UseHandler(async ctx =>
                {
                    var fff = ctx.Message.GetBody<WhoAmIRequest>()?.Tag;
                    await ctx.Reply(new WhoAmIResponse { DisplayName = fff });

                });
            });
        }
        public static void ChatConfigureBus(this IServiceCollectionBusConfigurator bus)
        {

            //bus.AddConsumer<Consumers.RingingConsumer>(cfg => { 

            //});

        }
        private static void ConfigureBus(ChatOptions options, IServiceCollection services, IServiceCollectionMediatorConfigurator mediator, IServiceCollectionBusConfigurator bus)
        {




        }


    }
}
