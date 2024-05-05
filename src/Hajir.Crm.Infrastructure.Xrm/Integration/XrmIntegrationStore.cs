using GN.Library.Data;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Integration;
using Hajir.Crm.Features.Integration.Infrastructure;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json.Linq;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Integration
{
    internal class XrmIntegrationStore : IIntegrationStore
    {
        private readonly IXrmDataServices dataServices;
        private readonly ICacheService cache;
        private readonly ILogger<XrmIntegrationStore> logger;

        internal class PicklistValue
        {
            public int Code { get; set; }
            public string Name { get; set; }
            public int Distance { get; set; }
            public Guid GUID { get; set; }

        }
        private List<PicklistValue> _salutions;
        private List<PicklistValue> _accountTypes;
        private List<PicklistValue> _connectiontypes;
        private List<PicklistValue> _roles;
        private List<PicklistValue> _importanceDegrees;


        public XrmIntegrationStore(IXrmDataServices dataServices, ICacheService cache, ILogger<XrmIntegrationStore> logger)
        {
            this.dataServices = dataServices;
            this.cache = cache;
            this.logger = logger;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }


        private List<PicklistValue> GetOptionSetData(string entityName, string propertyName)
        {
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = propertyName,
                    RetrieveAsIfPublished = false
                };
                var s = this.dataServices.GetXrmOrganizationService().GetOrganizationService();
                RetrieveAttributeResponse response = s.Execute(attributeRequest) as RetrieveAttributeResponse;

                string get_name(Label lbl)
                {
                    return lbl.LocalizedLabels.FirstOrDefault(x => x.LanguageCode == 1065)?.Label ?? lbl.UserLocalizedLabel.Label;
                }

                EnumAttributeMetadata attributeData = (EnumAttributeMetadata)response.AttributeMetadata;

                var optionList = (from option in attributeData.OptionSet.Options
                                  select new PicklistValue { Code = option.Value ?? 0, Name = get_name(option.Label) })
                  .ToList();
                return optionList;
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occred while trying to read picklist values. Entity:{entityName}, Field:{propertyName}, Error:{err.Message}");
            }
            return new List<PicklistValue>();
        }
        private List<PicklistValue> GetSalutions()
        {
            if (this._salutions == null)
            {
                this._salutions = this.GetOptionSetData("contact", "rhs_prefix");
            }
            return this._salutions;

        }
        private List<PicklistValue> GetImportanceDegrees()
        {
            if (this._importanceDegrees == null)
            {
                this._importanceDegrees = this.GetOptionSetData("account", XrmHajirAccount.Schema.DegreeImportance);
            }
            return this._importanceDegrees;

        }
        private List<PicklistValue> GetRoles()
        {
            if (this._roles == null)
            {
                this._roles = this.GetOptionSetData("contact", "accountrolecode");
            }
            return this._roles;

        }

        private List<PicklistValue> GetConnectionTypes()
        {
            if (this._connectiontypes == null)
            {
                this._connectiontypes = GetOptionSetData("account", XrmHajirAccount.Schema.rhs_connectiontype);
            }
            return this._connectiontypes;
        }
        private OptionSetValue GetSalutaion(string salutaion)
        {
            var f = this.GetSalutions()
                .Select(x => new PicklistValue
                {
                    Code = x.Code,
                    Name = x.Name,
                    Distance = CalcLevenshteinDistance(x.Name, salutaion)
                })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < salutaion.Length / 2)
                .ToArray()
                .FirstOrDefault();


            return f == null ? null : new OptionSetValue(f.Code);

        }
        private OptionSetValue GetAccountType(string salutaion)
        {
            if (string.IsNullOrWhiteSpace(salutaion))
                return null;
            var f = this.GetAccountTypes()
                .Select(x => new PicklistValue
                {
                    Code = x.Code,
                    Name = x.Name,
                    Distance = CalcLevenshteinDistance(x.Name, salutaion)
                })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < salutaion.Length / 2)
                .ToArray()
                .FirstOrDefault();


            return f == null ? null : new OptionSetValue(f.Code);

        }
        private OptionSetValue GetDegreeImportance(string importance)
        {

            var f = string.IsNullOrWhiteSpace(importance) ? null : this.GetImportanceDegrees()
                .Select(x => new PicklistValue
                {
                    Code = x.Code,
                    Name = x.Name,
                    Distance = CalcLevenshteinDistance(x.Name, importance)
                })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < importance.Length / 2)
                .ToArray()
                .FirstOrDefault();


            return f == null ? null : new OptionSetValue(f.Code);

        }
        private Guid? GetIndustry(string value)
        {
            Guid? result = null;

            if (!string.IsNullOrEmpty(value) && HajirCrmConstants.LegacyMaps.IndustryMap.TryGetValue(value, out var _value))
            {
                var f = this.cache.Industries
                .Select(x => new PicklistValue
                {
                    GUID = Guid.TryParse(x.Id, out var _i) ? _i : Guid.Empty,
                    Name = x.Name,
                    Distance = CalcLevenshteinDistance(x.Name, _value)
                })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < value.Length / 2)
                .ToArray()
                .FirstOrDefault();
                result = f == null ? (Guid?)null : f.GUID;

            }
            return result;
        }

        private Guid? GetMetodIntroduction(string value)
        {
            Guid? result = null;

            if (!string.IsNullOrEmpty(value) && HajirCrmConstants.LegacyMaps.NahveAshnaeiMap.TryGetValue(value, out var _value))
            {
                var f = this.cache.MethodIntrduction
                .Select(x => new PicklistValue
                {
                    GUID = Guid.TryParse(x.Id, out var _i) ? _i : Guid.Empty,
                    Name = x.Name,
                    Distance = CalcLevenshteinDistance(x.Name, _value)
                })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < value.Length / 2)
                .ToArray()
                .FirstOrDefault();
                result = f == null ? (Guid?)null : f.GUID;

            }
            return result;
        }
        private List<PicklistValue> GetAccountTypes()
        {
            if (this._accountTypes == null)
            {
                this._accountTypes = this.GetOptionSetData("account", "rhs_companytype");
            }
            return this._accountTypes;
        }

        internal static int CalcLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }

            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;

                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[lengthA, lengthB];
        }
        private OptionSetValue GetRole(IntegrationContact contact)
        {
            if (!string.IsNullOrWhiteSpace(contact.Role) && HajirCrmConstants.LegacyMaps.Roles.TryGetValue(contact.Role, out var role))
            {
                var _role = this.GetRoles()
                    .Select(x => new PicklistValue
                    {
                        Distance = CalcLevenshteinDistance(x.Name, contact.Role),
                        Code = x.Code,
                        Name = x.Name,
                    })
                    .OrderBy(x => x.Distance)
                    .ToArray()
                    .FirstOrDefault();
                if (_role != null)
                {
                    return new OptionSetValue(_role.Code);
                }

            }
            return null;

        }
        private HajirCityEntity GetCity(string city)
        {
            if (!string.IsNullOrWhiteSpace(city))
            {
                var _city = this.cache.Cities
                    .Select(x => new PicklistValue
                    {
                        GUID = Guid.TryParse(x.Id, out var _i) ? _i : Guid.Empty,
                        Name = x.Name,
                        Distance = CalcLevenshteinDistance(x.Name, city)
                    })
                    .OrderBy(x => x.Distance)
                    .FirstOrDefault();
                if (_city != null)
                {
                    return this.cache.Cities.FirstOrDefault(x => x.Id == _city.GUID.ToString());
                }
            }
            return null;

        }
        private HajirCountryEntity GetCountry(string country)
        {
            if (!string.IsNullOrWhiteSpace(country))
            {
                var _country = this.cache.Countries
                    .Select(x => new PicklistValue
                    {
                        GUID = Guid.TryParse(x.Id, out var _i) ? _i : Guid.Empty,
                        Name = x.Name,
                        Distance = CalcLevenshteinDistance(x.Name, country)
                    })
                    .OrderBy(x => x.Distance)
                    .FirstOrDefault();
                if (_country != null)
                {
                    return this.cache.Countries.FirstOrDefault(x => x.Id == _country.GUID.ToString());
                }
            }
            return null;

        }
        public IntegrationContact ImportLegacyContact(IntegrationContact contact)
        {
            var xrm_contact = this.dataServices.GetXrmOrganizationService<XrmHajirContact>()
                .CreateQuery<XrmHajirContact>()
                .FirstOrDefault(x => (string)x["rhs_externalid"] == contact.Id) ?? new XrmHajirContact();
            xrm_contact.FirstName = contact.FirstName;
            xrm_contact.LastName = contact.LastName;
            xrm_contact["rhs_externalid"] = contact.Id;
            xrm_contact["rhs_prefix"] = GetSalutaion(contact.Salutation);
            xrm_contact.MobilePhone = contact.MobilePhone;
            xrm_contact["jobtitle"] = contact.JobTitle;
            xrm_contact[XrmHajirContact.Schema.RHSAddress] = contact.Address;
            xrm_contact["telephone1"] = contact.BusinessPhone;
            xrm_contact["rhs_businessphone1"] = contact.BusinessPhone;
            xrm_contact["rhs_address"] = contact.Address;
            xrm_contact.Telephone1 = contact.BusinessPhone;
            xrm_contact.EmailAddress1 = contact.Email;
            xrm_contact["accountrolecode"] = GetRole(contact);
            xrm_contact["rhs_dateofbirth"] = contact.BirthDate;
            xrm_contact["rhs_gifts"] = contact.Hadaya;
            xrm_contact["description"] = contact.Description;
            xrm_contact["rhs_sendemail"] = !contact.DoNotEmail;
            xrm_contact["rhs_sendfax"] = !contact.DoNotFax;
            xrm_contact["donotemail"] = contact.DoNotEmail;
            xrm_contact["donotfax"] = contact.DoNotFax;
            xrm_contact["donotbulkemail"] = contact.DoNotBulkEmail;
            xrm_contact["donotphone"] = contact.DoNotPhone;
            xrm_contact["donotpostalmail"] = contact.DoNotPostalMail;
            xrm_contact["rhs_call"] = !contact.DoNotPhone;
            xrm_contact["rhs_sendpost"] = !contact.DoNotPost;
            xrm_contact["rhs_sendmessage"] = !contact.DonotSendMarketingMaterial;
            xrm_contact["rhs_postalcode"] = contact.PostalCode;
            xrm_contact[XrmContact.Schema.Address1_Line1] = contact.GetAttributeValue<string>(XrmContact.Schema.Address1_Line1);
            xrm_contact[XrmContact.Schema.Address1_Line2] = contact.GetAttributeValue<string>(XrmContact.Schema.Address1_Line2);
            xrm_contact[XrmContact.Schema.Address1_Line3] = contact.GetAttributeValue<string>(XrmContact.Schema.Address1_Line3);
            xrm_contact[XrmContact.Schema.Fax] = contact.GetAttributeValue<string>(XrmContact.Schema.Fax);
            xrm_contact["address1_country"] = contact.GetAttributeValue<string>("address1_country");
            var country = this.GetCountry(contact.GetAttributeValue<string>("address1_country"))
                ?? this.cache.Countries.Where(x => x.Name == "ایران").FirstOrDefault();

            if (country != null && Guid.TryParse(country.Id, out var _countryid))
            {
                xrm_contact["rhs_country"] = new EntityReference(XrmHajirCountry.Schema.LogicalName, _countryid);
            }


            var city_phone_code = this.GetCityPhoneCode(contact.MobilePhone, out var _phone);
            if (city_phone_code.HasValue)
            {
                xrm_contact[XrmHajirContact.Schema.RHSCityPhoneCode] = new EntityReference(XrmHajirCityPhoneCode.Schema.LogicalName, city_phone_code.Value);

                xrm_contact["rhs_businessphone1"] = _phone;
            }
            var city = this.GetCity(contact.City);
            if (city != null && Guid.TryParse(city.Id, out var cityId))
            {
                xrm_contact[XrmHajirContact.Schema.RHSCity] = new EntityReference(XrmHajirCity.Schema.LogicalName, cityId);
            }



            var account = this.GetAccountByExternalId(contact.AccontId);
            if (account != null && Guid.TryParse(account.Id, out var acc_id))
            {
                xrm_contact[XrmContact.Schema.ParentCustomerId] = new EntityReference(XrmAccount.Schema.LogicalName, acc_id);
            }

            var id = this.dataServices
                .GetRepository<XrmContact>()
                .Upsert(xrm_contact);

            try
            {
                this.dataServices.SetDates(XrmContact.Schema.LogicalName, id, contact.CreatedOn, contact.ModifiedOn);

            }
            catch (Exception err)
            {
                this.logger.LogWarning(
                    $"An error occured while trying to update CreardOn/ModifiedOn for this contact. Err:{err.Message}");
            }

            return LoadContact(id.ToString());
        }

        public IntegrationContact LoadContact(string contactId)
        {
            return contactId != null && Guid.TryParse(contactId, out var _id)
                ? this.dataServices
                .GetRepository<XrmContact>()
                .Queryable
                .FirstOrDefault(x => x.ContactId == _id)
                .ToIntegrationContact()
                : null;
        }
        public IntegrationAccount LoadAccount(string contactId)
        {
            return contactId != null && Guid.TryParse(contactId, out var _id)
                ? this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.AccountId == _id)
                .ToIntegrationAccount()
                : null;
        }

        public int? GetConnectionType(string connectionType)
        {
            int? result = null;
            if (HajirCrmConstants.LegacyMaps.RelationShipMap.TryGetValue(connectionType, out var _res))
            {
                var item = this.GetConnectionTypes()
                    .Select(x => new PicklistValue
                    {
                        Code = x.Code,
                        Name = x.Name,
                        Distance = CalcLevenshteinDistance(x.Name, _res)
                    })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < connectionType.Length / 2)
                .ToArray()
                .FirstOrDefault();
                result = item?.Code;
            }
            return result;

        }
        public Guid? GetCityPhoneCode(string phoneNumber, out string newPhoneNumber)
        {
            newPhoneNumber = phoneNumber;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var item = this.cache.CityPhoneCodes
                    .FirstOrDefault(x => phoneNumber.StartsWith(x.Name));
                if (item != null && Guid.TryParse(item.Id, out var _id))
                {
                    newPhoneNumber = phoneNumber.Substring(item.Name.Length);
                    return _id;
                }
            }

            return null;

        }

        public IntegrationAccount ImportLegacyAccount(IntegrationAccount account)
        {
            var xrm_account = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == account.Id) ?? new XrmHajirAccount();
            xrm_account.Name = account.Name?.RemoveArabic();
            xrm_account[XrmHajirAccount.Schema.ExternalId] = account.Id;
            xrm_account.AccountType = GetAccountType(account.gn_hesab_no);
            xrm_account.IndustryId = GetIndustry(account.Industry);
            xrm_account.ConectionTypeCode = GetConnectionType(account.RelationShipType);
            xrm_account.MethodIntroductionId = GetMetodIntroduction(account.Nahve_Ahnaei);
            xrm_account.NationalCode = account.gn_shenasemeli;
            xrm_account.EconomicCode = account.gn_eco_code;
            xrm_account.RegistrationNauber = account.gn_sabt;

            xrm_account.Telephone1 = account.MainPhone;
            xrm_account["telephone2"] = account.Telephone2;
            xrm_account[XrmHajirAccount.Schema.rhs_MainPhone] = account.MainPhone;
            xrm_account["rhs_subphone"] = account.Telephone2;
            xrm_account[XrmHajirAccount.Schema.BrandName] = account.AccountNumber;

            xrm_account["rhs_address"] = account.address1_name;
            xrm_account[XrmHajirAccount.Schema.DegreeImportance] = GetDegreeImportance(account.Daraje_Ahamiat);
            xrm_account["address1_postalcode"] = account.address1_postalcode;
            xrm_account[XrmHajirAccount.Schema.WebSiteUrl] = account.WebSite;
            xrm_account[XrmHajirAccount.Schema.Email1] = account.Email;

            xrm_account["donotbulkemail"] = account.DonotBulkEmail;
            xrm_account["donotemail"] = account.DonotEmail;
            xrm_account["donotfax"] = account.DoNotFax;
            xrm_account["donotphone"] = account.DoNotPhone;
            xrm_account["donotpostalmail"] = account.DoNotPostalMail;
            xrm_account["rhs_sendfax"] = !account.DoNotFax;
            xrm_account["rhs_sendpost"] = !account.DoNotPost;
            xrm_account["rhs_sendemail"] = !account.DonotEmail;


            //account.Daraje_Ahamiat

            var country = this.GetCountry(account.GetAttributeValue<string>("address1_country"))
                ?? this.cache.Countries.Where(x => x.Name == "ایران").FirstOrDefault();

            if (country != null && Guid.TryParse(country.Id, out var _countryid))
            {
                xrm_account["rhs_country"] = new EntityReference(XrmHajirCountry.Schema.LogicalName, _countryid);
            }

            var city = this.GetCity(account.City);
            if (city != null && Guid.TryParse(city.Id, out var cityId))
            {
                xrm_account[XrmHajirAccount.Schema.rhs_city] = new EntityReference(XrmHajirCity.Schema.LogicalName, cityId);
            }
            if (city != null && Guid.TryParse(city.ProvinceId, out var provinceId))
            {
                xrm_account[XrmHajirAccount.Schema.rhs_state] = new EntityReference(XrmHajirProvince.Schema.LogicalName, provinceId);
            }
            var phoneCityCode = this.GetCityPhoneCode(account.MainPhone, out var _phone);
            if (phoneCityCode.HasValue)
            {
                xrm_account[XrmHajirAccount.Schema.rhs_MainCityCode] = new EntityReference(XrmHajirCityPhoneCode.Schema.LogicalName, phoneCityCode.Value);
                xrm_account[XrmHajirAccount.Schema.rhs_MainPhone] = _phone;
            }

            var desc = xrm_account.Description;

            var import_start = "==== Import Start ====";
            xrm_account.Description = account.Description + import_start + "\r\n" + account.GetImportantIntegrationValuesAsText();

            var contact = this.GetContactByExternalId(account.PrimaryContactId);
            if (contact != null && Guid.TryParse(contact.Id, out var _primaryContact))
            {
                xrm_account[XrmHajirAccount.Schema.PrimaryContactId] = new EntityReference(XrmContact.Schema.LogicalName, _primaryContact);
            }

            var id = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Upsert(xrm_account);
            xrm_account = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.AccountId == id);
            xrm_account.Services.GetAnnotationService().UpdateObject("import_data", account);

            try
            {
                this.dataServices.SetDates("account", id, account.CreatedOn, account.ModifiedOn);
            }
            catch (Exception exp)
            {
                this.logger.LogWarning(
                    $"An error occured while tryng to set CreatedOn,ModifiedOn on this account. Err:{exp.Message} ");
            }

            //this.dataServices.WithImpersonatedSqlConnection(db => {
            //    db.Open();
            //    var transaction = db.BeginTransaction();
            //    var cmd = db.CreateCommand();
            //    cmd.Transaction = transaction;
            //    var command_text = @"
            //        update accountbase
            //        set modifiedon='$date'
            //        where accountid = '$id'
            //    ";
            //    cmd.CommandText = command_text;
            //    try
            //    {
            //        cmd.ExecuteNonQuery();
            //    }
            //    catch (Exception err)
            //    {

            //    }


            //});

            return LoadAccount(id.ToString());



        }

        public IntegrationAccount GetAccountByExternalId(string externalId)
        {
            return string.IsNullOrWhiteSpace(externalId) ? null : this.dataServices.GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == externalId)
                .ToIntegrationAccount();
        }
        public IntegrationContact GetContactByExternalId(string externalId)
        {
            return string.IsNullOrWhiteSpace(externalId) ? null : this.dataServices.GetRepository<XrmHajirContact>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == externalId)
                .ToIntegrationContact();
        }


        public IntegrationQuote LoadQuote(string id)
        {
            var result = id != null && Guid.TryParse(id, out var _id)
               ? this.dataServices
               .GetRepository<XrmHajirQuote>()
               .Queryable
               .FirstOrDefault(x => x.QuoteId == _id)
               .ToIntegrationQuote()
               : null;
            if (result != null)
            {
                var lines = this.dataServices
                    .GetRepository<XrmHajirQuoteDetail>()
                    .Queryable
                    .Where(x => x.QuoteId == _id)
                    .ToArray()
                    .Select(x => x.ToIntegrationQuoteProduct())
                    .ToArray();
                result.AddProducts(lines);

            }
            return result;

        }
        public IntegrationQuoteProduct ImportLegacryQuoteProduct(IntegrationQuoteProduct product, Guid quoteid)
        {
            var line = (product.Id != null && Guid.TryParse(product.Id, out var _id)
                ? this.dataServices
                .GetRepository<XrmHajirQuoteDetail>()
                .Queryable
                .FirstOrDefault(x => x.QuoteDetailId == _id)
                : null) ?? new XrmHajirQuoteDetail();

            line.QuoteId = quoteid;
            line.SetAttribiuteValue("isproductoverridden", true);
            line.SetAttribiuteValue("productdescription", product.GetAttributeValue("productdescription"));
            line.SetAttribiuteValue("priceperunit", product.GetAttributeValue<double>("priceperuint"));
            line.SetAttribiuteValue("priceperunit_base", product.GetAttributeValue<double>("priceperuint_base"));

            if (line.Id == Guid.Empty)
            {
                _id = this.dataServices
                    .GetRepository<XrmHajirQuoteDetail>()
                    .Insert(line);
            }
            else
            {
                _id = this.dataServices
                    .GetRepository<XrmHajirQuoteDetail>()
                    .Upsert(line);
            }

            return this.dataServices
                .GetRepository<XrmHajirQuoteDetail>()
                .Queryable
                .FirstOrDefault(x => x.QuoteDetailId == _id)?
                .ToIntegrationQuoteProduct();



        }
        public IntegrationQuote ImportLegacyQuote(IntegrationQuote quote)
        {
            var xrm_quote = this.dataServices
                .GetRepository<XrmHajirQuote>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == quote.Id) ?? new XrmHajirQuote();
            if (quote.AccountId != null)
            {
                var account = this.GetAccountByExternalId(quote.AccountId);
                xrm_quote.AccountId = Guid.TryParse(account?.Id, out var _t) ? _t : (Guid?)null;
            }
            xrm_quote.ExternalId = quote.Id;
            xrm_quote.Name = quote.GetAttributeValue("name");
            xrm_quote.QuoteNumber = quote.GetAttributeValue("quotenumber");

            var id = this.dataServices
                .GetRepository<XrmQuote>()
                .Upsert(xrm_quote);

            foreach (var line in quote.Products)
            {
                ImportLegacryQuoteProduct(line, id);
            }
            return LoadQuote(id.ToString());

        }
    }
}
