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
    public class XrmHajirOpportunityProduct:XrmEntity<XrmHajirOpportunityProduct,DefaultStateCodes,DefaultStatusCodes>
    {
        public new class Schema : HajirCrmConstants.Schema.OpportunityProduct
        {

        }
        public XrmHajirOpportunityProduct() : base(Schema.LogicalName) { }
        [AttributeLogicalName(Schema.OpportunityProcutId)]
        public System.Nullable<System.Guid> OpportunityProcutId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.OpportunityProcutId);
            }
            set
            {
                this.SetAttributeValue(Schema.OpportunityProcutId, value);
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

        [AttributeLogicalName(Schema.OpportunityProcutId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.OpportunityProcutId = value;
            }
        }

        [AttributeLogicalName(Schema.Description)]
        public string Description
        {
            get { return this.GetAttributeValue<string>(Schema.Description); }
            set { this.SetAttributeValue(Schema.Description, value); }
        }

        [AttributeLogicalName(Schema.OpportunityId)]
        public EntityReference Opportunity
        {
            get => this.GetAttributeValue<EntityReference>(Schema.OpportunityId);
            set => this.SetAttribiuteValue(Schema.OpportunityId, value);
        }
        [AttributeLogicalName(Schema.OpportunityId)]
        public Guid? OpportunityId
        {
            get => this.Opportunity.Id;
            set => this.Opportunity = value.HasValue ? new EntityReference(XrmHajirOpportunity.Schema.LogicalName, value.Value) : null;
        }
        [AttributeLogicalName(Schema.Quantity)]
        public decimal? Quantity
        {
            get { return this.GetAttributeValue<decimal?>(Schema.Quantity); }
            set { this.SetAttribiuteValue(Schema.Quantity, value); }
        }
        [AttributeLogicalName(Schema.ExtendedAmount)]
        public Money ExtendedAmountMoney
        {
            get { return this.GetAttributeValue<Money>(Schema.ExtendedAmount); }
            set { this.SetAttribiuteValue(Schema.ExtendedAmount, value); }

        }

        public decimal? ExtendedAmount
        {
            get { return this.ExtendedAmountMoney?.Value; }
            set { this.ExtendedAmountMoney = value.HasValue ? new Money(value.Value) : null; }
        }
        [AttributeLogicalName(Schema.ProductDescription)]
        public string ProductDescription
        {
            get { return this.GetAttributeValue<string>(Schema.ProductDescription); }
            set { this.SetAttribiuteValue(Schema.ProductDescription, value); }
        }

        [AttributeLogicalName(Schema.ProductId)]
        public EntityReference Product
        {
            get { return this.GetAttributeValue<EntityReference>(Schema.ProductId); }
            set { this.SetAttribiuteValue(Schema.ProductId, value); }
        }
        public Guid? ProductId
        {
            get { return this.Product?.Id; }
            set { this.Product = value.HasValue ? new EntityReference(XrmProduct.Schema.LogicalName, value.Value) : null; }
        }
        [AttributeLogicalName(Schema.IsProductOverridden)]
        public bool? IsProductOverridden
        {
            get { return this.GetAttributeValue<bool?>(Schema.IsProductOverridden); }
            set { this.SetAttribiuteValue(Schema.IsProductOverridden, value); }
        }
        [AttributeLogicalName(Schema.SequenceNumber)]
        public int? SequenceNumber
        {
            get { return this.GetAttributeValue<int?>(Schema.SequenceNumber); }
            set { this.SetAttribiuteValue(Schema.SequenceNumber, value); }
        }
        [AttributeLogicalName(Schema.LineItemNumber)]
        public int? LineItemNumber
        {
            get { return this.GetAttributeValue<int?>(Schema.LineItemNumber); }
            set { this.SetAttribiuteValue(Schema.LineItemNumber, value); }
        }
        [AttributeLogicalName(Schema.BaseAmount)]
        public Money BaseAmountMoney
        {
            get { return this.GetAttributeValue<Money>(Schema.BaseAmount); }
            set { this.SetAttribiuteValue(Schema.BaseAmount, value); }

        }

        public decimal? BaseAmount
        {
            get { return this.BaseAmountMoney?.Value; }
            set { this.BaseAmountMoney = value.HasValue ? new Money(value.Value) : null; }
        }

        [AttributeLogicalName(Schema.ManualDiscountAmount)]
        public Money ManualDiscountAmountMoney
        {
            get { return this.GetAttributeValue<Money>(Schema.ManualDiscountAmount); }
            set { this.SetAttribiuteValue(Schema.ManualDiscountAmount, value); }

        }

        public decimal? ManualDiscountAmount
        {
            get { return this.ManualDiscountAmountMoney?.Value; }
            set { this.ManualDiscountAmountMoney = value.HasValue ? new Money(value.Value) : null; }
        }
        [AttributeLogicalName(Schema.PricePerUnit)]
        public Money PricePerUnitMoney
        {
            get { return this.GetAttributeValue<Money>(Schema.PricePerUnit); }
            set { this.SetAttribiuteValue(Schema.PricePerUnit, value); }

        }

        public decimal? PricePerUnit
        {
            get { return this.PricePerUnitMoney?.Value; }
            set { this.PricePerUnitMoney = value.HasValue ? new Money(value.Value) : null; }
        }
        [AttributeLogicalName(Schema.Tax)]
        public Money TaxMoney
        {
            get { return this.GetAttributeValue<Money>(Schema.Tax); }
            set { this.SetAttribiuteValue(Schema.Tax, value); }

        }

        public decimal? Tax
        {
            get { return this.TaxMoney?.Value; }
            set { this.TaxMoney = value.HasValue ? new Money(value.Value) : null; }
        }
        [AttributeLogicalName(Schema.UnitOfMeasureId)]
        public EntityReference UnitOfMeasureIdReference
        {
            get { return this.GetAttributeValue<EntityReference>(Schema.UnitOfMeasureId); }
            set { this.SetAttribiuteValue(Schema.UnitOfMeasureId, value); }
        }
        [AttributeLogicalName(Schema.UnitOfMeasureId)]
        public Guid? UnitOfMeasureId
        {
            get { return this.UnitOfMeasureIdReference?.Id; }
            set { this.UnitOfMeasureIdReference = value.HasValue ? new EntityReference(XrmUnitOfMeasure.Schema.LogicalName, value.Value) : null; }
        }
    }
}
