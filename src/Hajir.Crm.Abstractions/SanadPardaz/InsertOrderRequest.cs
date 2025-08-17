using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.SanadPardaz
{
    public class InsertOrderRequest
    {
        public class OrderRow
        {
            /// itemRow integer شماره ردیف کالا در سفارش
            public int itemRow { get; set; }
            //goodCode    string کد کالا
            public string goodCode { get; set; }

            //amount  number مقدار یا تعداد کالا
            public decimal amount { get; set; }
            //itemNumber1 number مدت گارانتی(مقداردهی با تعداد ماه به صورت عدد صحیح)
            public int itemNumber1 { get; set; }

            //fee number  نرخ کالا(ریال)
            public decimal fee { get; set; }
            //rowDesc string توضیحات ردیف(از این مورد در سند پرداز هژیر صنعت، برای تغییر نام نمایشی کالا در فاکتور استفاده میشود)
            public string rowDesc { get; set; }
            //itemDesc4 string ظرفیت باتری(در اصل از این فیلد برای توضیحات درون سازمانی مربوط به ارسال سفارش استفاده میشود، از قبیل no name بودن – تعداد باتری و آمپر باتری ها و...)
            public string itemDesc4 { get; set; }

            //batchNo integer در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
            public int batchNo { get; set; }
            //
            //batchNo2 integer در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
            public int batchNo2 { get; set; }
            public int batchNo3 { get; set; }
            public int batchNo4 { get; set; }
            public int batchNo5 { get; set; }
            public int batchNo6 { get; set; }
            //vat1Price number  مبلغ ارزش افزوده کالا(ریال) (در صورتی که سفارش از نوع غیر رسمی باشد 0 مقدار دهی میشود)
            public decimal vat1Price { get; set; }
            //itemDetailCode1 integer در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
            public int itemDetailCode1 { get; set; }
            //itemDetailCode2 integer در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
            public int itemDetailCode2 { get; set; }
            //discountPer number  در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
            public decimal discountPer { get; set; }
            //discountPrice number  در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
            public decimal discountPrice { get; set; }


        }
        public class AddReds
        {
            //addRedTypeId integer آیدی اضافات کسورات تعریف شده در سند پرداز برای مجموعه هژیر صنعت: این شناسه برای شرکت هژیر صنعت در سند پرداز 256 می باشد، پس در اینجا همیشه مقدار 256 مقدار دهی میشود.
            public int addRedTypeId { get; set; }

            //rowId integer باید همیشه مقدار 1 به آن داده شود
            public int rowId { get; set; }
            //per number  باید همیشه مقدار 0 به آن داده شود
            public decimal per { get; set; }
            //price number  جمع مبالغ ارزش افزوده سفارش(ریال)  (در صورتی که سفارش از نوع غیر رسمی باشد 0 مقدار دهی میشود)
            public decimal price { get; set; }

            //addOrRed integer باید همیشه مقدار 1 به آن داده شود
            public int addOrRed { get; set; } = 1;

        }

        public int typeNo { get; set; } = 100;
        public string financYear { get; set; } = "1403";
        public int rdBaseNo { get; set; } = 0;
        public int companyId { get; set; } = 1;
        public string strDate { get; set; }
        /// <summary>
        /// کد تفصیل (کد شرکت یا مشتری در سند پرداز)
        /// </summary>
        public int detailCode1 { get; set; }
        /// <summary>
        /// کد انبار خروج: با توجه به محل خروج سفارش، با یکی از کدهای زیر  مقدار دهی میشود:
        /// انبار محصول(کارخانه): 994000
        ///انبار خدمات پس از فروش: 994001
        ///انبار دفتر مرکزی: 994002

        /// </summary>
        public int detailCode2 { get; set; }
        /// <summary>
        /// در حال حاضر کاربردی ندارد و باید همیشه مقدار 0 به آن داده شود
        /// </summary>
        public int detailCode3 { get; set; }
        /// <summary>
        /// کد تفصیل (شخص) اس ام اس: برای ارسال پیامک های سند پرداز مورد استفاده قرار میگیرد.
        /// </summary>
        public int detailCode4 { get; set; }
        /// <summary>
        /// کد نوع فروش (رسمی/غیر رسمی)

        ///        کد های نوع فروش در سند پرداز:
        /// غیر انتقالی: 1
        /// انتقالی: 2

        /// </summary>
        public int detailCode5 { get; set; }
        /// <summary>
        /// کد کاربر ثبت کننده مبنا در سند پرداز (کد کارشناس فروش در سند پرداز)
        /// (در سند پرداز برای هر یوزر دو کد ثبت میشود، یکی بعنوان کد پرسنلی و یکی بعنوان کد کاربری، در اینجا کد کاربری مد نظر است)
        /// </summary>
        public int userCode { get; set; }
        /// <summary>
        /// کد سیستم ثبت کننده (مربوط به سند پرداز)  شناسه سیستم فروش داخلی در سند پرداز 1 می باشد، پس در اینجا برای واحد فروش همیشه مقدار 1 مقدار دهی میشود.
        /// </summary>
        public int sysCode { get; set; }
        /// <summary>
        /// مقدار جمع مبلغ PL (جمع مبلغ خالص کل سفارش قبل از تخفیف و ارزش افزوده، این مورد برای محاسبات پورسانت کارشناس فروش مورد استفاده قرار میگیرد)
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// تاریخ ارسال
        /// </summary>
        public string strCreditDate { get; set; }
        /// <summary>
        /// آدرس تحویل
        /// </summary>
        public string desc1 { get; set; }
        /// <summary>
        /// مهلت پرداخت (مقداردهی با تعداد روز به صورت عدد صحیح در قالب رشته)
        /// </summary>
        public string desc2 { get; set; }

        /// <summary>
        /// شماره تلفن/موبایل تحویل
        /// </summary>
        public string desc3 { get; set; }
        /// <summary>
        /// نام شهر تحویل
        /// </summary>
        public string desc4 { get; set; }

        public OrderRow[] items { get; set; }

        public AddReds[] addReds { get; set; }

        public InsertOrderRequest WithCustomerDetailCode(int detailCode)
        {
            this.detailCode1 = detailCode;
            return this;
        }


    }

    public class InsertOrderResponse
    {
        public int idGrdb { get; set; }
    }
}
