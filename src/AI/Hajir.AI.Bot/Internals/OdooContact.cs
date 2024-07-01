using GN.Library.Odoo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Portal.Crm.Data.Odoo.Internals
{
	[OdooModel(Schema.LogicalName)]
	public class OdooContact : OdooEntity
	{
		public new class Schema
		{
			public const string LogicalName = "res.partner";
			public const string Name = "name";
			public const string FirstName = "gn_first_name";
			public const string LastName = "gn_last_name";
			public const string CrmId = "gn_mscrm_id";
			public const string Write_Date = "write_date";
			public const string Email = "email";
			public const string City = "city";
			public const string Street1 = "street";
			public const string Street2 = "street2";
			public const string Mobile = "mobile";
			public const string Phone = "phone";
			public const string EconomicCode = "gn_economic_code";
			public const string NationalCode = "gn_national_code";
			public const string Salutation = "gn_salutation";
			public const string Fax = "gn_fax";
			public const string ParentId = "parent_id";
			public const string CustomerRank = "customer_rank";
			public const string IsCompany = "is_company";
			public const string gn_synch_data = "gn_synch_data";
            public const string TelegramUserId = "street";
            public const string TelegramChatIdId = "street2";
			public const string Photo = "image_1920";




        }
		public OdooContact() : base(Schema.LogicalName)
		{

		}

		//public bool Update(IContactDataModel data, bool compareOnly = false)
		//{
		//	var result = false;

		//	this.Name = data.Name;
		//	if (this.IsCompany.HasValue && this.IsCompany.Value)
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.IsCompany = false;
		//	}

		//	if (!this.CustomerRank.HasValue)
		//	{
		//		result = true;
		//		if (!compareOnly)
		//			this.CustomerRank = 1;
		//	}
		//	if (!this.FirstName.EqualsEx(data.FirstName))
		//	{
		//		if (!compareOnly)
		//			this.FirstName = data.FirstName;
		//		result = true;
		//	}
		//	if (!this.LastName.EqualsEx(data.LastName))
		//	{
		//		if (!compareOnly)
		//			this.LastName = data.LastName;
		//		result = true;
		//	}
		//	if (!this.MobilePhone.EqualsEx(data.MobilePhone))
		//	{
		//		if (!compareOnly)
		//			this.MobilePhone = data.MobilePhone;
		//		result = true;
		//	}
		//	if (!this.AddressLine1.EqualsEx(data.AddressLine1))
		//	{
		//		if (!compareOnly)
		//			this.AddressLine1 = data.AddressLine1;
		//		result = true;
		//	}
		//	if (!this.AddressLine2.EqualsEx(data.AddressLine2))
		//	{
		//		this.AddressLine2 = data.AddressLine2;
		//		result = true;
		//	}
		//	if (!this.EmailAddress.EqualsEx(data.EmailAddress))
		//	{
		//		if (!compareOnly)
		//			this.EmailAddress = data.EmailAddress;
		//		result = true;
		//	}
		//	if (!this.City.EqualsEx(data.City))
		//	{
		//		this.City = data.City;
		//		result = true;
		//	}
		//	if (!this.Telephone.EqualsEx(data.Telephone))
		//	{
		//		if (!compareOnly)
		//			this.Telephone = data.Telephone;
		//		result = true;
		//	}
		//	if (!this.NationalCode.EqualsEx(data.NationalCode))
		//	{
		//		if (!compareOnly)
		//			this.NationalCode = data.NationalCode;
		//		result = true;
		//	}
		//	if (!this.EconomicCode.EqualsEx(data.EconomicCode))
		//	{
		//		if (!compareOnly)
		//			this.EconomicCode = data.EconomicCode;
		//		result = true;
		//	}
		//	if (!this.Fax.EqualsEx(data.Fax))
		//	{
		//		if (!compareOnly)
		//			this.Fax = data.Fax;
		//		result = true;
		//	}
		//	if (!this.Salutation.EqualsEx(data.Salutation))
		//	{
		//		if (!compareOnly)
		//			this.Salutation = data.Salutation;
		//		result = true;
		//	}
		//	if (this.ParentId != data.ParentAccountIntId)
		//	{
		//		if (!compareOnly)
		//			this.ParentId = data.ParentAccountIntId;
		//		result = true;
		//	}
		//	if (!this.SynchData.EqualsEx(data.SynchData))
		//	{
		//		if (!compareOnly)
		//			this.SynchData = data.SynchData;
		//		result = true;

		//	}

		//	return result;
		//}

		[OdooColumn(Schema.Name)]

		public string Name { get => this.GetAttributeValue<string>(Schema.Name); set => this.SetAttributeValue(Schema.Name, value); }

		[OdooColumn(Schema.gn_synch_data)]

		public string SynchData { get => this.GetAttributeValue<string>(Schema.gn_synch_data); set => this.SetAttributeValue(Schema.gn_synch_data, value); }

		[OdooColumn(Schema.IsCompany)]

		public bool? IsCompany { get => this.GetAttributeValue<bool?>(Schema.IsCompany); set => this.SetAttributeValue(Schema.IsCompany, value); }


		[OdooColumn(Schema.FirstName)]
		public string FirstName { get => this.GetAttributeValue<string>(Schema.FirstName); set => this.SetAttributeValue(Schema.FirstName, value); }
		[OdooColumn(Schema.LastName)]
		public string LastName { get => this.GetAttributeValue<string>(Schema.LastName); set => this.SetAttributeValue(Schema.LastName, value); }
		[OdooColumn(Schema.Email)]
		public string EmailAddress { get => this.GetAttributeValue<string>(Schema.Email); set => this.SetAttributeValue(Schema.Email, value); }
		[OdooColumn(Schema.Street1)]
		public string AddressLine1 { get => this.GetAttributeValue<string>(Schema.Street1); set => this.SetAttributeValue(Schema.Street1, value); }

		[OdooColumn(Schema.Street2)]
		public string AddressLine2 { get => this.GetAttributeValue<string>(Schema.Street2); set => this.SetAttributeValue(Schema.Street2, value); }

		[OdooColumn(Schema.City)]
		public string City { get => this.GetAttributeValue<string>(Schema.City); set => this.SetAttributeValue(Schema.City, value); }

		[OdooColumn(Schema.Mobile)]
		public string MobilePhone { get => this.GetAttributeValue<string>(Schema.Mobile); set => this.SetAttributeValue(Schema.Mobile, value); }

		[OdooColumn(Schema.TelegramUserId)]
		public string TelegramUserId { get => this.GetAttributeValue<string>(Schema.TelegramUserId); set => this.SetAttributeValue(Schema.TelegramUserId, value); }

        [OdooColumn(Schema.TelegramChatIdId)]
        public string TelegramChatId { get => this.GetAttributeValue<string>(Schema.TelegramChatIdId); set => this.SetAttributeValue(Schema.TelegramChatIdId, value); }


        [OdooColumn(Schema.Fax)]
		public string Fax { get => this.GetAttributeValue<string>(Schema.Fax); set => this.SetAttributeValue(Schema.Fax, value); }

		[OdooColumn(Schema.Phone)]
		public string Telephone { get => this.GetAttributeValue<string>(Schema.Phone); set => this.SetAttributeValue(Schema.Phone, value); }

		[OdooColumn(Schema.EconomicCode)]
		public string EconomicCode { get => this.GetAttributeValue<string>(Schema.EconomicCode); set => this.SetAttributeValue(Schema.EconomicCode, value); }

		[OdooColumn(Schema.NationalCode)]
		public string NationalCode { get => this.GetAttributeValue<string>(Schema.NationalCode); set => this.SetAttributeValue(Schema.NationalCode, value); }

		[OdooColumn(Schema.Salutation)]
		public string Salutation { get => this.GetAttributeValue<string>(Schema.Salutation); set => this.SetAttributeValue(Schema.Salutation, value); }

		[OdooColumn(Schema.ParentId)]
		public int? ParentId { get => this.GetAttributeValue<int?>(Schema.ParentId); set => this.SetAttributeValue(Schema.ParentId, value); }

		[OdooColumn(Schema.CustomerRank)]
		public int? CustomerRank { get => this.GetAttributeValue<int?>(Schema.CustomerRank); set => this.SetAttributeValue(Schema.CustomerRank, value); }

		public string Photo { get => this.GetAttributeValue<string>(Schema.Photo); set=> this.SetAttributeValue(Schema.Photo, value); }

		[OdooColumn(Schema.Write_Date)]
		public DateTime? Write_Date { get => this.GetAttributeValue<DateTime?>(Schema.Write_Date); set => this.SetAttributeValue(Schema.Write_Date, value); }
		//Guid IContactDataModel.Id { get; set; }
		public int IntId { get => this.Id; set => this.Id = this.Id; }
		//public IAccountDataModel ParrentAccount { get; set; }
		public Guid? ParentAccountId { get; set; }
		public int? ParentAccountIntId { get => this.ParentId; set => this.ParentId = value; }
		public string Country { get; set; }

		//public bool Is_Equal(IContactDataModel other)
		//{
		//	return other.FirstName == this.FirstName &&
		//		other.LastName == this.LastName &&
		//		other.MobilePhone == this.MobilePhone &&
		//		other.Telephone == this.Telephone &&
		//		other.AddressLine1 == this.AddressLine1 &&
		//		other.AddressLine2 == this.AddressLine2 &&
		//		other.City == this.City &&
		//		other.EmailAddress == this.EmailAddress;
		//}
	}


	public static class OdooContactExtensions
	{

		public static OdooContact GetBySynchData(this IOdooQueryable<OdooContact> repo, string data)
		{

			return !string.IsNullOrWhiteSpace(data)
				? repo.Execute(q => q.Filter.Equal(OdooContact.Schema.gn_synch_data, data)).FirstOrDefault()
				: null;
		}
		public static IEnumerable<OdooContact> GetByMobilePhone(this IOdooQueryable<OdooContact> repo, string phone)
		{
			var result = new OdooContact[] { };

			if (string.IsNullOrEmpty(phone) || phone.Length < 5)
				return result.AsEnumerable();
			var digits = phone.Right(5);
			result = repo
				.Execute(q => q.Filter.Like(OdooContact.Schema.Mobile, digits))
				.ToArray()
				.Where(x => x.MobilePhone == phone)
				.ToArray();
			return result;
		}

	}

}
