using System;
using System.Collections.Generic;
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
            public const string STR_BATTERY_GAURANTEE = "باطری --- دارای -- ماه گارانتی میباشد.";
            public const string STR_BATTERY_GAURANTEE_SABA = "گارانتی و مسولیت کیفی باطری‌های برند صبا بر عهده شرکت سازنده (صبا باطری) میباشد.";
            public const string STR_FREE_CABLES = "با خرید همزمان باتری و دستگاه یو پی اس از شرکت هژیر صنعت کابل باتری رایگان تقدیم خریدار محترم خواهد شد.";
            public const string STR_SUPPORT_CONTRACT = "در صورت عقد قرار داد نگهداری برای سال دوم تا حداکثر یک ماه از زمان خرید مشتریان محترم از امتیاز بازدید ادواری رایگان در طول گارانتی (سال اول) نیز بهره‌مند میشوند";
            public const string STR_DELIVERY = "کالا آماده تحویل یوده و مدت ارسال 1 الی 2 روز کاری پس از تایید میباشد.";
            public const string STR_DELIVERY_POINT = "تحویل درب کارخانه هزیر صنعت (شهرک صنعتی عباس آباد) می‌باشد، حمل به عهده خریدار ااست.";

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
