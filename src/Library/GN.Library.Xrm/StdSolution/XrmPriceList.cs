using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmPriceList : XrmEntity<XrmPriceList, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema :XrmEntity.Schema
		{
			public const string LogicalName = "pricelevel";
			public const string PriceListId = LogicalName + "id";
			public const string Name = "name";
		}

		public XrmPriceList() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalName(Schema.PriceListId)]
		public System.Nullable<System.Guid> PriceListId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PriceListId);
			}
			set
			{
				this.SetAttributeValue(Schema.PriceListId, value);
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



		[AttributeLogicalName(Schema.PriceListId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.PriceListId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); }


	}


	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmPriceListItem:XrmEntity<XrmPriceListItem, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "productpricelevel";
			public const string PriceListItemId = LogicalName + "id";
			public const string PriceListId = "pricelevelid";
			public const string ProcuctId = "productid";
			public const string Name = "name";
			public const string Amount = "amount";
			public const string uomid = "uomid";
			public const string uomscheduleid = "uomscheduleid";

        }
		public XrmPriceListItem() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.PriceListItemId)]
		public System.Nullable<System.Guid> PriceListItemId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PriceListItemId);
			}
			set
			{
				this.SetAttributeValue(Schema.PriceListItemId, value);
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



		[AttributeLogicalName(Schema.PriceListItemId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.PriceListItemId = value;
			}
		}

		[AttributeLogicalName(Schema.PriceListId)]
		public EntityReference PriceList
		{
			get => this.GetAttributeValue<EntityReference>(Schema.PriceListId);
			set => this.SetAttribiuteValue(Schema.PriceListId,value);
		}
		[AttributeLogicalName(Schema.PriceListId)]
		public Guid? PriceListId
		{
			get => this.PriceList?.Id;
			set => this.PriceList = value.HasValue ? new EntityReference(XrmPriceList.Schema.LogicalName, value.Value) : null;
		}
		[AttributeLogicalName(Schema.ProcuctId)]
		public EntityReference Procuct
		{
			get => this.GetAttributeValue<EntityReference>(Schema.ProcuctId);
			set => this.SetAttribiuteValue(Schema.ProcuctId, value);
		}
		[AttributeLogicalName(Schema.ProcuctId)]
		public Guid? ProcuctId
		{
			get => this.Procuct?.Id;
			set => this.Procuct = value.HasValue ? new EntityReference(XrmProduct.Schema.LogicalName, value.Value) : null;
		}

		[AttributeLogicalName(Schema.Amount)]
		public Money AmountMoney
		{
			get { return this.GetAttributeValue<Money>(Schema.Amount); }
			set { this.SetAttribiuteValue(Schema.Amount, value); }

		}
		[AttributeLogicalName(Schema.Amount)]
		public decimal? Amount
		{
			get { return this.AmountMoney?.Value; }
			set { this.AmountMoney = value.HasValue ? new Money(value.Value) : null; }
		}

	}
}
