using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("GoodGroup")]
    public class GoodGroup
    {
        [Key]
        public int Gid { get; set; }
        public string GName { get; set; }

    }
}
