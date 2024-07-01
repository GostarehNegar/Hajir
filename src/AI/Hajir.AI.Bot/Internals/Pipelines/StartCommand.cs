using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GostarehNegarBot.Internals.Pipelines
{
    public class StartCommand
    {
        public static async Task Step1( TelegramBotContext context)
        {

        }

        public static WithPipe<TelegramBotContext> Setup()
        {
            return WithPipe<TelegramBotContext>.Setup()
               .Then(StartCommand.Step1);
        }
    }
}
