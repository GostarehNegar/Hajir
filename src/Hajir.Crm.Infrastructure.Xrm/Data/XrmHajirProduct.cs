using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmHajirProduct : XrmProduct
	{
		public new class Schema : HajirProductEntity.Schema
		{

		}

		[AttributeLogicalName(Schema.ProductType)]
		public OptionSetValue ProductTypeCode { get => this.GetAttributeValue<OptionSetValue>(Schema.ProductType); set => this.SetAttributeValue(Schema.ProductType, value); }

		public Schema.ProductTypes? ProductType
		{
			get => this.ProductTypeCode == null ? (Schema.ProductTypes?)null : (Schema.ProductTypes)this.ProductTypeCode.Value;
			set => this.ProductTypeCode = value.HasValue ? new OptionSetValue((int)value.Value) : null;
		}
	}
}
