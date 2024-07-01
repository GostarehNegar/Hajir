using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GostarehNegarBot.Internals
{
    public class TelegramBotContext : IDisposable
    {
        private readonly IServiceScope scope;
        private readonly ITelegramBotClient bot;

        public class TTSContext
        {
            public byte[] Content { get; set; }

            public Stream GetStream()
            {
                var mem = new MemoryStream(this.Content);
                mem.Seek(0, SeekOrigin.Begin);
                return mem;
            }
        }
        public class VoiceContext
        {
            public string FileName { get; set; }
            public byte[] Content { get; set; }
            public string ResultText { get; set; }
        }
        
        public Update Update { get; private set; }
        public CancellationToken CancellationToken { get; private set; } = default;
        public IServiceProvider ServiceProvider => scope.ServiceProvider;
        public ITelegramBotClient Bot => this.bot;
        public ILogger Logger { get; private set; }
        public ConcurrentDictionary<string, object> PropertyBag = new ConcurrentDictionary<string, object>();

        public VoiceContext Voice = new VoiceContext();
        public TTSContext TTS = new TTSContext();

        public string Question => !string.IsNullOrWhiteSpace(Voice?.ResultText) ? Voice.ResultText : Update?.Message.Text;

        public TelegramBotContext(IServiceScope scope, Update update, ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            this.scope = scope;
            this.Update = update;
            this.bot = bot;
            this.CancellationToken = cancellationToken;
            this.Logger = this.ServiceProvider.GetService<ILoggerFactory>().CreateLogger($"Chat ({update.Id})");
        }
        public string Reply { get; set; }

        public void Dispose()
        {
            this.scope.Dispose();
        }
    }
}
