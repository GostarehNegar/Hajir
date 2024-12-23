using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmContact : XrmEntity<XrmContact, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmEntity.Schema
		{
			public const string LogicalName = "contact";
			public const string ContactId = "contactid";
			public const string FirstName = "firstname";
			public const string LastName = "lastname";
			public const string ParentCustomerId = "parentcustomerid";
			public const string FullName = "fullname";
			public const string MobilePhone = "mobilephone";
			public const string Address = "address1_composite";
			public const string Address1_Line1 = "address1_line1";
			public const string Address1_Line2 = "address1_line2";
			public const string Address1_Line3 = "address1_line3";
			public const string Address1_City = "address1_city";
			public const string Address1_Country = "address1_country";
			public const string address1_stateorprovince = "address1_stateorprovince";
            public const string Telephone1 = "telephone1";
			public const string Telephone2 = "telephone2";
			public const string Telephone3 = "telephone3";
			public const string EmailAddress1 = "emailaddress1";
			public const string EmailAddress2 = "emailaddress2";
			public const string EmailAddress3 = "emailaddress3";
			public const string Salutation = "salutation";
			public const string Fax = "fax";
			




		}
		public XrmContact() : base(Schema.LogicalName) { }
		[AttributeLogicalName(Schema.ContactId)]
		public Guid ContactId
		{
			get
			{
				return this.GetAttributeValue<Guid>(Schema.ContactId);
			}
			set
			{
				this.SetAttributeValue(Schema.ContactId, value);
				base.Id = value;
				//if (value.HasValue)
				//{
				//	base.Id = value.Value;
				//}
				//else
				//{
				//	base.Id = System.Guid.Empty;
				//}
			}
		}

       

		[AttributeLogicalNameAttribute(Schema.ContactId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.ContactId = value;
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

		[AttributeLogicalNameAttribute(Schema.Fax)]
		public string Fax
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Fax);
			}
			set
			{
				this.SetAttributeValue(Schema.Fax, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.Address1_City)]
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

		[AttributeLogicalNameAttribute(Schema.Address1_Line1)]
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

		[AttributeLogicalNameAttribute(Schema.Address1_Line2)]
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

		[AttributeLogicalNameAttribute(Schema.Address1_Line3)]
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


		[AttributeLogicalNameAttribute(Schema.Address1_Country)]
		public string Address1_Country
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Address1_Country);
			}
			set
			{
				this.SetAttributeValue(Schema.Address1_Country, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.Telephone1)]
		public string Telephone1
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Telephone1);
			}
			set
			{
				this.SetAttributeValue(Schema.Telephone1, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.Telephone2)]
		public string Telephone2
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Telephone2);
			}
			set
			{
				this.SetAttributeValue(Schema.Telephone2, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.Telephone3)]
		public string Telephone3
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Telephone3);
			}
			set
			{
				this.SetAttributeValue(Schema.Telephone3, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.EmailAddress1)]
		public string EmailAddress1
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.EmailAddress1);
			}
			set
			{
				this.SetAttributeValue(Schema.EmailAddress1, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.EmailAddress2)]
		public string EmailAddress2
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.EmailAddress2);
			}
			set
			{
				this.SetAttributeValue(Schema.EmailAddress2, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.EmailAddress3)]
		public string EmailAddress3
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.EmailAddress3);
			}
			set
			{
				this.SetAttributeValue(Schema.EmailAddress3, value);
			}
		}

		[AttributeLogicalNameAttribute(Schema.Salutation)]
		public string Salutation
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Salutation);
			}
			set
			{
				this.SetAttributeValue(Schema.Salutation, value);
			}
		}

		[AttributeLogicalName(Schema.ParentCustomerId)]
		public EntityReference ParentCustomer
		{
			get { return this.GetAttributeValue<EntityReference>(Schema.ParentCustomerId); }
			set { this.SetAttributeValue(Schema.ParentCustomerId, value); }

		}





		[AttributeLogicalName(Schema.ParentCustomerId)]
		
		public Guid? AccountId
		{
			get { return this.ParentCustomer?.Id; }
			set
			{
				ParentCustomer = value.HasValue
					? new EntityReference(XrmAccount.Schema.LogicalName, value.Value)
					: null;
			}
		}


	}

	public static partial class StdSoltutionExtensions
    {
		public static IEnumerable<TEntity> FindByName<TEntity>(
			this IXrmRepository<TEntity> repo,
			string searchText,
			int page = 0,
			int pageLength = 10) 
			where TEntity : XrmContact
        {
			return repo.Queryable
				.Where(x => x.FirstName.Contains(searchText) || x.LastName.Contains(searchText))
				.Skip(page*pageLength)
				.Take(pageLength)
				.ToArray();
        }
    }
}
