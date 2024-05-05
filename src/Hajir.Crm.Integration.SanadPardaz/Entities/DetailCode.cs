using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("A_DetailAccIdent")]
    public class DetailCode
    {
        [Key]
        public int DetailAccCode { get; set; }
        public string DetailAccDesc { get; set; }
        public string NationalId { get;set; }
        public byte? Sex { get; set; }
        public int? CityCode { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public byte? Typee { get; set; }
        public Int16? DetailClass { get;set; }
        public string MobileNo { get; set; }
        public string EconomicCode { get;set; }
        public string RegNo { get; set; }



    }
}
