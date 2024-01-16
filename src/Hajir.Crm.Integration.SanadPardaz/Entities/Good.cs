using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    public class Good
    {
        [Key]
        public string GoodCode { get; set; }
        public string GoodName { get; set;}
        public Int16 CatCode { get; set;}
    }
}
