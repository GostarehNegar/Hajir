using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Dynamic.Communication.Messaging.SMS.Providers.MelliPayamak
{
    class MelliPayamakSMSProvider
    {
        private readonly ILogger logger;

        public string Url { get { return "http://api.payamak-panel.com/post/Send.asmx?wsdl"; } }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LineNumber { get; set; }

        public MelliPayamakSMSProvider(ILogger<MelliPayamakSMSProvider> logger )
        {
            this.logger = logger;
            LineNumber = "90004624";
            //c.LineNumber = "xxxx";
            UserName = "09129410023";
            Password = "74625f22-7b49-4e22-8d60-ae353c40c3e0";
        }
        public void Configure(SMSConnectionConfiguration confgure)
        {
            this.UserName = confgure.UserName;
            this.Password = confgure.Password;
            this.LineNumber = confgure.LineNumber;
        }






        public async Task<SMSSendResultModel> SendAsync(SMSSendModel messages)
        {
            await Task.CompletedTask;
            SMSSendResultModel resultModel = new SMSSendResultModel();
            Send soapClient = new Send();
            Dictionary<int, string> error_codes = new Dictionary<int, string>()
            {

                {0 , "نام کاربری یا رمز عبور اشتباه می باشد." },
                { 1 ," درخواست با موفقیت انجام شد." },
                {2 ," اعتبار کافی نمی باشد." },
                {3," محدودیت در ارسال روزانه" },
                {4," محدودیت در حجم ارسال" },
                {5 ," شماره فرستنده معتبر نمی باشد." },
                {6 ," سامانه در حال بروزرسانی می باشد." },
                {7," متن حاوی کلمه فیلتر شده می باشد." },
                {9," ارسال از خطوط عمومی از طریق وب سرویس امکان پذیر نمی باشد." },
                {10," کاربر مورد نظر فعال نمی باشد." },
                {11," ارسال نشده" },
                {12," مدارک کاربر کامل نمی باشد." }

            };


            try
            {
                soapClient.Url = this.Url;
                var response = soapClient.SendSimpleSMS2(this.UserName, this.Password, messages.To, this.LineNumber, messages.Message, false);
                if (long.TryParse(response, out var _id) && _id < 100)
                {
                    resultModel.ProviderSendStatusCode = response;
                    resultModel.SendStausCode = SMSSendStatusCodes.ProviderError;
                    if (error_codes.TryGetValue((int)_id, out var _err))
                    {
                        resultModel.ProviderSendStaus = _err;
                    }
                    this.logger.LogError(
                        $"An error occured while trying to send SMS with MeliPayamak. Error:'{_err}'. Code:;{_id}'");
                }
                else
                {
                    resultModel.ProviderTrackingId = response;
                    resultModel.SendStausCode = SMSSendStatusCodes.Successfull;
                    this.logger.LogDebug(
                        $"Successfully Sent SMS through MeliPayamak Provider. Tracking Code:{_id}");
                }
                //resultModel.ProviderSendStatusCode = soapClient.SendSimpleSMS2(this.UserName, this.Password, messages.To, this.LineNumber, messages.Message, false);
                /*یک عدد: recId ارسال
    ۰ : نام کاربری یا رمز عبور اشتباه می باشد.
    ۱ : درخواست با موفقیت انجام شد.
    ۲ : اعتبار کافی نمی باشد.
    ۳ : محدودیت در ارسال روزانه
    ۴ : محدودیت در حجم ارسال
    ۵ : شماره فرستنده معتبر نمی باشد.
    ۶ : سامانه در حال بروزرسانی می باشد.
    ۷ : متن حاوی کلمه فیلتر شده می باشد.
    ۹ : ارسال از خطوط عمومی از طریق وب سرویس امکان پذیر نمی باشد.
    ۱۰ : کاربر مورد نظر فعال نمی باشد.
    ۱۱ : ارسال نشده
    ۱۲ : مدارک کاربر کامل نمی باشد.*/
            }
            catch (Exception err)
            {
                this.logger.LogWarning(
                    $"An error occured while trying to send SMS thru MeliPayamak: {err.GetBaseException().Message}");
                resultModel.SendStausCode = SMSSendStatusCodes.ConnectionError;

            }
            return resultModel;
        }
    }

    public enum SMSSendStatusCodes
    {
        Unexpected,
        Successfull,
        ConnectionError,
        ProviderError
    }
    public class SMSSendModel
    {
        public string Id { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
    }
    public class SMSSendResultModel
    {
        public string ProviderTrackingId { get; set; }
        public SMSSendStatusCodes SendStausCode { get; set; }
        public SMSSendModel SMSSent { get; set; }
        public string ProviderSendStatusCode { get; set; }
        public string ProviderSendStaus { get; set; }
        public string Message { get; set; }

        public TimeSpan? PostponeNextAttempt { get; set; }
    }
    public class SMSConnectionConfiguration
    {
        public Dictionary<string, string> Attributes { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LineNumber { get; set; }
    }
    public class SMSQueryDeliverStatusResultModel
    {
        public SMSDeliveryStatusCode DeliveryStatus { get; set; }
        public string ProviderDeliveryStatusCode { get; set; }
        public string ProvideDeliveryStatus { get; set; }
    }
    public enum SMSDeliveryStatusCode
    {
        Successfull,
        UnSuccessfull,
        Pending,
        Blocked
    }
}
