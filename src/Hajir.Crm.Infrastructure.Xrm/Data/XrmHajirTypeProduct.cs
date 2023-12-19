using GN.Library.Shared.Entities;
using GN.Library.Xrm;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmHajirTypeProduct : XrmEntity<XrmHajirProduct, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
			public const string LogicalName = SolutionPerfix + "typeproduct";
			public const string TypeProductId = LogicalName + "id";
			public const string Name = SolutionPerfix + "name";
		}

		public XrmHajirTypeProduct() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.TypeProductId)]
		public System.Nullable<System.Guid> TypeProductId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.TypeProductId);
			}
			set
			{
				this.SetAttributeValue(Schema.TypeProductId, value);
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



		[AttributeLogicalNameAttribute(Schema.TypeProductId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.TypeProductId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

	}
}
