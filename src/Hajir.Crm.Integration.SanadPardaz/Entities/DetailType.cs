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
    public enum DetailTypes
    {
        Haghighi = 0,
        Hoghoghi_Gehir_Dolati = 1,
        Hoghoghi_Dolati = 2,
        Moadi_Mashmool_Sabtnam_Dar_Neazame_Maliati = 5,
        Mahmool_Haghighi_Madeh_81 = 6,
        Ahskasi_ke_Madhmool_Maliat_Nistand = 7,
        Masraf_Konandeh_Nahaei = 8
    }
}
