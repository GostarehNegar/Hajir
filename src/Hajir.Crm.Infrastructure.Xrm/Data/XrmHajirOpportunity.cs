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
    public class XrmHajirOpportunity : XrmEntity<XrmHajirOpportunity, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : HajirCrmConstants.Schema.Opportunity
        {

        }
        public XrmHajirOpportunity() : base(Schema.LogicalName)
        {

        }
        [AttributeLogicalName(Schema.OpportunityId)]
        public System.Nullable<System.Guid> OpportunityId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.OpportunityId);
            }
            set
            {
                this.SetAttributeValue(Schema.OpportunityId, value);
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

        [AttributeLogicalName(Schema.OpportunityId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.OpportunityId = value;
            }
        }

        [AttributeLogicalName(Schema.Name)]
        public string Topic
        {
            get { return this.GetAttributeValue<string>(Schema.Name); }
            set { this.SetAttributeValue(Schema.Name, value); }
        }


        [AttributeLogicalName(Schema.parentaccountid)]
        public EntityReference Account
        {
            get => this.GetAttributeValue<EntityReference>(Schema.parentaccountid);
            set => this.SetAttribiuteValue(Schema.parentaccountid, value);
        }
        [AttributeLogicalName(Schema.parentaccountid)]
        public Guid? AccountId
        {
            get => this.Account?.Id;
            set => this.Account = value.HasValue ? new EntityReference(XrmHajirAccount.Schema.LogicalName, value.Value) : null;
        }
        [AttributeLogicalName(Schema.pricelevelid)]
        public EntityReference PriceLevel { get => this.GetAttributeValue<EntityReference>(Schema.pricelevelid); set => this.SetAttribiuteValue(Schema.pricelevelid, value); }

        [AttributeLogicalName(Schema.pricelevelid)]
        public Guid? PriceLevelId { get => this.PriceLevel?.Id; set => this.PriceLevel = value.HasValue ? new EntityReference(XrmPriceList.Schema.LogicalName, value.Value) : null; }

        [AttributeLogicalName(Schema.estimatedvalue)]
        public Money EstimavetRevenueMoney
        {
            get => this.GetAttributeValue<Money>(Schema.estimatedvalue);
            set => this.SetAttribiuteValue(Schema.estimatedvalue, value);
        }
        public decimal? EstimavetRevenue
        {
            get { return this.EstimavetRevenueMoney?.Value; }
            set { this.EstimavetRevenueMoney = value.HasValue ? new Money(value.Value) : null; }
        }
    }
}
