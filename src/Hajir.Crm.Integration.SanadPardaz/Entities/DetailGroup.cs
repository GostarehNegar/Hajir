using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Entities
{
    [Table("A_detailGroup")]
    public class DetailGroup
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }

    }
}
