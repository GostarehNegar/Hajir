using GostarehNegarBot;
using GostarehNegarBot.Internals;
using GostarehNegarBot.Lib;
using Hajir.AI.Bot.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Microsoft.Extensions.DependencyInjection
{
    
    public static class GostarehNegarBotExtensions
    {
        public static IServiceCollection AddHajirAIBot(this IServiceCollection services, IConfiguration configurations, Action<GostarehNegarBotOptions> configure)
        {
            
            
            _ = "ll";
            
            var options = new GostarehNegarBotOptions().Validate();
            configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddMemoryCache();
            services.AddSingleton<ChatMemoryCache>();
            services.AddSingleton<Bus>();
            services.AddSingleton<IBus>(s => s.GetService<Bus>());
            services.AddSingleton<GostarehNegarUpdateHandler>();
            services.AddSingleton<TelegramBotService>();
            services.AddHostedService(sp => sp.GetService<TelegramBotService>());
            services.AddSingleton<LiteDbContactStore>();
            //services.AddSingleton<IContactStore>(sp => sp.GetService<LiteDbContactStore>());
            services.AddSingleton<OdooContactStore>(); 
            services.AddSingleton<IContactStore>(sp => sp.GetService<OdooContactStore>());
            //services.AddSingleton<OdooService>();
            //services.AddHostedService(sp => sp.GetService<OdooService>());
            return services;
        }
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout, CancellationToken token = default, bool Throw = true)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout, token)))
                return await task.ConfigureAwait(false);
            else if (Throw)
                throw new TimeoutException();
            return default(TResult);
        }
    }
}
