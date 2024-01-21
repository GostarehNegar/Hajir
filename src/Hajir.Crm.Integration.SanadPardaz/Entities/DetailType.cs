using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("A_DetailType")]
    public class DetailType
    {
        [Key]
        public byte Id { get; set; }
        public string Name { get; set; }
        public bool ActiveFlag { get; set; }

    }
}
