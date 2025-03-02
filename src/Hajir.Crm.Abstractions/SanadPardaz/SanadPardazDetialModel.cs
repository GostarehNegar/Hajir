using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.SanadPardaz
{
    public class SanadPardazDetialModel
    {
        public int DetailAccCode { get; set; }
        public string DetailAccDesc { get; set; }
        public int? DetailClass { get; set; }
        public int? Parent_DetailAccCode { get; set; }
        public int? Parent_2_DetailAccCode { get; set; }
        public string NationalId { get; set; }
        public int? Typee { get; set; }
        public string ZipCode { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string TellNo { get; set; }
        public string FaxNo { get; set; }
        public string Addr { get; set; }
        public string City { get; set; }
        public int? ProvinceID { get; set; }
        public string RegNo { get; set; }
        public string EconomicCode { get; set; }
        public int? BuyerType { get; set; }
        public int? CityCode { get; set; }
        public string PreCodeCity { get; set; }
        public string NationId { get; set; }
        public int? Parent_3_DetailAccCode { get; set; }
        public int? Parent_4_DetailAccCode { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
