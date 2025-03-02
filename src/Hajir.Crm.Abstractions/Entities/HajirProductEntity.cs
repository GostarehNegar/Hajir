using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
    public class HajirProductEntity : DynamicEntity
    {
        public new class Schema : HajirCrmConstants.Schema.Product
        {
            
        }
        public string ProductNumber { get => this.GetAttributeValue<string>(Schema.ProductNumber); set => this.SetAttributeValue(Schema.ProductNumber, value); }

       
        public Schema.StateCodes? StateCode
        {
            get => this.GetAttributeValue<Schema.StateCodes?>(Schema.StateCode);
        }
    }
}
