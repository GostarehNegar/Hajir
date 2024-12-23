using GN.Library.Xrm.StdSolution.Accounts;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	/// <summary>
	/// https://msdn.microsoft.com/en-us/library/gg328210.aspx?f=255&MSPPError=-2147217396
	/// https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/account?view=dynamics-ce-odata-9
	/// </summary>
	//
	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmAccount : XrmEntity<XrmAccount, XrmAccountServices, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : LibraryConstants.Schema.Account
		{
		}
		public XrmAccount() : base(Schema.LogicalName)
		{
		}

		[AttributeLogicalName(Schema.AccountId)]
		public System.Nullable<System.Guid> AccountId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.AccountId);
			}
			set
			{
				this.SetAttributeValue(Schema.AccountId, value);
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

		

		[AttributeLogicalName(Schema.AccountId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.AccountId = value;
			}
		}
		
		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get
			{
				
				return this.GetAttributeValue<string>(Schema.Name);
			}
			set
			{
				this.SetAttributeValue(Schema.Name, value);
			}
		}
		[AttributeLogicalName(Schema.Address1_Line1)]
		public string Address1_Line1
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Line1);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Line1, value);
			}
		}
		[AttributeLogicalName(Schema.Address1_Line2)]
		public string Address1_Line2
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Line2);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Line2, value);
			}
		}
		[AttributeLogicalName(Schema.Address1_Line3)]
		public string Address1_Line3
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Line3);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Line3, value);
			}
		}
		[AttributeLogicalName(Schema.Address1_Telehone1)]
		public string Address1_Telehone1
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Telehone1);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Telehone1, value);
			}
		}
		[AttributeLogicalName(Schema.Address1_Telehone2)]
		public string Address1_Telehone2
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Telehone2);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Telehone2, value);
			}
		}
		
		[AttributeLogicalName(Schema.Address1_Telehone2)]
		public string Address1_Telehone3
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Telehone3);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Telehone3, value);
			}
		}

		[AttributeLogicalName(Schema.Address1_Fax)]
		public string Address1_Fax
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Fax);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Fax, value);
			}
		}
		[AttributeLogicalName(Schema.Address1_City)]
		public string Address1_City
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_City);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_City, value);
			}
		}

		[AttributeLogicalName(Schema.WebSiteUrl)]
		public string WebSiteUrl
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.WebSiteUrl);
			}
			set
			{
				this.SetAttributeValue(Schema.WebSiteUrl, value);
			}
		}

		[AttributeLogicalName(Schema.Telephone1)]
		public string Telephone1 { get => this.GetAttributeValue<string>(Schema.Telephone1); set => this.SetAttribiuteValue(Schema.Telephone1, value); }

		[AttributeLogicalName(Schema.Telephone2)]
		public string Telephone2 { get => this.GetAttributeValue<string>(Schema.Telephone2); set => this.SetAttribiuteValue(Schema.Telephone2, value); }

		[AttributeLogicalName(Schema.Telephone3)]
		public string Telephone3 { get => this.GetAttributeValue<string>(Schema.Telephone3); set => this.SetAttribiuteValue(Schema.Telephone3, value); }

		[AttributeLogicalName(Schema.Fax)]
		public string Fax { get => this.GetAttributeValue<string>(Schema.Fax); set => this.SetAttribiuteValue(Schema.Fax, value); }

		[AttributeLogicalName(Schema.AccountNumber)]
		public string AccountNumber { get => this.GetAttributeValue<string>(Schema.AccountNumber); set => this.SetAttribiuteValue(Schema.AccountNumber, value); }

        [AttributeLogicalName(Schema.AccountRatingCode)]
		public OptionSetValue AccountRatingOption
		{
			get => this.GetAttributeValue<OptionSetValue>(Schema.AccountRatingCode);
			set => this.SetAttribiuteValue(Schema.AccountRatingCode, value);
		}
        //[AttributeLogicalName(Schema.AccountRatingCode)]
        public int? AccountRating
		{
			get => this.AccountRatingOption?.Value;
			set => this.AccountRatingOption = value.HasValue ? new OptionSetValue(value.Value) : null;
		}



    }




}
