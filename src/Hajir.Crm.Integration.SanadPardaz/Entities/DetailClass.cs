using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("A_DetailClass")]
    public class DetailClass
    {
        [Key]
        public Int16 ClassCode { get; set; }
        public string ClassDesc { get; set; }
    }
}
