using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class GNClassType : XrmEntity<DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "gn_casetype";
			public const string CaseTypeId = LogicalName + "id";
			public const string Name = "gn_name";
		}
		public GNClassType() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalNameAttribute(Schema.CaseTypeId)]
		public System.Nullable<System.Guid> CaseTypeId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.CaseTypeId);
			}
			set
			{
				this.SetAttributeValue(Schema.CaseTypeId, value);
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

		[AttributeLogicalNameAttribute(Schema.CaseTypeId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.CaseTypeId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}


	}
}
