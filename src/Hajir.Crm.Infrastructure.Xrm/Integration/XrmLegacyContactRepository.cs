using GN.Library.Xrm;
using Hajir.Crm.Features.Integration;
using Hajir.Crm.Features.Integration.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

using System.Text;
using System.Linq;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using GN.Library.Xrm.StdSolution;

namespace Hajir.Crm.Infrastructure.Xrm.Integration
{
    internal class XrmLegacyContactRepository : ILegacyCrmStore
    {
        internal class PicklistValue
        {
            public int Code { get; set; }
            public string Name { get; set; }

        }
        private List<PicklistValue> _salutions;
        private List<PicklistValue> _accounttypes;
        XrmOrganizationService organizationService;
        private Lazy<List<Entity>> Users;
        public XrmLegacyContactRepository(HajirIntegrationOptions options)
        {

            this.organizationService = new GN.Library.Xrm.XrmOrganizationService(new XrmConnectionString(options.LegacyConnectionString));
            this.Users = new Lazy<List<Entity>>(() =>
            {

                return this.organizationService.CreateQuery("systemuser")
                .ToList();

            }, true);
        }

        public void Dispose()
        {
            this.organizationService?.Dispose();

        }

        public int GetContatCount()
        {
            var query = this.organizationService.CreateQuery("contact");
            var a = query.Select(x => x["contactid"]).ToArray();
            return a.Length;
            return query.Count();
        }
        private List<PicklistValue> GetOptionSetData(string entityName, string propertyName)
        {
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = propertyName,
                RetrieveAsIfPublished = false
            };
            var s = this.organizationService.GetOrganizationService();
            RetrieveAttributeResponse response = s.Execute(attributeRequest) as RetrieveAttributeResponse;

            EnumAttributeMetadata attributeData = (EnumAttributeMetadata)response.AttributeMetadata;

            var optionList = (from option in attributeData.OptionSet.Options
                              select new PicklistValue { Code = option.Value ?? 0, Name = option.Label.UserLocalizedLabel.Label })
              .ToList();
            return optionList;
        }
        private List<PicklistValue> GetSalutions()
        {
            if (this._salutions == null)
            {
                this._salutions = this.GetOptionSetData("contact", "gn_salutation");
            }
            return this._salutions;

        }
        private List<PicklistValue> GetAccountTpes()
        {
            if (this._accounttypes == null)
            {
                this._accounttypes = this.GetOptionSetData("account", "gn_hesab_no");
            }
            return this._accounttypes;

        }
        public string GetSalution(Entity e)
        {
            var opt = e?.GetAttributeValue<OptionSetValue>("gn_salutation");
            return opt != null ? this.GetSalutions().FirstOrDefault(x => x.Code == opt.Value)?.Name : null;

        }
        public string GetAccountType(Entity e)
        {
            return e.GetFormattedValue("gn_hesab_no");
            var opt = e?.GetAttributeValue<OptionSetValue>("gn_hesab_no");
            return opt != null ? this.GetAccountTpes().FirstOrDefault(x => x.Code == opt.Value)?.Name : null;

        }
        public string GetRole(Entity e)
        {
            return e.GetFormattedValue("accountrolecode");
            var opt = e?.GetAttributeValue<OptionSetValue>("accountrolecode");
            return opt != null ? this.GetSalutions().FirstOrDefault(x => x.Code == opt.Value)?.Name : null;

        }
        public string GetAccountId(Entity e)
        {
            var a = e.GetAttributeValue<EntityReference>("parentcustomerid");
            return a != null && a.LogicalName == "account" ? a.Id.ToString() : null;
        }
        public string GetOwnerLoginName(Entity e)
        {
            var owner = e.GetAttributeValue<EntityReference>("ownerid");
            var user = this.Users.Value.FirstOrDefault(x => x.Id == owner.Id);
            return user?.GetAttributeValue<string>("domainname");

        }

