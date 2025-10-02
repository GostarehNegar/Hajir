using GN.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm
{
    public partial class HajirCrmConstants
    {
        public class Schema : LibraryConstants.Schema
        {
            public const string HajrSolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
            public new class Account : LibraryConstants.Schema.Account
            {
                public const string LL = "";
                public const string NationalId = HajirCrmConstants.HajirSolutionPerfix + "nationalid";
                public const string RegistrationNumber = HajirCrmConstants.HajirSolutionPerfix + "registrationnumber";
                public const string EconomicCode = HajirCrmConstants.HajirSolutionPerfix + "economiccode";
                public const string IntroductionMethod = HajirCrmConstants.HajirSolutionPerfix + "introductionmethod";
                public const string RelationTypeCode = HajirCrmConstants.HajirSolutionPerfix + "relationshiptype";
                public const string AccountType = HajirCrmConstants.HajirSolutionPerfix + "accounttype";
                public const string BrandName = HajirCrmConstants.HajirSolutionPerfix + "brandname";
                public const string CityId = HajirCrmConstants.HajirSolutionPerfix + "cityid";
                public const string ProvinceId = HajirCrmConstants.HajirSolutionPerfix + "provinceid";
                public const string CountryId = HajirCrmConstants.HajirSolutionPerfix + "countryid";
                public const string ImportanceCode = HajirCrmConstants.HajirSolutionPerfix + "importancecode";


            }
            public new class Product : LibraryConstants.Schema.Product
            {
                public const int BaseOptionSetIndex = 130770000;
                public const string RHS_SolutionPerfix = "rhs";
                public const string SolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
                //public const string ProductType = RHS_SolutionPerfix + "producttype";
                public const string TypeProducts_deprecated = "rhs_typeproducts";
                public const string ProductSerie = "rhs_productseries";
                public const string SpecSupportedBatteries = SolutionPerfix + "spec_ups_supported_batteries";
                public const string CabinetNumberOfFloors = "spec_cabinet_rows";
                public const string SpecBatteryAmperage = SolutionPerfix + "spec_batterty_amperage";
                //public const string ProductCategoryCode = SolutionPerfix + "categorycode";
                public const string SynchedOn = SolutionPerfix + "synchedon";
                public const string ProductTypeCode = SolutionPerfix + "producttypecode";
                public const string CategoryId = SolutionPerfix + "categoryid";
                public const string JsonProps = SolutionPerfix + "jsonprops";

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
            public class PriceRecord
            {
                public const string LogicalName = HajrSolutionPerfix + "pricerecord";
                public const string PriceRecordId = LogicalName + "id";
                public const string ProductId = HajrSolutionPerfix + "productid";
                public const string Price1 = HajrSolutionPerfix + "price1";
                public const string Price2 = HajrSolutionPerfix + "price2";
                public const string Name = HajrSolutionPerfix + "name";

            }

            public class Datasheet
            {

                /*
                 * [{"Name":"كد كالا","Description":"كد كالا","Label":"A","Type":"string"},{"Name":"نام كالا","Description":"نام كالا","Label":"B","Type":"string"},{"Name":"طبقه خصوصيت سري محصول","Description":"طبقه خصوصيت سري محصول","Label":"C","Type":"string"},{"Name":"توان","Description":"توان","Label":"D","Type":"string"},{"Name":"موقعیت ترمینال","Description":"موقعیت ترمینال","Label":"E","Type":"string"},{"Name":"ترمینال DC یوپی اس","Description":"ترمینال DC یوپی اس","Label":"F","Type":"string"},{"Name":"اندازه حداکثر سرسیم ترمینال DC","Description":"اندازه حداکثر سرسیم ترمینال DC","Label":"G","Type":"string"},{"Name":"ترمینال AC ورودی یوپی اس","Description":"ترمینال AC ورودی یوپی اس","Label":"H","Type":"string"},{"Name":"اندازه حداکثر سرسیم ترمینال AC ورودی","Description":"اندازه حداکثر سرسیم ترمینال AC ورودی","Label":"I","Type":"string"},{"Name":"ترمینال AC خروجی یوپی اس","Description":"ترمینال AC خروجی یوپی اس","Label":"J","Type":"string"},{"Name":"اندازه حداکثر سرسیم ترمینال AC خروجی","Description":"اندازه حداکثر سرسیم ترمینال AC خروجی","Label":"K","Type":"string"},{"Name":"وزن خالص","Description":"وزن خالص","Label":"L","Type":"string"},{"Name":"وزن با بسته بندی","Description":"وزن با بسته بندی","Label":"M","Type":"string"},{"Name":"ابعاد دستگاه ارتفاع(mm)","Description":"ابعاد دستگاه ارتفاع(mm)","Label":"N","Type":"string"},{"Name":"ابعاد دستگاه مقابل(mm)","Description":"ابعاد دستگاه مقابل(mm)","Label":"O","Type":"string"},{"Name":"ابعاد دستگاه کنار(mm)","Description":"ابعاد دستگاه کنار(mm)","Label":"P","Type":"string"},{"Name":"ابعاد بسته بندی           (L-W-H)","Description":"ابعاد بسته بندی           (L-W-H)","Label":"Q","Type":"string"},{"Name":"جریان شارژر","Description":"جریان شارژر","Label":"R","Type":"string"},{"Name":"SNMP بیرونی","Description":"SNMP بیرونی","Label":"S","Type":"string"},{"Name":"SNMP داخلی","Description":"SNMP داخلی","Label":"T","Type":"string"},{"Name":"خروجی DC  یوپی اس","Description":"خروجی DC  یوپی اس","Label":"U","Type":"string"},{"Name":"امکان پارالل","Description":"امکان پارالل","Label":"V","Type":"string"},{"Name":"کیت پارالل","Description":"کیت پارالل","Label":"W","Type":"string"},{"Name":"Maintenance Bypass","Description":"Maintenance Bypass","Label":"X","Type":"string"},{"Name":"EPO","Description":"EPO","Label":"Y","Type":"string"},{"Name":"Cold Start","Description":"Cold Start","Label":"Z","Type":"string"},{"Name":"تعداد فاز ورودی","Description":"تعداد فاز ورودی","Label":"AA","Type":"string"},{"Name":"تعداد فاز خروجی","Description":"تعداد فاز خروجی","Label":"AB","Type":"string"},{"Name":"تعداد باتری 1","Description":"تعداد باتری 1","Label":"AC","Type":"decimal"},{"Name":"Power Factor 1","Description":"Power Factor 1","Label":"AD","Type":"decimal"},{"Name":"تعداد باتری 2","Description":"تعداد باتری 2","Label":"AE","Type":"decimal"},{"Name":"Power Factor 2","Description":"Power Factor 2","Label":"AF","Type":"decimal"},{"Name":"تعداد باتری 3","Description":"تعداد باتری 3","Label":"AG","Type":"decimal"},{"Name":"Power Factor 3","Description":"Power Factor 3","Label":"AH","Type":"decimal"},{"Name":"تعداد باتری 4","Description":"تعداد باتری 4","Label":"AI","Type":"decimal"},{"Name":"Power Factor 4","Description":"Power Factor 4","Label":"AJ","Type":"decimal"},{"Name":"تعداد باتری 5","Description":"تعداد باتری 5","Label":"AK","Type":"decimal"},{"Name":"Power Factor 5","Description":"Power Factor 5","Label":"AL","Type":"decimal"},{"Name":"تعداد باتری 6","Description":"تعداد باتری 6","Label":"AM","Type":"decimal"},{"Name":"Power Factor 6","Description":"Power Factor 6","Label":"AN","Type":"decimal"},{"Name":"تعداد باتری 7","Description":"تعداد باتری 7","Label":"AO","Type":"decimal"},{"Name":"Power Factor 7","Description":"Power Factor 7","Label":"AP","Type":"decimal"},{"Name":"تعداد باتری 8","Description":"تعداد باتری 8","Label":"AQ","Type":"decimal"},{"Name":"Power Factor 8","Description":"Power Factor 8","Label":"AR","Type":"decimal"},{"Name":"تعداد باتری 9","Description":"تعداد باتری 9","Label":"AS","Type":"decimal"},{"Name":"Power Factor 9","Description":"Power Factor 9","Label":"AT","Type":"decimal"},{"Name":"تعداد باتری 10","Description":"تعداد باتری 10","Label":"AU","Type":"decimal"},{"Name":"Power Factor 10","Description":"Power Factor 10","Label":"AV","Type":"decimal"},{"Name":"تعداد باتری 11","Description":"تعداد باتری 11","Label":"AW","Type":"decimal"},{"Name":"Power Factor 11","Description":"Power Factor 11","Label":"AX","Type":"decimal"}]
                 * */
                public class XlPropSchema
                {
                    public const string Code = "كد كالا";

                }
            }
        }
    }
}
