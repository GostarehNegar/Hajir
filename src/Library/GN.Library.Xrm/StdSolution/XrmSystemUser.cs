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
	public class XrmSystemUser : XrmEntity<XrmSystemUser, DefaultStateCodes, DefaultStatusCodes>
	{
		public class XrmSystemUserServices : XrmEntityService<XrmSystemUser>
		{
			public XrmSystemUserServices(XrmSystemUser user) : base(user) { }

		}
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "systemuser";
			public const string SystemUserId = LogicalName + "id";
			public const string FirstName = "firstname";
			public const string LastName = "lastname";
			public const string FullName = "fullname";
			public const string DomainName = "domainname";
			public const string BusinessUnitId = "businessunitid";
			public const string MobilePhone = "mobilephone";
			public const string HomePhone = "homephone";
			public const string Address1_Telephone2 = "address1_telephone2";
			public const string Address1_Telephone1 = "address1_telephone1";

		}

		public XrmSystemUser() : base("systemuser") { }

		[AttributeLogicalNameAttribute(Schema.SystemUserId)]
		public System.Nullable<System.Guid> SystemUserId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SystemUserId);
			}
			set
			{
				this.SetAttributeValue(Schema.SystemUserId, value);
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

		[AttributeLogicalNameAttribute(Schema.SystemUserId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.SystemUserId = value;
			}
		}

		[AttributeLogicalNameAttribute(Schema.FirstName)]
		public string FirstName
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.FirstName);
			}
			set
			{
				this.SetAttributeValue(Schema.FirstName, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.Address1_Telephone2)]
		public string Address1_Telephone2
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Telephone2);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Telephone2, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.Address1_Telephone1)]
		public string Address1_Telephone1
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Telephone1);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Telephone1, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.HomePhone)]
		public string HomePhone
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.HomePhone);
			}
			set
			{
				this.SetAttributeValue(Schema.HomePhone, value);
			}
		}
		[AttributeLogicalNameAttribute(Schema.LastName)]
		public string LastName
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.LastName);
			}
			set
			{
				this.SetAttributeValue(Schema.LastName, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.FullName)]
		public string FullName
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.FullName);
			}
			set
			{
				this.SetAttributeValue(Schema.FullName, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.DomainName)]
		public string DomainName
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.DomainName);
			}
			set
			{
				this.SetAttributeValue(Schema.DomainName, value);
			}
		}

		//[AttributeLogicalNameAttribute(Schema.BusinessUnitId)]
		//public EntityReference BusinessUnit
		//{
		//	get
		//	{
		//		return this.GetAttributeValue<EntityReference>(Schema.BusinessUnitId);
		//	}
		//}

		[AttributeLogicalNameAttribute(Schema.MobilePhone)]
		public string MobilePhone
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.MobilePhone);
			}
			set
			{
				this.SetAttributeValue(Schema.MobilePhone, value);
			}
		}
	
	
		[AttributeLogicalName("isdisabled")]
		public bool IsDisabled
        {
			get => this.GetAttributeValue<bool>("isdisabled");
			set => this.SetAttribiuteValue("isdisabled", value);
		}
	}
}
