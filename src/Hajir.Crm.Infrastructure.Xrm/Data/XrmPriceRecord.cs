using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmPriceRecord : XrmEntity<XrmPriceRecord, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : HajirCrmConstants.Schema.PriceRecord
        {

        }
        public XrmPriceRecord() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.PriceRecordId)]
        public System.Nullable<System.Guid> PriceRecordId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PriceRecordId);
            }
            set
            {
                this.SetAttributeValue(Schema.PriceRecordId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }



        [AttributeLogicalNameAttribute(Schema.PriceRecordId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.PriceRecordId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

        [AttributeLogicalName(Schema.ProductId)] 
        public EntityReference ProductReference
        {
            get => this.GetAttributeValue<EntityReference>(Schema.ProductId);
            set => this.SetAttribiuteValue(Schema.ProductId, value);
        }
        [AttributeLogicalName(Schema.ProductId)]
        public Guid? ProductId
        {
            get => this.ProductReference?.Id;
            set => this.ProductReference = value.HasValue ? new EntityReference(XrmProduct.Schema.LogicalName, value.Value) : null;
        }

        [AttributeLogicalName(Schema.Price1)]
        public Money Price1
        {
            get => this.GetAttributeValue<Money>(Schema.Price1);
            set => this.SetAttribiuteValue(Schema.Price1, value);
        }
        [AttributeLogicalName(Schema.Price2)]
        public Money Price2
        {
            get => this.GetAttributeValue<Money>(Schema.Price2);
            set => this.SetAttribiuteValue(Schema.Price2, value);
        }

    }
}
