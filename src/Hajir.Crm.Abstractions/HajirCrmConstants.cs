using GN.Library;
using Hajir.Crm.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hajir.Crm
{
    public partial class HajirCrmConstants
    {
        public static int[,] CabinetRowCapacityTable = {
            {07,16,12 },
            {09,16,12 },
            {12,10,08 },
            {18,12,10 },
            {28,06,04 },
            {40,06,04 },
            {65,03,02 },
            {100,03,02}
        };
        public const string HajirSolutionPerfix = "hajir_";
        public const string RahsamSolutionPerfix = "rhs_";
        public const int RahsamSolutionIndex = 130770000;
        public const string DefaultLegacyCrmConnectionString = "Url=http://192.168.20.15:5555/hajircrm;UserName=CRMIMPU01;Password=%H@ZH!r_1402&$;Domain=hsco";
        public const string DatasheetFileName = "Datasheet.csv";
        public const string DatasheetPropSpecFileName = "DatasheetProps.json";
        public static string GetDatasheetFullPathFileName()
        {
            var asm = Path.GetDirectoryName(typeof(HajirCrmConstants).Assembly.Location);
            return Path.GetFullPath(
                Path.Combine(Path.Combine(asm, "Products"), DatasheetFileName));
        }
        public static string GetDatasheetPropSpecFullPath()
        {
            var asm = Path.GetDirectoryName(typeof(HajirCrmConstants).Assembly.Location);
            return Path.GetFullPath(
                Path.Combine(Path.Combine(asm, "Products"), DatasheetPropSpecFileName));
            return Path.GetFullPath(Path.Combine("Products", DatasheetPropSpecFileName));
        }

        public class Reporting
        {
            public class ReportNames
            {
                public const string QuoteStandardReport = "QuoteStandardReport.frx";
            }
        }
        public class QuoteComments
        {
            public const string STR_Gaurantee = "دستگاه ----- دارای -- گارانتی و 10 سال خدمات پس از فروش میباشد.";
            public const string STR_BATTERY_GAURANTEE = "باتری --- دارای -- ماه گارانتی میباشد.";
            public const string STR_BATTERY_GAURANTEE_SABA = "گارانتی و مسولیت کیفی باطری‌های برند صبا بر عهده شرکت سازنده (صبا باتری) میباشد.";
            public const string STR_FREE_CABLES = "با خرید همزمان باتری و دستگاه یو پی اس از شرکت هژیر صنعت کابل باتری رایگان تقدیم خریدار محترم خواهد شد.";
            public const string STR_SUPPORT_CONTRACT = "در صورت عقد قرار داد نگهداری برای سال دوم تا حداکثر یک ماه از زمان خرید مشتریان محترم از امتیاز بازدید ادواری رایگان در طول گارانتی (سال اول) نیز بهره‌مند میشوند";
            public const string STR_DELIVERY = "کالا آماده تحویل یوده و مدت ارسال 1 الی 2 روز کاری پس از تایید میباشد.";
            public const string STR_DELIVERY_POINT = "تحویل درب کارخانه هزیر صنعت (شهرک صنعتی عباس آباد) می‌باشد، حمل به عهده خریدار است.";
            public const string STR1 = "تحویل فوری می باشد";
            public const string STR2 = "کالا آماده تحویل بوده و مدت ارسال ــــ روز کاری پس از تایید می باشد. ";
            public const string STR3 = "کالا آماده تحویل بوده و مدت ارسال ــــ روز کاری پس از تسویه حساب می باشد.";
            public const string STR4 = "مدت ارسال ــــ روز کاری پس از دریافت ــــ درصد پیش پرداخت می باشد.";
            public const string STR5 = "حمل رایگان است. ";
            public const string STR6 = "نصب در مناطق 22 گانه تهران و شهرک صنعتی عباس آباد رایگان است.";
            public const string STR7 = "نصب رایگان است. ";
            public const string STR8 = "مدت اعتبار پیش فاکتور ــــ روز می باشد. ";
            public const string STR9 = "9% مالیات ارزش افزوده به مبالغ فوق اضافه می شود.";
            public const string STR10 = "اطلاعات حساب جهت پرداخت: شماره حساب 350685888 شماره کارت 6104337841353954 شماره شبا IR030120000000000350685888 بانک ملت شعبه نارمک شمالی به نام شرکت هژیر صنعت. لطفا پس از واریز، رسید خود را ارسال کنید.";


        }
        
        

        public static string MAP1 = @"
شرکت	شرکت
-	شرکت
فروشگاه	فروشگاه
موسسه	موسسه
آموزشگاه	آموزشگاه
سازمان	سازمان
اداره کل	اداره کل
اداره	اداره
کارخانه	کارخانه
وزارتخانه	وزارتخانه
";

    }
}
