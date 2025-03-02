using System;

namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class GetGoodRequestModel
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string orderBy { get; set; }
        public string orderDirection { get; set; }
    }
    public class GetGoodResponseModel
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
