using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("GoodCategory")]
    public class GoodCategory
    {
        [Key]
        public Int16 CatCode { get; set; }
        public string CatName { get; set; }
    }
}