        private IntegrationContact ToContact(Entity x)
        {
            return x == null ? null : new IntegrationContact(x.Attributes)
            {
                Id = x.GetAttributeValue<Guid>("contactid").ToString(),
                FirstName = x.GetAttributeValue<string>("firstname")?.RemoveArabic(),
                LastName = x.GetAttributeValue<string>("lastname")?.RemoveArabic(),
                MobilePhone = x.GetAttributeValue<string>("mobilephone"),
                City = x.GetAttributeValue<string>("address1_city")?.RemoveArabic(),
                Salutation = this.GetSalution(x)?.RemoveArabic(),
                JobTitle = x.GetAttributeValue<string>("jobtitle"),
                Province = x.GetAttributeValue<EntityReference>("gn_province")?.Name,
                Hadaya = x.GetAttributeValue<bool?>("new_hadaya"),
                Role = GetRole(x),
                AccontId = GetAccountId(x),
                Address = x.GetAttributeValue<string>("address1_name"),
                BusinessPhone = x.GetAttributeValue<string>("telephone1"),
                Email = x.GetAttributeValue<string>(XrmContact.Schema.EmailAddress1),
                //OwnerLoginName = GetOwnerLoginName(x),
                ModifiedOn = x.GetAttributeValue<DateTime>(XrmEntity.Schema.ModifiedOn),
                CreatedOn = x.GetAttributeValue<DateTime>(XrmEntity.Schema.CreatedOn),
                State = x.GetAttributeValue<OptionSetValue>("statecode")?.Value ?? 0,
                BirthDate = x.GetAttributeValue<DateTime?>("birthdate"),
                Description = x.GetAttributeValue<string>("description"),
                DoNotEmail = x.GetAttributeValue<bool?>("donotemail") ?? false,
                DoNotBulkEmail = x.GetAttributeValue<bool?>("donotbulkemail") ?? false,
                DoNotFax = x.GetAttributeValue<bool?>("donotfax") ?? false,
                DoNotPhone = x.GetAttributeValue<bool?>("donotphone") ?? false,
                DoNotPost = x.GetAttributeValue<bool?>("new_dontpost") ?? false,
                DoNotPostalMail = x.GetAttributeValue<bool?>("donotpostalmail") ?? false,
                DonotSendMarketingMaterial = x.GetAttributeValue<bool?>("donotsendmm") ?? false,
                PostalCode = x.GetAttributeValue<string>("address1_postalcode"),
                


            };
        }

        public IEnumerable<IntegrationContact> ReadContacts(int skip, int take)
        {
            var query = this.organizationService.CreateQuery("contact");
            return query.Skip(skip).Take(take).ToArray()
                .Select(x => ToContact(x))
                .ToArray();
        }

