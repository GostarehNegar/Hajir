using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	/// <summary>
	/// 
	/// https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/userquery?view=op-9-1
	/// </summary>
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmUserQuery : XrmEntity<XrmUserQuery>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "userquery";
			public const string UserQueryId = LogicalName + "id";
        }
        public XrmUserQuery() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.UserQueryId)]
		public Guid? UserQueryId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.UserQueryId);
			}
			set
			{
				this.SetAttributeValue(Schema.UserQueryId, value);
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
		[AttributeLogicalName(Schema.UserQueryId)]
		public override Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.UserQueryId = value;
			}
		}
	}
}
