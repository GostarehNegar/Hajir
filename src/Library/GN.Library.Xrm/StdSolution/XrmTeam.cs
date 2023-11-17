using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmTeam : XrmEntity<XrmTeam, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema
		{
			public const string LogicalName = "team";
			public const string TeamId = LogicalName + "id";
			public const string Name = "name";
			public const string IsDefault = "isdefault";
			public const string BusinessUnitId = "businessunitid";
		}

		public XrmTeam():base(Schema.LogicalName)
		{

		}
		[AttributeLogicalNameAttribute(Schema.TeamId)]
		public System.Nullable<System.Guid> TeamId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.TeamId);
			}
			set
			{
				this.SetAttributeValue(Schema.TeamId, value);
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

		[AttributeLogicalNameAttribute(Schema.TeamId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.TeamId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttribiuteValue(Schema.Name, value); }
		}

		[AttributeLogicalName(Schema.IsDefault)]
		public bool? IsDefault
		{
			get { return this.GetAttributeValue<bool?>(Schema.IsDefault); }
			set { this.SetAttribiuteValue(Schema.IsDefault, value); }
		}
		[AttributeLogicalNameAttribute(Schema.BusinessUnitId)]
		public EntityReference BusinessUnit
		{
			get
			{
				return this.GetAttributeValue<EntityReference>(Schema.BusinessUnitId);
			}
		}


	}
}
