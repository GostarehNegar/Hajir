using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using GostarehNegarBot.Contracts;

namespace GostarehNegarBot.Internals.Pipelines
{
    class VoiceMessagePipe
    {
        public static async Task Download(TelegramBotContext context)
        {
            context.Logger.LogDebug("Trying to Download Voice...");
            var file = await context.Bot.GetFileAsync(context.Update.Message.Voice.FileId, context.CancellationToken);
            using (var stream = new MemoryStream())
            {
                await context.Bot.DownloadFileAsync(file.FilePath, stream);
                context.Voice = new TelegramBotContext.VoiceContext
                {
                    Content = stream.ToArray(),
                    FileName = Path.GetFileName(file.FilePath)
                };
            }


        }
        public static async Task Recognize(TelegramBotContext context)
        {
            context.Logger.LogDebug("Trying Recognize Voice using API");
            using (var client = new HttpClient())
            {
                var dic = new Dictionary<string, string>();
                dic.Add("format", "oga");
                dic.Add("data", System.Convert.ToBase64String(context.Voice.Content));
                var req = Utils.Serialize(dic);
                //var t = new HttpRequestMessage();
                //t.Method= 
                var response = await client.PostAsync("http://172.16.6.78:8000/rec", new StringContent(req, Encoding.UTF8, "application/json"));
                var text = await response.Content.ReadAsStringAsync();
                var result = Utils.Deserialize<Dictionary<string, string>>(text);
                if ((result.TryGetValue("result", out var _result)))
                {
                    context.Voice.ResultText = _result;
                }
            }
        }
        public static async Task TTS(TelegramBotContext context)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync("http://127.0.0.1:8001/tts",
                        new StringContent(
                            Newtonsoft.Json.JsonConvert.SerializeObject(new TTSRequestModel { text = context.Reply }),
                            encoding: System.Text.Encoding.UTF8,
                            "application/json"));
                    response.EnsureSuccessStatusCode();
                    var result = Utils.Deserialize<TTSResponseModel>(await response.Content.ReadAsStringAsync());
                    context.TTS = new TelegramBotContext.TTSContext
                    {
                        Content = System.Convert.FromBase64String(result.content)
                    };
                }
                catch (Exception err)
                {

                }


            }




            //await Bot.SendVoiceAsync(msgText.Chat.Id, new Telegram.Bot.Types.InputFileStream);
        }
        public static async Task SendVoice(TelegramBotContext context)
        {
            var stream = context.TTS.GetStream();
            var file = new Telegram.Bot.Types.InputFileStream(stream, $"{Guid.NewGuid()}.oga");
            await context.Bot.SendVoiceAsync(context.Update.Message.Chat.Id, file);

        }
        public static WithPipe<TelegramBotContext> Setup()
        {

            return WithPipe<TelegramBotContext>.Setup()
            .Then(Download)
            .Then(Recognize)
            .Then(ReplyPipe.MakeReply)
            .Then(TTS)
            .Then(SendVoice);


        }
    }
}
