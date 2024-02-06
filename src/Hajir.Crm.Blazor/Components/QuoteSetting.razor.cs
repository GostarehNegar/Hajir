using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hajir.Crm.Blazor.Components
{
    public partial class QuoteSetting
    {

        private class Officiality
        {
            public bool IsOfficial { get; set; } = false;
            public string Label
            {
                get
                {
                    if (IsOfficial) return truestring;
                    else return falsestring;
                }
                set { }
            }

            public void Toggle()
            {
                IsOfficial = !IsOfficial;
            }

            public static string truestring = "انتقالی";
            public static string falsestring = "غیر انتقالی";
        }
        private class Header
        {
            public bool HasHeader { get; set; } = false;
            public string Label
            {
                get
                {
                    if (HasHeader) return truestring;
                    else return falsestring;
                }
                set { }
            }

            public void Toggle()
            {
                HasHeader = !HasHeader;
            }

            public static string truestring = "با سربرگ";
            public static string falsestring = "بدون سربرگ";
        }
        private class Aggregation
        {
            public bool IsAggregated { get; set; } = false;
            public string Label
            {
                get
                {
                    if (IsAggregated) return truestring;
                    else return falsestring;
                }
                set { }
            }

            public void Toggle()
            {
                IsAggregated = !IsAggregated;
            }

            public static string truestring = "تجمیعی";
            public static string falsestring = "تفکیکی";
        }
        private class Datasheet
        {
            public bool HasDatasheet { get; set; } = false;
            public string Label
            {
                get
                {
                    if (HasDatasheet) return truestring;
                    else return falsestring;
                }
                set { }
            }

            public void Toggle()
            {
                HasDatasheet = !HasDatasheet;
            }

            public static string truestring = "با دیتاشیت";
            public static string falsestring = "بدون دیتاشیت";
        }
        private class DiscountControl
        {
            public enum DiscountMode
            {
                WHOLE,
                ZERO_FEE,
                NO_DISCOUNT,
                INLINE
            }
            public DiscountMode Discount { get; set; } = DiscountMode.NO_DISCOUNT;
            public List<string> Labels { get; set; } = new List<string>() { "با تخفیف کل", "با مبلغ صفر", "بدون تخفیف", "تخفیف در ردیف" };
            public string Label
            {
                get
                {
                    return Discount switch
                    {
                        DiscountMode.WHOLE => Labels[(int)DiscountMode.WHOLE],
                        DiscountMode.ZERO_FEE => Labels[(int)DiscountMode.ZERO_FEE],
                        DiscountMode.NO_DISCOUNT => Labels[(int)DiscountMode.NO_DISCOUNT],
                        DiscountMode.INLINE => Labels[(int)DiscountMode.INLINE],
                        _ => "بدون تخفیف",
                    };
                }
                set { }
            }

            public void Check(int index)
            {
                this.Discount = (DiscountMode)index;
            }

            //public void SetDiscount(int index)
            //{
            //   this.Discount = ;
            //}
        }

        private Officiality Official { get; set; } = new Officiality();
        private Header HeaderOption { get; set; } = new Header();
        private Aggregation Aggregate { get; set; } = new Aggregation();
        private Datasheet Data { get; set; } = new Datasheet();
        private DiscountControl Discount { get; set; } = new DiscountControl();
    }
}
