using GN.Library.Shared.Entities;
using Hajir.Crm.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration
{
    public class IntegrationProduct : HajirProductEntity
    {
        public new class Schema : HajirProductEntity.Schema
        {

        }
        public string SanadPardazCode { get; set; }
        public string CatName { get; set; }
        public int? CatCode { get; set; }
        public string GroupName { get; set; }
        public int? GroupId { get; set; }

        public string UnitOfMeasurement { get; set; }

        public DateTime? SynchedOn => this.GetAttributeValue<DateTime?>(Schema.SynchedOn);
    }
}
