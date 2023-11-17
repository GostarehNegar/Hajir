using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmUnitOfMeasure : XrmEntity<XrmUnitOfMeasure, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema
		{
			public const string LogicalName = "uom";
			public const string UnitOfMeasureId = "uom" + "id";
			public const string Name = "name";
		}
		public XrmUnitOfMeasure() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.UnitOfMeasureId)]
		public System.Nullable<System.Guid> UnitOfMeasureId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.UnitOfMeasureId);
			}
			set
			{
				this.SetAttributeValue(Schema.UnitOfMeasureId, value);
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

		[AttributeLogicalNameAttribute(Schema.UnitOfMeasureId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.UnitOfMeasureId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttribiuteValue(Schema.Name, value); }
		}

	}
}
