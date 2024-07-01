using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using GostarehNegarBot.Internals.Pipelines;
using GostarehNegarBot.Internals;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;
using Hajir.AI.Bot.Internals;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Net.Http;

namespace GostarehNegarBot
{
    internal class GostarehNegarUpdateHandler : IUpdateHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<GostarehNegarUpdateHandler> logger;
        private readonly ChatMemoryCache cache;
        private readonly IContactStore contactStore;

        public GostarehNegarUpdateHandler(IServiceProvider serviceProvider,
            ILogger<GostarehNegarUpdateHandler> logger, ChatMemoryCache cache, IContactStore contactStore)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.cache = cache;
            this.contactStore = contactStore;
        }
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            if (exception.InnerException is HttpRequestException http)
            {
                return Task.CompletedTask;
            }
            var ErrorMessage = exception is ApiRequestException apiRequestException
                ? $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}"
                : exception.Message;
            this.logger.LogError(ErrorMessage);
            return Task.CompletedTask;
        }
        //public ContactData GetContactFromCache(long userId, Action<ContactData> configure)
        //{
        //    return this.cache.GetOrCreate<ContactData>(userId, entry => {
        //        entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));
        //        var contact = this.contactStore.GetByTelegramUserId(userId);
        //        if (contact == null)
        //        {
        //            contact = new ContactData
        //            {
        //                TelegramId = userId,

        //            };
        //            configure?.Invoke(contact);
        //            this.contactStore.Create(contact);
        //        };
        //        return contact;

        //    });
        //    return null;
        //}
        public async Task HandleVoiceUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var task = Task<bool>.Run(async () =>
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    try
                    {
                        var repl = await VoiceMessagePipe.Setup().Run(new TelegramBotContext(scope, update, botClient, cancellationToken)).TimeoutAfter(120000, token: cancellationToken);
                        //repl = await ReplyPipe.Setup().Run(new TelegramBotContext(scope, update, botClient, cancellationToken)).TimeoutAfter(120000, token: cancellationToken);
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, text: repl.Reply, cancellationToken: cancellationToken);
                    }
                    catch (Exception err)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, text: $"Err: {err.GetBaseException().Message}", cancellationToken: cancellationToken);
                    }
                }
                return true;
            });

            await Task.CompletedTask;
        }
        private async Task<bool> EnsureSignedIn(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var user = this.cache.GetContactFromCache(update.Message.From.Id,
            c =>
            {
                c.FirstName = update.Message?.From?.FirstName;
                c.LastName = update.Message?.From?.LastName;
                c.ChatId = update.Message.Chat?.Id ?? 0;
                c.TelegramId = update.Message.From.Id;


            });
            if (string.IsNullOrEmpty(user?.Mobile))
            {
                KeyboardButton button = KeyboardButton.WithRequestContact("ثبت نام.");  // Right here, the string defines what text appears on the button
                ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(button);
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "لازم است ابتدا ثبت نام کنید. برای اینکار روی ثبت نام کلیک کنید. ", replyMarkup: keyboard);
                return false;
            }
            return true;

        }
        public async Task HandleTextUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var task = Task<bool>.Run(async () =>
            {

                using (var scope = this.serviceProvider.CreateScope())
                {
                    try
                    {
                        var ReplyMarkup = new ReplyKeyboardRemove();
                        //ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(new KeyboardButton(""));
                        var repl = await ReplyPipe.Setup().Run(new TelegramBotContext(scope, update, botClient, cancellationToken)).TimeoutAfter(120000, token: cancellationToken);
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, text: repl.Reply, replyMarkup: ReplyMarkup, cancellationToken: cancellationToken);
                        this.logger.LogInformation(
                            $"Text update successfully handled. User:{update.Message.From.LastName}");
                    }
                    catch (Exception err)
                    {
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, text: $"Err: {err.GetBaseException().Message}", cancellationToken: cancellationToken);
                    }
                }
                return true;
            });
            // await task;
        }
        public async Task HandleContactUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            var contact = update?.Message?.Contact;
            if (contact != null && contact.UserId.HasValue)
            {
                this.cache.UpdateContactInCache(contact.UserId.Value, c =>
                {
                    c.Mobile = contact.PhoneNumber;
                    c.FirstName = contact.FirstName;
                    c.LastName = contact.LastName;

                });
            }
            var ReplyMarkup = new ReplyKeyboardRemove();
            var text = "ممنون حالا میتوانید پرسش خود را مطرح کنید..";

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, text: text, replyMarkup: ReplyMarkup, cancellationToken: cancellationToken);

        }
        public async Task EnsurePhoto(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                await (this.contactStore as OdooContactStore).UpdatePhoto(update.Message.From.Id, async () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        var photo = await botClient.GetUserProfilePhotosAsync(update.Message.From.Id);
                        if (photo.TotalCount > 0)
                        {
                            var f = photo.Photos[0][0];
                            var file = await botClient.GetFileAsync(f.FileId, cancellationToken);
                            await botClient.DownloadFileAsync(file.FilePath, stream);

                        }
                        this.logger.LogInformation(
                            $"Photo successsfuly downloaded. User:{update.Message.From.LastName}");
                        return stream.ToArray();
                    }


                });
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to download photo. Err:{err.GetBaseException().Message}");
            }
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Message == null)
                return;
            try
            {
                this.cache.GetContactFromCache(update.Message.From.Id,
                       c =>
                       {
                           c.FirstName = update.Message?.From?.FirstName;
                           c.LastName = update.Message?.From?.LastName;
                           c.ChatId = update.Message.Chat?.Id ?? 0;
                           c.TelegramId = update.Message.From.Id;


                       });
                //await this.EnsurePhoto(botClient, update, cancellationToken);
                switch (update.Message.Type)
                {
                    case MessageType.Text:
                        if (await this.EnsureSignedIn(botClient, update, cancellationToken))
                        {
                            await this.HandleTextUpdateAsync(botClient, update, cancellationToken);
                        }
                        break;
                    //case MessageType.Voice:
                    //    if (signedIn)
                    //    {
                    //        await this.HandleVoiceUpdateAsync(botClient, update, cancellationToken);
                    //    }
                    //    break;
                    case MessageType.Contact:
                        await this.HandleContactUpdate(botClient, update, cancellationToken);
                        break;
                    default:
                        break;
                }
            }

            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to handle update.{err.GetBaseException().Message}");
            }



        }
    }
}
