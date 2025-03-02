using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.SanadPardaz
{
    public class SanadPardazGoodModel
    {
        public string GoodCode { get; set; }
        public int? CatCode { get; set; }
        public string GoodName { get; set; }
        public string CountUnit { get; set; }
        public string SecondUnit { get; set; }
        public decimal? UnitsRatio { get; set; }
        public string CommodityCode { get; set; }
        public DateTime? ActionDate { get; set; }

       
    }
}
