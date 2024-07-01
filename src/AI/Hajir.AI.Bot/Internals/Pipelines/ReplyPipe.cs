using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GostarehNegarBot.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using GostarehNegarBot.Models;
using GostarehNegarBot.Lib;
using System.Net.Http;

namespace GostarehNegarBot.Internals
{
   
    public class ReplyPipe
    {
        public static async Task MakeReply_Dep(TelegramBotContext context)
        {
            context.Reply = "hey";
            var _mem = context.ServiceProvider.GetService<ChatMemoryCache>().Cache.Get<MemoryModel>(context.Update.Message.Chat.Id);

            var ret = await context.ServiceProvider.GetService<Bus>().Request("makereply", new MakeReplyContract {
                Input = context.Question,
                Memory = _mem,
                ChatId= context.Update.Message.Chat.Id.ToString()

            });
                
            var dic = ret.GetDataAsDicionary();
            if (dic.TryGetValue("status", out var _status) && int.TryParse(_status.ToString(), out var __status) && __status!=0)
            {
                dic.TryGetValue("error", out var _err);
                throw new Exception($"AI Failure. Error:{_err}");
            }
            if (dic.TryGetValue("answer", out var ans))
            {
                context.Reply = ans.ToString();
            }
            if (dic.TryGetValue("memory", out var memory))
            {
                var mem = Utils.Deserialize<MemoryModel>(memory.ToString());
                context.ServiceProvider.GetService<ChatMemoryCache>().Cache.Set(context.Update.Message.Chat.Id, mem,TimeSpan.FromMinutes(10));
            }

        }

        public static async Task MakeReply(TelegramBotContext context)
        {
            context.Reply = "hey";

            var request = new BrainRequestModel()
            {
                Input = context.Question
            };
            request.Conversation = context.ServiceProvider.GetService<ChatMemoryCache>().Cache.Get<ConversationModel>(context.Update.Message.Chat.Id)
                ?? new ConversationModel();
            //var response= await context.ServiceProvider.GetService<Bus>().Request("makereply", request);
            //var reply = response.GetPayload<BrainReplyModel>();
            var reply = new BrainReplyModel
            {
                Status = 1,
                Error = "Not Found"
            };
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://127.0.0.1:8005/makereply",
                   new StringContent(
                       Newtonsoft.Json.JsonConvert.SerializeObject(request),
                       encoding: System.Text.Encoding.UTF8,
                       "application/json"));
                reply = Newtonsoft.Json.JsonConvert.DeserializeObject<BrainReplyModel>(await response.Content.ReadAsStringAsync());
            }
            if (reply.Status != 0)
            {
                var error = string.IsNullOrWhiteSpace(reply.Error)
                    ? "An error occured. Please try later."
                    : reply.Error;
                throw new Exception(error);
            }
            context.Reply = reply.Output;
            context.ServiceProvider.GetService<ChatMemoryCache>().Cache.Set(context.Update.Message.Chat.Id, reply.Conversation, TimeSpan.FromMinutes(10));
            //var dic = ret.GetDataAsDicionary();
            //if (dic.TryGetValue("status", out var _status) && int.TryParse(_status.ToString(), out var __status) && __status != 0)
            //{
            //    dic.TryGetValue("error", out var _err);
            //    throw new Exception($"AI Failure. Error:{_err}");
            //}
            //if (dic.TryGetValue("answer", out var ans))
            //{
            //    context.Reply = ans.ToString();
            //}
            //if (dic.TryGetValue("memory", out var memory))
            //{
            //    var mem = Utils.Deserialize<MemoryModel>(memory.ToString());
            //    context.ServiceProvider.GetService<ChatMemoryCache>().Cache.Set(context.Update.Message.Chat.Id, mem, TimeSpan.FromMinutes(10));
            //}

        }
        public static WithPipe<TelegramBotContext> Setup()
        {

            return WithPipe<TelegramBotContext>.Setup()
                .Then(MakeReply);
                

        }
    }
}
