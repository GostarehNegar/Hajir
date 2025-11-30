using GN.Dynamic.Communication.Messaging.SMS.Providers.MelliPayamak;
using GN.Library.AI.Agents;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class SendSMSToolsOptions
    {

    }
    internal class SendSMSTool : HajirBaseTool
    {
        private const string Name = "send_sms";
        public SendSMSTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {
            description = "sends short messages (sms) to a mobile phone.",
            parameters = new ToolParameter[]
            {
                new ToolParameter{

                    name="mobilephone",
                    description="mobile phone number to send sms to",
                },
                new ToolParameter
                {
                    name="message",
                    description="text message to send"
                },
            },
            returns = {
                {"type","object" },
                {"description","the status of the opertaion and also the TrackingCod if successfull" }
            }

        };

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            var service = this.serviceProvider.GetService<MelliPayamakSMSProvider>();
            var mobile = context.GetParameterValue<string>(this.MetaData.parameters[0].name);
            var message = context.GetParameterValue<string>(this.MetaData.parameters[1].name);
            var result = (1 == 0)
                ? new SMSSendResultModel
                {
                    SendStausCode = SMSSendStatusCodes.Successfull,
                    ProviderTrackingId = "ppp"
                }
                : await service.SendAsync(new SMSSendModel { To = mobile, Message = message });

            this.logger.LogInformation($"Message Sent To:{mobile}, Text:{message}");
            if (result.SendStausCode == SMSSendStatusCodes.Successfull)
                return new
                {
                    Successfull = true,
                    TrackingCode = result.ProviderTrackingId
                };
            return
                 new
                 {
                     Failed = true
                 };

        }
    }
}
