using GN.Library.Odoo;
using GN.Library.Odoo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Portal.Crm.Data.Odoo.Internals
{
	[OdooModel(Schema.LogicalName)]
	public class OdooAccount : OdooPartner
	{
		public new class Schema : OdooPartner.Schema
		{
			//public const string LogicalName = "res.partner";
			//public const string Name = "name";
			//public const string Is_Company = "is_company";
			public const string Synch_Data = "gn_synch_data";
			public const string Street = "street";
			public const string Street2 = "street2";
			public const string Fax = "gn_fax";
			public const string EconomicCode = "gn_economic_code";
			public const string RegistrationCode = "gn_registration_code";
			public const string Phone = "phone";
			public const string City = "city";
			public const string Write_Date = "write_date";
			public const string CustomerRank = "customer_rank";
			public const string ParentId = "parent_id";
			public const string WebSite = "website";
			public const string IsCompany = "is_company";
			public const string reference = "ref";

		}
		public OdooAccount() : base()
		{

		}
		[OdooColumn(Schema.Write_Date)]
		public DateTime? Write_Date { get => this.GetAttributeValue<DateTime?>(Schema.Write_Date); set => this.SetAttributeValue(Schema.Write_Date, value); }

		[OdooColumn(Schema.Name)]
		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttributeValue(Schema.Name, value); }

		[OdooColumn(Schema.IsCompany)]

		public bool? IsCompany { get => this.GetAttributeValue<bool?>(Schema.IsCompany); set => this.SetAttributeValue(Schema.IsCompany, value); }

		[OdooColumn(Schema.ParentId)]
		public int? ParentId { get => this.GetAttributeValue<int?>(Schema.ParentId); set => this.SetAttributeValue(Schema.ParentId, value); }

		[OdooColumn(Schema.CustomerRank)]
		public int? CustomerRank { get => this.GetAttributeValue<int?>(Schema.CustomerRank); set => this.SetAttributeValue(Schema.CustomerRank, value); }


		[OdooColumn(Schema.Synch_Data)]
		public string SynchData { get => this.GetAttributeValue<string>(Schema.Synch_Data); set => this.SetAttributeValue(Schema.Synch_Data, value); }

		[OdooColumn(Schema.Street)]
		public string Street { get => this.GetAttributeValue<string>(Schema.Street); set => this.SetAttributeValue(Schema.Street, value); }

		[OdooColumn(Schema.Street2)]
		public string Street2 { get => this.GetAttributeValue<string>(Schema.Street2); set => this.SetAttributeValue(Schema.Street2, value); }

		[OdooColumn(Schema.EconomicCode)]
		public string EconomicCode { get => this.GetAttributeValue<string>(Schema.EconomicCode); set => this.SetAttributeValue(Schema.EconomicCode, value); }

		[OdooColumn(Schema.RegistrationCode)]
		public string RegistrationCode { get => this.GetAttributeValue<string>(Schema.RegistrationCode); set => this.SetAttributeValue(Schema.RegistrationCode, value); }

		[OdooColumn(Schema.Phone)]
		public string Phone { get => this.GetAttributeValue<string>(Schema.Phone); set => this.SetAttributeValue(Schema.Phone, value); }

		[OdooColumn(Schema.WebSite)]
		public string WebSite { get => this.GetAttributeValue<string>(Schema.WebSite); set => this.SetAttributeValue(Schema.WebSite, value); }

		[OdooColumn(Schema.Fax)]
		public string Fax { get => this.GetAttributeValue<string>(Schema.Fax); set => this.SetAttributeValue(Schema.Fax, value); }

		[OdooColumn(Schema.City)]
		public string City { get => this.GetAttributeValue<string>(Schema.City); set => this.SetAttributeValue(Schema.City, value); }


		[OdooColumn(Schema.Is_Company)]
		public bool? Is_Company { get => this.GetAttributeValue<bool?>(Schema.Is_Company); set => this.SetAttributeValue(Schema.Is_Company, value); }
		//Guid IAccountDataModel.Id { get; set; }
		public int IntId { get => this.Id; set => this.Id = this.Id; }
		public string AddressLine1 { get => this.Street; set => this.Street = value; }
		public string AddressLine2 { get => this.Street2; set => this.Street2 = value; }

		public string Telephone { get => this.Phone; set => this.Phone = value; }
		public string Reference { get => this.GetAttributeValue<string>(Schema.reference); set => this.SetAttributeValue(Schema.reference,value); }
		public string AccountNumber { get => this.Reference; set => this.Reference = value; }
		//public bool Update(IAccountDataModel model, bool compareOnly = false)
		//{
		//	var result = false;
		//	if (!this.Name.EqualsEx(model.Name))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.Name = model.Name;

		//	}
		//	if (string.IsNullOrWhiteSpace(this.Name))
		//	{
		//		this.Name = "no name";
		//	}
		//	if (!this.IsCompany.HasValue || !this.IsCompany.Value)
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.IsCompany = true;
		//	}
		//	if (!this.CustomerRank.HasValue)
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.CustomerRank = 1;
		//	}
		//	if (!this.EconomicCode.EqualsEx(model.EconomicCode))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.EconomicCode = model.EconomicCode;
		//	}
		//	if (!this.RegistrationCode.EqualsEx(model.RegistrationCode))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.RegistrationCode = model.RegistrationCode;
		//	}
		//	if (!this.AddressLine1.EqualsEx(model.AddressLine1))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.AddressLine1 = model.AddressLine1;
		//	}
		//	if (!this.AddressLine2.EqualsEx(model.AddressLine2))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.AddressLine2 = model.AddressLine2;
		//	}
		//	if (!this.Telephone.EqualsEx(model.Telephone))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.Telephone = model.Telephone;
		//	}
		//	if (!this.Fax.EqualsEx(model.Fax))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.Fax = model.Fax;
		//	}
		//	if (!this.City.EqualsEx(model.City))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.City = model.City;
		//	}
		//	if (!this.WebSite.EqualsEx(model.WebSite))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.WebSite = model.WebSite;
		//	}
		//	if (!this.SynchData.EqualsEx(model.SynchData))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.SynchData = model.SynchData;

		//	}
		//	if (!this.AccountNumber.EqualsEx(model.AccountNumber))
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.AccountNumber = model.AccountNumber;

		//	}
		//	return result;

		//}

		//public bool Equals(IAccountDataModel model)
		//{

		//	return this.Name == model.Name;
		//}

	}


	public static class PortalAccountExtensions
	{
		public const string MissingPartner = "_MISSING PARTNER_";
		public static OdooAccount GetBySynchData(this IOdooQueryable<OdooAccount> repo, string data)
		{
			return string.IsNullOrEmpty(data)
				? null
				: repo.Execute(q =>
				{
					q.Filter.Equal(OdooAccount.Schema.Synch_Data, data);
				}).FirstOrDefault();
		}
		public static OdooAccount GetMissingPartnerAccount(this IOdooQueryable<OdooAccount> repo)
		{

			OdooAccount result = repo.GetBySynchData(MissingPartner);
			if (result == null)
			{
				result = repo.Connection.NewAccount();
				result.Name = MissingPartner;
				result.SynchData = MissingPartner;
				result.Save();
			}
			return result;

		}

	}
}
