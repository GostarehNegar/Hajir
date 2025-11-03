using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("Good")]
    public class Good
    {

        [Key]
        public string GoodCode { get; set; }
        public string GoodName { get; set;}
        public Int16? CatCode { get; set;}
        public int? Gid { get; set; }
        public string CountUnit { get; set; }
        //public DateTime ActionDate { get; set; }
    }
}
