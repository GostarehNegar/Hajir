using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("GoodGroupType")]
    public class GoodGroupType
    {
        [Key]
        public Byte GroupTypeCode { get; set; }
        public string GroupTypeName { get; set; }
    }
}