        private IntegrationAccount ToAccount(Entity entity)
        {
            if (entity == null)
            {
                return null;
            }
            var result = entity.ToDynamic().To<IntegrationAccount>();
            result.Id = entity.Id.ToString();
            result.Name = ((string)entity[XrmAccount.Schema.Name]).RemoveArabic();
            result.AccountNumber = entity.GetAttributeValue<string>("accountnumber");
            result.MainPhone = entity.GetAttributeValue<string>("telephone1");
            result.Telephone2 = entity.GetAttributeValue<string>("telephone2");
            result.Fax = entity.GetAttributeValue<string>("fax");
            result.gn_shenasemeli = entity.GetAttributeValue<string>("gn_shenasemeli");
            result.gn_sabt = entity.GetAttributeValue<string>("gn_sabt");
            result.gn_eco_code = entity.GetAttributeValue<string>("gn_eco_code");
            result.address1_name = entity.GetAttributeValue<string>("address1_name");
            //gn_hesab_no
            result.gn_hesab_no = GetAccountType(entity);
            result.City = entity.GetAttributeValue<string>("address1_city");
            result.address1_postalcode = entity.GetAttributeValue<string>("address1_postalcode");
            result.Daraje_Ahamiat = entity.GetFormattedValue("gn_type").RemoveArabic();
            result.Nahve_Ahnaei = entity.GetFormattedValue("gn_ashnayi").RemoveArabic();
            result.Category = entity.GetFormattedValue("accountcategorycode").RemoveArabic();
            result.Industry = entity.GetFormattedValue("industrycode").RemoveArabic();
            result.RelationShipType = entity.GetFormattedValue("customertypecode").RemoveArabic();
            result.Description = entity.GetAttributeValue<string>("description");
            result.State = entity.GetAttributeValue<OptionSetValue>("statecode")?.Value ?? 0;
            result.DonotBulkEmail = entity.GetAttributeValue<bool?>("donotbulkemail") ?? false;
            result.DoNotFax = entity.GetAttributeValue<bool?>("donotfax") ?? false;
            result.DonotEmail = entity.GetAttributeValue<bool?>("donotemail") ?? false;
            result.DoNotPhone = entity.GetAttributeValue<bool?>("donotphone") ?? false;
            result.DoNotPost = entity.GetAttributeValue<bool?>("new_dontpost") ?? false;
            result.DoNotPostalMail = entity.GetAttributeValue<bool?>("donotpostalmail") ?? false;
            result.WebSite = entity.GetAttributeValue<string>("websiteurl");
            result.Email = entity.GetAttributeValue<string>("emailaddress1");
            result.PrimaryContactId = entity.GetAttributeValue<EntityReference>("primarycontactid")?.Id.ToString();
            result.CreatedOn = entity.GetAttributeValue<DateTime>(XrmEntity.Schema.CreatedOn);
            result.ModifiedOn = entity.GetAttributeValue<DateTime>(XrmEntity.Schema.ModifiedOn);



            return result;

            return new IntegrationAccount
            {
                Id = entity.Id.ToString(),
                Name = (string)entity[XrmAccount.Schema.Name],
                MainPhone = entity.GetAttributeValue<string>("telephone1"),
                Fax = entity.GetAttributeValue<string>("fax"),
                gn_shenasemeli = entity.GetAttributeValue<string>("gn_shenasemeli"),
                gn_sabt = entity.GetAttributeValue<string>("gn_sabt"),
                gn_eco_code = entity.GetAttributeValue<string>("gn_eco_code"),
                address1_name = entity.GetAttributeValue<string>("address1_name"),
                //gn_hesab_no
                gn_hesab_no = GetAccountType(entity),
                City = entity.GetAttributeValue<string>("address1_city"),
                address1_postalcode = entity.GetAttributeValue<string>("address1_postalcode"),
                Daraje_Ahamiat = entity.GetFormattedValue("gn_type"),
                Nahve_Ahnaei = entity.GetFormattedValue("gn_ashnayi"),
                Category = entity.GetFormattedValue("accountcategorycode"),



            };
        }
        public IEnumerable<IntegrationAccount> ReadAccounts(int skip, int take)
        {
            var query = this.organizationService.CreateQuery("account");
            return query.Skip(skip).Take(take).ToArray()
               .Select(x => ToAccount(x))
               .ToArray();
        }

        public IntegrationAccount GetAccount(string id)
        {
            return id != null && Guid.TryParse(id, out var _id)
                ? ToAccount(this.organizationService.CreateQuery("account").FirstOrDefault(x => (Guid)x[XrmAccount.Schema.AccountId] == _id))
                : null;

        }

        public int GetAccountsCount()
        {
            var query = this.organizationService.CreateQuery("account");
            var a = query.Select(x => x["accountid"]).ToArray();
            return a.Length;
        }

        public IntegrationQuote ToQuote(Entity q)
        {
            var result = q.ToDynamic().To<IntegrationQuote>();
            return result;
        }
        public IntegrationQuoteProduct ToQuoteProduct(Entity e)
        {
            var result = e.ToDynamic().To<IntegrationQuoteProduct>();

            return result;
        }
        public IntegrationQuote LoadLines(IntegrationQuote quote)
        {
            if (Guid.TryParse(quote.Id, out var _id))
            {
                this.organizationService
                    .CreateQuery(XrmQuoteDetail.Schema.LogicalName)
                    .Where(x => (Guid)x["quoteid"] == _id)
                    .ToArray()
                    .Select(x => ToQuoteProduct(x))
                    .ToList()
                    .ForEach(x => quote.AddProducts(x));
            }
            return quote;
        }
        public IEnumerable<IntegrationQuote> ReadQuotes(int skip, int take)
        {
            var query = this.organizationService.CreateQuery(XrmQuote.Schema.LogicalName);
            var result = query.Skip(10).Take(10)
               .ToArray()
               .Select(x => ToQuote(x))
               .Select(x => LoadLines(x))
               .ToArray();
            return result;

        }

        public IntegrationContact GetContact(string id)
        {
            return id != null && Guid.TryParse(id, out var _id)
               ? ToContact(this.organizationService.CreateQuery("contact").FirstOrDefault(x => (Guid)x[XrmContact.Schema.ContactId] == _id))
               : null;

        }
    }
}
