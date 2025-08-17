using GN.Library.Data;
using GN.Library.Shared.Entities;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Hajir.Crm.Entities;
using Hajir.Crm.Integration;
using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Integration.Infrastructure;
using MassTransit.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json.Linq;
using NLog.Targets;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using static Hajir.Crm.Entities.GeoData;


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
        private List<PicklistValue> _rhssalutions;
        private List<PicklistValue> _salutions;
        private List<PicklistValue> _rhsAccountTypes;
        private List<PicklistValue> _accountTypes;
        private List<PicklistValue> _rhsconnectiontypes;
        private List<PicklistValue> _roles;
        private List<PicklistValue> _importanceDegrees;
        private List<PicklistValue> _accountRatings;
        private List<PicklistValue> _introductionMethods;
        private List<PicklistValue> _relationTypes;
        private EntityReference _defualtBusinessUnit;
        private EntityReference _defualtRole;

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
        private List<PicklistValue> GetRhsSalutions()
        {
            if (this._rhssalutions == null)
            {
                this._rhssalutions = this.GetOptionSetData("contact", "rhs_prefix");
            }
            return this._rhssalutions;

        }
        private List<PicklistValue> GetSalutions()
        {
            if (this._salutions == null)
            {
                this._salutions = this.GetOptionSetData("contact", XrmHajirContact.Schema.SalutaionCode);
            }
            return this._salutions;

        }
        private List<PicklistValue> GetImportanceDegrees()
        {
            if (this._importanceDegrees == null)
            {
                this._importanceDegrees = this.GetOptionSetData("account", XrmHajirAccount.Schema.ImportanceCode);
            }
            return this._importanceDegrees;

        }
        private List<PicklistValue> GetAccountRatings()
        {
            if (this._accountRatings == null)
            {
                this._accountRatings = this.GetOptionSetData("account", XrmHajirAccount.Schema.AccountRatingCode);
            }
            return this._accountRatings;

        }
        private List<PicklistValue> GetRoles()
        {
            if (this._roles == null)
            {
                this._roles = this.GetOptionSetData("contact", "accountrolecode");
            }
            return this._roles;

        }

        private List<PicklistValue> GetIntroductionMethods()
        {
            if (this._introductionMethods == null)
            {
                this._introductionMethods = GetOptionSetData("account", XrmHajirAccount.Schema.IntroductionMethod);
            }
            return this._introductionMethods;
        }

        private List<PicklistValue> GetConnectionTypes()
        {
            if (this._rhsconnectiontypes == null)
            {
                this._rhsconnectiontypes = GetOptionSetData("account", XrmHajirAccount.Schema.rhs_connectiontype);
            }
            return this._rhsconnectiontypes;
        }
        private List<PicklistValue> GetRelationTypes()
        {
            if (this._relationTypes == null)
            {
                this._relationTypes = GetOptionSetData("account", XrmHajirAccount.Schema.RelationTypeCode);
            }
            return this._relationTypes;
        }
        private OptionSetValue GetRhsSalutaion(string salutaion)
        {
            var f = this.GetRhsSalutions()
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
        private OptionSetValue GetSalutaionCode(string salutaion)
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
        private OptionSetValue GetRhsAccountType(string salutaion)
        {
            if (string.IsNullOrWhiteSpace(salutaion))
                return null;
            var f = this.GetRhsAccountTypes()
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
        private OptionSetValue GetAccountRatingValue(string importance)
        {

            var f = string.IsNullOrWhiteSpace(importance) ? null : this.GetAccountRatings()
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
        private List<PicklistValue> GetRhsAccountTypes()
        {
            if (this._rhsAccountTypes == null)
            {
                this._rhsAccountTypes = this.GetOptionSetData("account", "rhs_companytype");
            }
            return this._rhsAccountTypes;
        }
        private List<PicklistValue> GetAccountTypes()
        {
            if (this._accountTypes == null)
            {
                this._accountTypes = this.GetOptionSetData("account", XrmHajirAccount.Schema.AccountType);
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
        public EntityReference ImportEntityReference(EntityReference entityReference)
        {
            return HajirXrmExtensions.ImportEntityReference(this.dataServices, entityReference);
        }
        public IntegrationContact ImportLegacyContact(IntegrationContact contact)
        {
            var xrm_contact = this.dataServices.GetXrmOrganizationService<XrmHajirContact>()
                .CreateQuery<XrmHajirContact>()
                .FirstOrDefault(x => x.Id == contact.GetId<Guid>()) ?? new XrmHajirContact();
            var is_new = xrm_contact.Id == Guid.Empty;
            if (is_new)
            {
                xrm_contact.Id = Guid.TryParse(contact.Id, out var _id) ? _id : Guid.Empty;
            }

            xrm_contact.FirstName = contact.FirstName;
            xrm_contact.LastName = contact.LastName;
            xrm_contact.MobilePhone = contact.MobilePhone;
            xrm_contact.Salutation = contact.Salutation;
            xrm_contact[XrmHajirContact.Schema.Gifts] = contact.Hadaya;
            xrm_contact["jobtitle"] = contact.JobTitle;
            xrm_contact["telephone1"] = contact.BusinessPhone;
            xrm_contact["accountrolecode"] = GetRole(contact);
            xrm_contact.Telephone1 = contact.BusinessPhone;
            xrm_contact.EmailAddress1 = contact.Email;
            //xrm_contact["accountrolecode"] = GetRole(contact);
            xrm_contact["description"] = contact.Description;
            xrm_contact["donotemail"] = contact.DoNotEmail;
            xrm_contact["donotfax"] = contact.DoNotFax;
            xrm_contact["donotbulkemail"] = contact.DoNotBulkEmail;
            xrm_contact["donotphone"] = contact.DoNotPhone;
            xrm_contact["donotpostalmail"] = contact.DoNotPostalMail;
            xrm_contact[XrmContact.Schema.Address1_Line1] = contact.Address + " " + (contact.GetAttributeValue<string>(XrmContact.Schema.Address1_Line1) ?? "");
            xrm_contact[XrmContact.Schema.Address1_Line2] = contact.GetAttributeValue<string>(XrmContact.Schema.Address1_Line2);
            xrm_contact[XrmContact.Schema.Address1_Line3] = contact.GetAttributeValue<string>(XrmContact.Schema.Address1_Line3);
            xrm_contact[XrmContact.Schema.Fax] = contact.GetAttributeValue<string>(XrmContact.Schema.Fax);
            xrm_contact["address1_country"] = contact.GetAttributeValue<string>("address1_country");
            xrm_contact["address1_city"] = contact.City;
            xrm_contact["address1_stateorprovince"] = contact.Province;
            //
            xrm_contact[XrmHajirContact.Schema.SalutaionCode] = GetSalutaionCode(contact.Salutation);
            xrm_contact["address1_postalcode"] = contact.PostalCode;
            //address1_postalcode
            var city = this.GetCity(contact.City);
            if (city != null && Guid.TryParse(city.Id, out var cityId))
            {
                xrm_contact[XrmHajirContact.Schema.CityId] = new EntityReference(XrmHajirCity.Schema.LogicalName, cityId);
            }
            if (city.ProvinceId != null && Guid.TryParse(city.ProvinceId, out var provinceId))
            {
                xrm_contact[XrmHajirContact.Schema.ProvinceId] = new EntityReference(XrmHajirProvince.Schema.LogicalName, provinceId);
            }
            var country = this.GetCountry(contact.GetAttributeValue<string>("address1_country"))
                ?? this.cache.Countries.Where(x => x.Name == "ایران").FirstOrDefault();
            if (country != null && Guid.TryParse(country.Id, out var _countryid))
            {
                xrm_contact[XrmHajirContact.Schema.CountryId] = new EntityReference(XrmHajirCountry.Schema.LogicalName, _countryid);
            }
            if (city != null)
            {
                var province = this.cache.Provinces.FirstOrDefault(x => x.Id == city.ProvinceId);
                if (province != null && province.CountryId != null && Guid.TryParse(province.CountryId, out var countryId))
                {
                    xrm_contact[XrmHajirContact.Schema.CountryId] = new EntityReference(XrmHajirCountry.Schema.LogicalName, countryId);
                }
            }






            if (1 == 0)
            {
                xrm_contact["rhs_prefix"] = GetRhsSalutaion(contact.Salutation);
                xrm_contact["rhs_externalid"] = contact.Id;
                xrm_contact["rhs_prefix"] = GetRhsSalutaion(contact.Salutation);

                xrm_contact["jobtitle"] = contact.JobTitle;
                xrm_contact[XrmHajirContact.Schema.RHSAddress] = contact.Address;
                xrm_contact["rhs_businessphone1"] = contact.BusinessPhone;
                xrm_contact["rhs_address"] = contact.Address;
                xrm_contact["rhs_address2"] = contact.GetAttributeValue("address1_line1");
                xrm_contact["rhs_address3"] = contact.GetAttributeValue("address1_line2");
                xrm_contact["rhs_dateofbirth"] = contact.BirthDate;
                xrm_contact["rhs_gifts"] = contact.Hadaya;
                xrm_contact["rhs_call"] = !contact.DoNotPhone;
                xrm_contact["rhs_sendpost"] = !contact.DoNotPost;
                xrm_contact["rhs_sendmessage"] = !contact.DonotSendMarketingMaterial;
                xrm_contact["rhs_postalcode"] = contact.PostalCode;
                xrm_contact["rhs_internalphone"] = contact.GetAttributeValue<string>("thr_foriegnmobile");
                xrm_contact["rhs_sendemail"] = !contact.DoNotEmail;
                xrm_contact["rhs_sendfax"] = !contact.DoNotFax;
            }
            if (1 == 0)
            {



                var city_phone_code = this.GetCityPhoneCode(contact.MobilePhone, out var _phone);
                if (city_phone_code.HasValue)
                {
                    xrm_contact[XrmHajirContact.Schema.RHSCityPhoneCode] = new EntityReference(XrmHajirCityPhoneCode.Schema.LogicalName, city_phone_code.Value);
                    xrm_contact["rhs_businessphone1"] = _phone;
                }


                var owner = string.IsNullOrEmpty(contact.OwnerLoginName) ? null : this.cache.Users.FirstOrDefault(x => x.GetAttributeValue("domainname")?.ToLowerInvariant() == contact.OwnerLoginName?.ToLowerInvariant());
                if (owner != null && Guid.TryParse(owner.Id, out var _ownerid))
                {
                    xrm_contact.Owner = new EntityReference(XrmSystemUser.Schema.LogicalName, _ownerid);
                }
            }

            if (contact.AccontId != null && Guid.TryParse(contact.AccontId, out var __accountid))
            {
                xrm_contact[XrmContact.Schema.ParentCustomerId] = this.ImportEntityReference(new EntityReference(XrmAccount.Schema.LogicalName, __accountid));
            }
            var id = xrm_contact.Id;
            if (is_new)
            {
                this.dataServices
                .GetRepository<XrmContact>()
                .Insert(xrm_contact);
            }
            else
            {
                dataServices.GetXrmOrganizationService().SetState(xrm_contact, 0, 1);
                this.dataServices
                .GetRepository<XrmContact>()
                .Update(xrm_contact);

            }
            xrm_contact.Services.GetAnnotationService().UpdateObject("import_data", contact);
            this.dataServices.SetStates(contact, xrm_contact);
            try
            {

                this.dataServices.SetDates(XrmContact.Schema.LogicalName, id, contact.CreatedOn, contact.ModifiedOn);
                //this.dataServices.SetOwner("account", id, this.ResolveUserId(account, "ownerid"));// account.GetOwnerId());
                //this.dataServices.SetModifiedeBy("account", id, this.ResolveUserId(account, "createdby"), this.ResolveUserId(account, "modifiedby"));// account.GetCreatedBy(), account.GetModifiedBy());

                this.dataServices.SetOwner("contact", id, this.ResolveUserId(contact, "ownerid"));// contact.GetOwnerId());
                this.dataServices.SetModifiedeBy("contact", id, this.ResolveUserId(contact, "createdby"), this.ResolveUserId(contact, "modifiedby"));// contact.GetCreatedBy(), contact.GetModifiedBy());


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
        public IntegrationAccount LoadAccount(string accountId)
        {
            return accountId != null && Guid.TryParse(accountId, out var _id)
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
        public int? GetRelationType(string connectionType)
        {
            int? result = null;
            if (HajirCrmConstants.LegacyMaps.RelationShipMap.TryGetValue(connectionType, out var _res))
            {
                var item = this.GetRelationTypes()
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

        public int? GetIntroductionMethod(string connectionType)
        {
            int? result = null;
            if (connectionType != null && HajirCrmConstants.LegacyMaps.NahveAshnaeiMap.TryGetValue(connectionType, out var _res))
            {
                var item = this.GetIntroductionMethods()
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

        public Guid? ResolveUserId(DynamicEntity entity, string field)
        {
            if (entity == null)
                return null;

            var user = entity.GetAttributeValue<DynamicEntityReference>(field);
            if (user?.Name != null)
            {
                if (user.Name == "mehdi shafiee")
                {
                    return Guid.TryParse(this.cache.Users.First().Id, out var ___id) ? ___id : (Guid?)null;
                }
                var u = this.cache.Users;
                var __user = this.cache.Users.FirstOrDefault(x => x.FulleName == user.Name);
                if (__user != null && Guid.TryParse(__user.Id, out var __id))
                {
                    return __id;
                }

                var _user = this.dataServices.GetRepository<XrmSystemUser>()
                    .Queryable.FirstOrDefault(x => x.FullName == user.Name);
                if (_user != null)
                {
                    return _user.Id;
                }
            }
            return Guid.TryParse(user?.Id, out var id) ? id : (Guid?)null;

            return null;

            return entity.GetAttributeValue<DynamicEntityReference>("ownerid")?.Id != null &&
                Guid.TryParse(entity.GetAttributeValue<DynamicEntityReference>("ownerid")?.Id, out var _res)
                ? _res
                : (Guid?)null;
        }
        public IntegrationAccount ImportLegacyAccount(IntegrationAccount account)
        {
            var xrm_account = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.Id == account.GetId<Guid>()) ?? new XrmHajirAccount();
            var is_new = xrm_account.Id == Guid.Empty;
            if (is_new)
            {
                xrm_account.Id = Guid.TryParse(account.Id, out var _id) ? _id : Guid.Empty;
            }

            xrm_account.Name = account.Name?.RemoveArabic();
            xrm_account.Description = account.Description;

            xrm_account.IntroductionMethodCode = this.GetIntroductionMethod(account.Nahve_Ahnaei);
            xrm_account.AccountRatingOption = this.GetAccountRatingValue(account.Daraje_Ahamiat);
            xrm_account.ImportanceCode = this.GetDegreeImportance(account.Daraje_Ahamiat);

            xrm_account.AccountType = GetAccountType(account.gn_hesab_no);
            xrm_account.RelationType = GetRelationType(account.RelationShipType);


            xrm_account[XrmHajirAccount.Schema.NationalId] = account.gn_shenasemeli;
            xrm_account[XrmHajirAccount.Schema.RegistrationNumber] = account.gn_sabt;
            xrm_account[XrmHajirAccount.Schema.EconomicCode] = account.gn_eco_code;



            xrm_account.Telephone1 = account.MainPhone;
            xrm_account["telephone2"] = account.Telephone2;
            xrm_account[XrmHajirAccount.Schema.BrandName] = account.AccountNumber;
            xrm_account["address1_postalcode"] = account.address1_postalcode;
            xrm_account[XrmHajirAccount.Schema.WebSiteUrl] = account.WebSite;
            xrm_account[XrmHajirAccount.Schema.Email1] = account.Email;
            xrm_account.Address1_Line1 = account.address1_name;
            xrm_account.Address1_Line2 = account.GetAttributeValue("address1_line1");
            xrm_account.Address1_Line3 = account.GetAttributeValue("address1_line2");
            xrm_account.Address1_City = account.GetAttributeValue(XrmAccount.Schema.Address1_City);
            xrm_account["address1_country"] = account.GetAttributeValue("address1_country");
            xrm_account["donotbulkemail"] = account.DonotBulkEmail;
            xrm_account["donotemail"] = account.DonotEmail;
            xrm_account["donotfax"] = account.DoNotFax;
            xrm_account["donotphone"] = account.DoNotPhone;
            xrm_account["donotpostalmail"] = account.DoNotPostalMail;

            var city = this.GetCity(account.City);
            if (city != null && Guid.TryParse(city.Id, out var cityId))
            {
                xrm_account[XrmHajirAccount.Schema.CityId] = new EntityReference(XrmHajirCity.Schema.LogicalName, cityId);
            }
            if (city != null && city.ProvinceId != null && Guid.TryParse(city.ProvinceId, out var provinceId))
            {
                xrm_account[XrmHajirAccount.Schema.ProvinceId] = new EntityReference(XrmHajirProvince.Schema.LogicalName, provinceId);
            }
            var country = this.GetCountry(account.GetAttributeValue<string>("address1_country"))
                ?? this.cache.Countries.Where(x => x.Name == "ایران").FirstOrDefault();
            if (country != null && Guid.TryParse(country.Id, out var _countryid))
            {
                xrm_account[XrmHajirAccount.Schema.CountryId] = new EntityReference(XrmHajirCountry.Schema.LogicalName, _countryid);
            }
            if (city != null)
            {
                var province = this.cache.Provinces.FirstOrDefault(x => x.Id == city.ProvinceId);
                if (province != null && province.CountryId != null && Guid.TryParse(province.CountryId, out var countryId))
                {
                    xrm_account[XrmHajirAccount.Schema.CountryId] = new EntityReference(XrmHajirCountry.Schema.LogicalName, countryId);
                }

            }



            if (1 == 0)
            {
                //account.Daraje_Ahamiat

                //var country = this.GetCountry(account.GetAttributeValue<string>("address1_country"))
                //    ?? this.cache.Countries.Where(x => x.Name == "ایران").FirstOrDefault();

                //if (country != null && Guid.TryParse(country.Id, out var _countryid))
                //{
                //    xrm_account["rhs_country"] = new EntityReference(XrmHajirCountry.Schema.LogicalName, _countryid);
                //}

                //var city = this.GetCity(account.City);
                //if (city != null && Guid.TryParse(city.Id, out var cityId))
                //{
                //    xrm_account[XrmHajirAccount.Schema.rhs_city] = new EntityReference(XrmHajirCity.Schema.LogicalName, cityId);
                //}
                //if (city != null && Guid.TryParse(city.ProvinceId, out var provinceId))
                //{
                //    xrm_account[XrmHajirAccount.Schema.rhs_state] = new EntityReference(XrmHajirProvince.Schema.LogicalName, provinceId);
                //}
                var phoneCityCode = this.GetCityPhoneCode(account.MainPhone, out var _phone);
                if (phoneCityCode.HasValue)
                {
                    xrm_account[XrmHajirAccount.Schema.rhs_MainCityCode] = new EntityReference(XrmHajirCityPhoneCode.Schema.LogicalName, phoneCityCode.Value);
                    xrm_account[XrmHajirAccount.Schema.rhs_MainPhone] = _phone;
                }
            }

            //var owner = string.IsNullOrEmpty(account.OwningLoginName) ? null : this.cache.Users.FirstOrDefault(x => x.GetAttributeValue("domainname")?.ToLowerInvariant() == account.OwningLoginName?.ToLowerInvariant());
            //if (owner != null && Guid.TryParse(owner.Id, out var _ownerid))
            //{
            //    xrm_account.Owner = new EntityReference(XrmSystemUser.Schema.LogicalName, _ownerid);
            //}
            var parent = account.GetAttributeValue<DynamicEntityReference>("parentaccountid");

            if (parent != null)
            {

                xrm_account[XrmAccount.Schema.ParentAccount] = HajirXrmExtensions.ImportEntityReference(this.dataServices, parent.ToXrmEntityReference());
                //var parent_account = this.GetAccountByExternalId(parent.Id);
                //if (parent_account != null && Guid.TryParse(parent_account.Id, out var _parentid))
                //{
                //    xrm_account[XrmAccount.Schema.ParentAccount] = new EntityReference(XrmAccount.Schema.LogicalName, _parentid);
                //}

            }

            if (account.PrimaryContactId != null && Guid.TryParse(account.PrimaryContactId, out var __contactid))
            {
                xrm_account[XrmHajirAccount.Schema.PrimaryContactId] = HajirXrmExtensions
                    .ImportEntityReference(this.dataServices, new EntityReference(XrmContact.Schema.LogicalName, __contactid));
            }
            //var contact = this.GetContactByExternalId(account.PrimaryContactId);
            //if (contact != null && Guid.TryParse(contact.Id, out var _primaryContact))
            //{
            //    xrm_account[XrmHajirAccount.Schema.PrimaryContactId] = new EntityReference(XrmContact.Schema.LogicalName, _primaryContact);
            //}
            var id = xrm_account.Id;
            if (is_new)
            {
                this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Insert(xrm_account);
            }
            else
            {
                dataServices.GetXrmOrganizationService().SetState(xrm_account, 0, 1);
                this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Update(xrm_account);

            }
            xrm_account = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.AccountId == id);

            this.dataServices.SetStates(account, xrm_account);
            xrm_account.Services.GetAnnotationService().UpdateObject("import_data", account);

            try
            {
                this.dataServices.SetDates("account", id, account.CreatedOn, account.ModifiedOn);
                this.dataServices.SetOwner("account", id, this.ResolveUserId(account, "ownerid"));// account.GetOwnerId());
                this.dataServices.SetModifiedeBy("account", id, this.ResolveUserId(account, "createdby"), this.ResolveUserId(account, "modifiedby"));// account.GetCreatedBy(), account.GetModifiedBy());

            }
            catch (Exception exp)
            {
                this.logger.LogWarning(
                    $"An error occured while tryng to set CreatedOn,ModifiedOn on this account. Err:{exp.Message} ");
            }
            return LoadAccount(id.ToString());
        }

        public IntegrationAccount GetAccountByExternalId(string externalId)
        {
            return !string.IsNullOrWhiteSpace(externalId) && Guid.TryParse(externalId, out var _id)
                ? this.dataServices.GetRepository<XrmHajirAccount>()
                        .Queryable
                        .FirstOrDefault(x => x.Id == _id)
                        .ToIntegrationAccount()
                : null;


            return string.IsNullOrWhiteSpace(externalId) ? null : this.dataServices.GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == externalId)
                .ToIntegrationAccount();
        }
        public IntegrationContact GetContactByExternalId(string externalId)
        {
            return !string.IsNullOrWhiteSpace(externalId) && Guid.TryParse(externalId, out var _id)

                ? this.dataServices.GetRepository<XrmContact>()
                        .Queryable
                        .FirstOrDefault(x => x.Id == _id)
                        .ToIntegrationContact()
                : null;
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

            var is_new = line.Id == Guid.Empty;
            if (is_new)
            {
                line.Id = _id;
            }
            line.QuoteId = quoteid;
            line.SetAttribiuteValue("isproductoverridden", true);
            var productId = product.GetAttributeValue<DynamicEntityReference>(XrmQuoteDetail.Schema.ProductId);
            line.ProductDescription = productId == null
                ? product.GetAttributeValue(XrmQuoteDetail.Schema.ProductDescription)
                : productId.Name;
            line.Quantity = product.GetAttributeValue<decimal?>(XrmQuoteDetail.Schema.Quantity);
            line.PercentTax = product.GetAttributeValue<int?>("gn_taxpercent");
            //line.SetAttribiuteValue(XrmHajirQuoteDetail.Schema.PercentTax, product.GetAttributeValue<int?>("gn_taxpercent"));
            //line.SetAttribiuteValue("priceperunit", product.GetAttributeValue<double>("priceperuint"));
            //line.SetAttribiuteValue("priceperunit_base", product.GetAttributeValue<double>("priceperuint_base"));
            line.PricePerUnit = product.GetAttributeValue<decimal?>(XrmQuoteDetail.Schema.PricePerUnit);
            line.Tax = product.GetAttributeValue<decimal?>(XrmQuoteDetail.Schema.Tax);
            line.ManualDiscountAmount = product.GetAttributeValue<decimal?>(XrmQuoteDetail.Schema.ManualDiscountAmount);




            if (is_new)
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
                .FirstOrDefault(x => x.QuoteId == quote.GetId<Guid>()) ?? new XrmHajirQuote();
            var is_new = xrm_quote.Id == Guid.Empty;
            if (is_new)
            {
                xrm_quote.Id = Guid.TryParse(quote.Id, out var _id) ? _id : Guid.Empty;
            }
            DateTime modifiedOn = quote.ModifiedOn.Value;
            DateTime createdOn = quote.CreatedOn.Value;
            if (quote.AccountId != null)
            {
                var account = this.GetAccountByExternalId(quote.AccountId);
                xrm_quote.AccountId = Guid.TryParse(account?.Id, out var _t) ? _t : (Guid?)null;
            }
            //xrm_quote.ExternalId = quote.Id;
            xrm_quote.Name = quote.GetAttributeValue("name");
            xrm_quote.QuoteNumber = quote.GetAttributeValue("quotenumber");
            if (is_new)
            {
                var existing = this.dataServices.GetRepository<XrmHajirQuote>()
                    .Queryable
                    .FirstOrDefault(x => x.QuoteNumber == xrm_quote.QuoteNumber) != null;
                if (existing)
                {
                    xrm_quote.QuoteNumber = xrm_quote.QuoteNumber + (new Random().Next(1, 100)).ToString();
                }
            }
            //xrm_quote[XrmHajirQuote.Schema.HajirQuoteId] = xrm_quote.QuoteNumber;
            xrm_quote[XrmHajirQuote.Schema.Remarks] = quote.GetAttributeValue<string>(XrmQuote.Schema.Description);
            xrm_quote[XrmQuote.Schema.Description] = quote.GetAttributeValue<string>(XrmQuote.Schema.Description);
            xrm_quote["statecode"] = new OptionSetValue(0); // Draft
            xrm_quote["statuscode"] = new OptionSetValue(1);
            //var _stateCode = quote.GetAttributeValue<int?>(XrmEntity.Schema.StateCode);
            //var owner = string.IsNullOrEmpty(quote.OwningLoginName) ? null : this.cache.Users.FirstOrDefault(x => x.GetAttributeValue("domainname")?.ToLowerInvariant() == quote.OwningLoginName?.ToLowerInvariant());
            //if (owner != null && Guid.TryParse(owner.Id, out var _ownerid))
            //{
            //    xrm_quote.Owner = new EntityReference(XrmSystemUser.Schema.LogicalName, _ownerid);
            //}


            Guid id = xrm_quote.Id;
            if (is_new)
            {
                this.dataServices
                 .GetRepository<XrmQuote>()
                 .Insert(xrm_quote);
            }
            else
            {
                dataServices.GetXrmOrganizationService().SetState(xrm_quote, 0, 1);
                this.dataServices
                 .GetRepository<XrmQuote>()
                 .Update(xrm_quote);
            }



            //this.dataServices.GetXrmOrganizationService().SetState(xrm_quote,
            //    quote.GetAttributeValue<int>(XrmEntity.Schema.StateCode),
            //    quote.GetAttributeValue<int>(XrmEntity.Schema.StatusCode), true);


            foreach (var line in quote.Products)
            {
                ImportLegacryQuoteProduct(line, id);
            }
            var _stateCode = quote.GetAttributeValue<int?>(XrmEntity.Schema.StateCode);
            var _statusCode = quote.GetAttributeValue<int>(XrmEntity.Schema.StatusCode);
            if (_stateCode == (int)XrmQuote.Schema.QuoteStateCodes.Won)
            {
                this.dataServices.GetXrmOrganizationService().SetState(xrm_quote, 1, 2);
                this.dataServices.GetXrmOrganizationService().GetOrganizationService().Execute(new WinQuoteRequest
                {

                    QuoteClose = new Entity("quoteclose")
                    {
                        Attributes = {
                            {"subject", $"Quote Close {DateTime.Now}" },
                            {"quoteid",xrm_quote.ToEntityReference() }                    }
                    },
                    Status = new OptionSetValue(_statusCode)
                });
            }
            else if (_stateCode == (int)XrmQuote.Schema.QuoteStateCodes.Closed)
            {
                this.dataServices.GetXrmOrganizationService().SetState(xrm_quote, 1, 2);


                this.dataServices.GetXrmOrganizationService().GetOrganizationService().Execute(new CloseQuoteRequest
                {

                    QuoteClose = new Entity("quoteclose")
                    {
                        Attributes = {
                        {"quoteid", xrm_quote.ToEntityReference() },
                        {"subject", "Quote Close " + DateTime.Now.ToString()}}
                    },

                    Status = new OptionSetValue(_statusCode)

                });

            }

            try
            {
                this.dataServices.SetDates(XrmQuote.Schema.LogicalName, id, createdOn, modifiedOn);
                this.dataServices.SetOwner(XrmQuote.Schema.LogicalName, id, this.ResolveUserId(quote, "ownerid"));// quote.GetOwnerId());
                this.dataServices.SetModifiedeBy(XrmQuote.Schema.LogicalName, id, this.ResolveUserId(quote, "createdby"), this.ResolveUserId(quote, "modifiedby"));// quote.GetCreatedBy(), quote.GetModifiedBy());
            }
            catch (Exception err)
            {
                Console.WriteLine($"$$$ An error occured while trying to db update quote:{err.Message}");
            }


            return LoadQuote(id.ToString());

        }

        private EntityReference GetDefaultBussinessUnit()
        {
            if (this._defualtBusinessUnit == null)
            {
                this._defualtBusinessUnit = this.dataServices
                    .GetXrmOrganizationService()
                    .CreateQuery("businessunit")
                    .ToArray()
                    .FirstOrDefault()
                    .ToEntityReference();
            }
            return this._defualtBusinessUnit;
        }
        private EntityReference GetDefaultSecurityRole()
        {
            if (this._defualtRole == null)
            {
                this._defualtRole = this.dataServices
                    .GetXrmOrganizationService()
                    .CreateQuery("role")
                    .ToArray()
                    .FirstOrDefault(x => x.GetAttributeValue<string>("name") == "Sales Manager")
                    .ToEntityReference();

                //this._defualtRole =  this.dataServices
                //    .GetXrmOrganizationService()
                //    .CreateQuery("role")
                //    .ToArray()
                //    .FirstOrDefault()
                //    .ToEntityReference();
            }
            return this._defualtRole;
        }
        public IntegrationUser ImportLegacyUser(IntegrationUser user)
        {
            if (user.FullName == "mehdi shafiee")
            {
                return this.cache.Users.First()?.To<IntegrationUser>();
            }
            var xrmUser = this.dataServices
                .GetRepository<XrmSystemUser>()
                .Queryable
                .FirstOrDefault(x => x.Id == user.GetId<Guid>())
                ?? this.dataServices.GetRepository<XrmSystemUser>()
                .Queryable
                .FirstOrDefault(x => x.FullName == user.FullName)
                ?? new XrmSystemUser();
            var is_new = xrmUser.Id == Guid.Empty;
            if (is_new)
            {
                xrmUser.Id = user.GetId<Guid>();
            }
            if (xrmUser.Id == Guid.Empty)
            {
                throw new Exception("Invalid User Id");
            }
            xrmUser.FirstName = user.GetAttributeValue(XrmSystemUser.Schema.FirstName);
            xrmUser.LastName = user.GetAttributeValue(XrmSystemUser.Schema.LastName);
            xrmUser.DomainName = user.GetAttributeValue(XrmSystemUser.Schema.DomainName);
            xrmUser["businessunitid"] = GetDefaultBussinessUnit();
            GetDefaultSecurityRole();

            if (is_new)
            {
                this.dataServices
                    .GetRepository<XrmSystemUser>()
                    .Insert(xrmUser);
            }
            else
            {
                this.dataServices
                    .GetRepository<XrmSystemUser>()
                    .Update(xrmUser);

            }
            // https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/sample-associate-security-role-user?view=op-9-1

            this.dataServices.GetXrmOrganizationService().GetOrganizationService().Associate(
                                   "systemuser",
                                   xrmUser.Id,
                                   new Relationship("systemuserroles_association"),
                                   new EntityReferenceCollection() { GetDefaultSecurityRole() });

            return xrmUser.ToDynamic()
                .To<IntegrationUser>();


        }

        public IntegrationUser GetUserById(string id)
        {

            if (id != null && Guid.TryParse(id, out var _id))
            {
                return this.dataServices
                .GetRepository<XrmSystemUser>()
                .Queryable
                .FirstOrDefault(x => x.Id == _id)?
                .ToDynamic().To<IntegrationUser>();
            }
            return null;
        }

        public IntegrationUser GetUserByFullName(string fullName)
        {
            if (fullName == "mehdi shafiee")
            {
                return this.cache.Users.First()?.To<IntegrationUser>();
            }
            return string.IsNullOrWhiteSpace(fullName)
                ? null
                : this.dataServices
                .GetRepository<XrmSystemUser>()
                .Queryable
                .FirstOrDefault(x => x.FullName == fullName)?
                .ToDynamic().To<IntegrationUser>();

        }

        public void ImportGeoData(GeoData geoData)
        {

            this.logger.LogInformation($"Importing GeoData");

            var jobTitles = this.dataServices.GetRepository<XrmHajirJobTitle>()
               .Queryable
               .Select(x => x.Name)
               .ToArray();

            foreach (var jobTitle in HajirCrmConstants.LegacyMaps.JobTitles
                .Split('\n')
                .Where(x => x != null)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => !jobTitles.Contains(x)))
            {
                this.dataServices.GetRepository<XrmHajirJobTitle>()
                       .Upsert(new XrmHajirJobTitle { Name = jobTitle });
            };
            var industries = this.dataServices.GetRepository<XrmHajirIndustry>()
               .Queryable
               .Select(x => x.Name)
               .ToArray();


            foreach (var ind in HajirCrmConstants.LegacyMaps.Industries
                .Split('\n')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => !industries.Contains(x)))
            {
                try
                {
                    this.dataServices.GetRepository<XrmHajirIndustry>()
                        .Upsert(new XrmHajirIndustry { Name = ind });
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        $"An error occcured while trying to update industry. Err:{err.Message}");
                }
                if (!industries.Contains(ind))
                {

                }
            }
            var countries = this.dataServices
                .GetRepository<XrmHajirCountry>()
                .Queryable
                .Select(x => x.Id)
                .ToArray();
            geoData
                .Countries
                .Where(x => !countries.Contains(x.Id))
                .ToList()
                .ForEach(country =>
                {
                    try
                    {
                        this.dataServices
                            .GetRepository<XrmHajirCountry>()
                            .Insert(new XrmHajirCountry
                            {
                                Id = country.Id,
                                Name = country.Name
                            });
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError($"An erro occured while trying to insert country. err:{err.Message}");
                    }
                });
            var provinces = this.dataServices
                .GetRepository<XrmHajirProvince>()
                .Queryable
                .Select(x => x.Id)
                .ToArray();
            geoData
                .Provinces
                .Where(x => !provinces.Contains(x.Id))
                .ToList()
                .ForEach(prov =>
                {
                    try
                    {
                        this.dataServices
                        .GetRepository<XrmHajirProvince>()
                        .Insert(new XrmHajirProvince
                        {
                            Id = prov.Id,
                            Name = prov.Name,
                            Code = prov.Code,
                            Country = new EntityReference(XrmHajirCountry.Schema.LogicalName, geoData.Countries.FirstOrDefault(x => x.Name == "ایران").Id),
                            //CenterCityId = prov.CenterCityId.HasValue
                            //    ? new EntityReference(XrmHajirCity.Schema.LogicalName, prov.CenterCityId.Value)
                            //    : null
                        }); ;
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to update province. Err:{err.Message}");
                    }
                });
            var cities = this.dataServices
                .GetRepository<XrmHajirCity>()
                .Queryable
                .Select(x => x.Id)
                .ToArray();
            geoData
                .Cities
                .Where(x => !cities.Contains(x.Id))
                .ToList()
                .ForEach(city =>
                {
                    try
                    {
                        this.dataServices
                        .GetRepository<XrmHajirCity>()
                        .Insert(new XrmHajirCity
                        {
                            Id = city.Id,
                            Name = city.Name,
                            Code = city.Code,
                            ProvinceReference = city.ProvinceId.HasValue
                                ? new EntityReference(XrmHajirProvince.Schema.LogicalName, city.ProvinceId.Value)
                                : null
                        });
                    }
                    catch (Exception err)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to insert city. err:{err.Message}");
                    }



                });

            this.dataServices
                .GetRepository<XrmHajirProvince>()
                .Queryable
                .Where(x => x.CenterCityId == null)
                .ToList()
                .ForEach(p =>
                {
                    var prov = geoData.Provinces.FirstOrDefault(x => x.Id == p.Id);
                    p.CenterCityId = prov != null && prov.CenterCityId.HasValue
                        ? new EntityReference(XrmHajirCity.Schema.LogicalName, prov.CenterCityId.Value)
                        : null;
                    this.dataServices.GetRepository<XrmHajirProvince>().Update(p);
                });

            var categories = this.dataServices
                .GetRepository<XrmHajirProductCategory>()
                .Queryable
                .ToArray();
            HajirCrmConstants.LegacyMaps
                .DefaultProductCategories
                .ToList()
                .ForEach(x =>
                {

                    if (!categories.Any(cat => cat.Code == x.Code))
                    {
                        var type = HajirCrmConstants.Schema.Product.GetProductTypeFromProductCategory(HajirCrmConstants.Schema.Product.IntToProductCategory(x.Code));
                        var _cat = new XrmHajirProductCategory
                        {
                            Code = x.Code,
                            Name = x.Name,
                            ProductType = type
                        };
                        this.dataServices
                        .GetRepository<XrmHajirProductCategory>()
                        .Upsert(_cat);
                    }


                });



            this.logger.LogInformation($"Finished Importing GeoData");
        }

        public IEnumerable<IntegrationQuote> GetQuotesReadyToIntgrate(int skip, int take)
        {
            return this.dataServices
                .GetRepository<XrmHajirQuote>()
                .Queryable
                .Where(x => x.QuoteNumber == "QUO-01000-H1S9D1")
                .ToArray()
                .Select(x => LoadQuote(x.QuoteId.ToString()))
                .ToArray();




        }

        public IntegrationAccount LoadAccountById(string accountId)
        {
            return this.LoadAccount(accountId);
        }

        public IntegrationQuote LoadQuoteById(string quoteId)
        {
            return this.LoadQuote(quoteId);
        }

        public IntegrationProduct LoadProcuct(string productId)
        {
            throw new NotImplementedException();
        }
    }
}
