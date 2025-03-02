using GN.Library;
using Hajir.Crm.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return Path.GetFullPath(Path.Combine("Products", DatasheetFileName));
        }
        public static string GetDatasheetPropSpecFullPath()
        {
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
        
        public class Schema : LibraryConstants.Schema
        {
            public const string HajrSolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
            public new class Account : LibraryConstants.Schema.Account
            {
                public const string LL = "";

            }
            public new class Product : LibraryConstants.Schema.Product
            {
                public const int BaseOptionSetIndex = 130770000;
                public const string RHS_SolutionPerfix = "rhs";
                public const string SolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
                //public const string ProductType = RHS_SolutionPerfix + "producttype";
                public const string TypeProducts_deprecated = "rhs_typeproducts";
                public const string ProductSerie = "rhs_productseries";
                public const string SupportedBatteries = SolutionPerfix + "spec_ups_supported_batteries";
                public const string NumberOfFloors = "rhs_numberoffloors";
                public const string BatteryCurrent = SolutionPerfix + "spec_ups_battery_current";
                //public const string ProductCategoryCode = SolutionPerfix + "categorycode";
                public const string SynchedOn = SolutionPerfix + "synchedon";
                public const string ProductTypeCode = SolutionPerfix + "producttypecode";
                public const string CategoryId = SolutionPerfix + "categoryid";

                public static ProductCategories? IntToProductCategory(int? cat)
                {

                    if (cat.HasValue && Enum.IsDefined(typeof(ProductCategories), cat.Value))
                    {
                        return (ProductCategories)cat;
                    }
                    return null;
                }
                public static ProductTypes? GetProductTypeFromProductCategory(int? category)
                {
                    return GetProductTypeFromProductCategory(IntToProductCategory(category));
                }
                public static ProductTypes? GetProductTypeFromProductCategory(ProductCategories? category)
                {
                    if (!category.HasValue)
                        return null;
                    switch (category)
                    {
                        case ProductCategories.UPS_Tolidi:
                        case ProductCategories.UPS_Bazargani:
                            return ProductTypes.UPS;
                        case ProductCategories.Cabinet_Majazi:
                        case ProductCategories.Cabinet_Tolidi:
                            return ProductTypes.Cabinet;
                        case ProductCategories.Inverter_Bazargani:
                        case ProductCategories.Inverter_Tolidi:
                            return ProductTypes.Inverter;
                        case ProductCategories.Battery:
                            return ProductTypes.Battery;
                        case ProductCategories.Battery_Pack:
                            return ProductTypes.Battery_Pack;
                        case ProductCategories.Stabilizer_Bazargani:
                        case ProductCategories.Stabilizer_Tolidi:
                            return ProductTypes.Stabilizer;
                        case ProductCategories.Switch_ATS:
                            return ProductTypes.Switch_ATS;
                        case ProductCategories.Kart_SNMP:
                            return ProductTypes.SMP_Card;
                        default:
                            return ProductTypes.Other;

                    }

                }
                public enum ProductTypes
                {
                    UPS = 1,
                    Stabilizer = 2,
                    Battery = 4,
                    Cabinet = 12,
                    Inverter = 5,
                    Battery_Pack = 3,
                    Switch_ATS = 13,
                    SMP_Card = 6,
                    Genrator = 14,
                    Parallel_Card = 15,
                    Battery_Connector = 16,
                    Other = 10,
                }
                public enum ProductCategories
                {
                    UPS_Tolidi = 401,
                    UPS_Bazargani = 501,
                    Stabilizer_Tolidi = 402,
                    Stabilizer_Bazargani = 502,
                    Inverter_Tolidi = 405,
                    Inverter_Bazargani = 505,
                    Kart_SNMP = 506,
                    Switch_ATS = 513,
                    Battery = 504,
                    Battery_Pack = 403,
                    Cabinet_Tolidi = 412,
                    Cabinet_Majazi = 905
                }
                public enum ProductSeries
                {
                    Homa = BaseOptionSetIndex,
                    Classic_I = BaseOptionSetIndex + 1,
                    Classic_RMI = BaseOptionSetIndex + 2,
                    Classic = BaseOptionSetIndex + 3,
                    Genesis = BaseOptionSetIndex + 4,
                    Genesis_B = BaseOptionSetIndex + 5,
                    Gensis_A = BaseOptionSetIndex + 6,
                    Gensis_RMI = BaseOptionSetIndex + 7,
                    Gensis_RM = BaseOptionSetIndex + 8,
                    Uranus = BaseOptionSetIndex + 9,
                    Eternal = BaseOptionSetIndex + 10,
                    Super_Nova = BaseOptionSetIndex + 11,
                    Spider_Net = BaseOptionSetIndex + 12,
                    Salicru = BaseOptionSetIndex + 13,
                    Euro_Inverter = BaseOptionSetIndex + 14,
                    AVR = BaseOptionSetIndex + 15,
                    STB = BaseOptionSetIndex + 16,
                    STB_3P = BaseOptionSetIndex + 17,
                    SERVO = BaseOptionSetIndex + 18,
                    First_Power = BaseOptionSetIndex + 19,
                    Piltan = BaseOptionSetIndex + 20,
                    MISOL = BaseOptionSetIndex + 21,
                    SABA = BaseOptionSetIndex + 22,
                    HAJIR = BaseOptionSetIndex + 23,
                    PILTAN = BaseOptionSetIndex + 24,
                    UNKOWN = BaseOptionSetIndex + 25,
                }
            }

            public class ProductCategory : LibraryConstants.Schema.EntitySchema
            {
                public const string LogicalName = HajrSolutionPerfix + "productcategory";
                public const string ProductCategoryId = LogicalName + "id";
                public const string Name = HajrSolutionPerfix + "name";
                public const string Code = HajrSolutionPerfix + "code";
                public const string ProductTypeCode = HajrSolutionPerfix + "producttypecode";
            }
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
