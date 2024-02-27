using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Integration;
using Hajir.Crm.Features.Integration.Infrastructure;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
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

        internal class PicklistValue
        {
            public int Code { get; set; }
            public string Name { get; set; }
            public int Distance { get; set; }
            public Guid GUID { get; set; }

        }
        private List<PicklistValue> _salutions;
        private List<PicklistValue> _accountTypes;


        public XrmIntegrationStore(IXrmDataServices dataServices, ICacheService cache)
        {
            this.dataServices = dataServices;
            this.cache = cache;
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
        private Guid? GetIndustry(string value)
        {
            var f = this.cache.Industries
                .Select(x => new PicklistValue
                {
                    GUID = Guid.TryParse(x.Id, out var _i) ? _i : Guid.Empty,
                    Name = x.Name,
                    Distance = CalcLevenshteinDistance(x.Name, value)
                })
                .OrderBy(x => x.Distance)
                .Where(x => x.Distance < value.Length / 2)
                .ToArray()
                .FirstOrDefault();


            return f == null ? (Guid?)null : f.GUID;

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

        public IntegrationContact ImportLegacyContact(IntegrationContact contact)
        {
            var xrm_contact = this.dataServices.GetXrmOrganizationService()
                .CreateQuery<XrmContact>()
                .FirstOrDefault(x => (string)x["rhs_externalid"] == contact.Id) ?? new XrmContact();
            xrm_contact.FirstName = contact.FirstName;
            xrm_contact.LastName = contact.LastName;
            xrm_contact["rhs_externalid"] = contact.Id;
            xrm_contact["rhs_prefix"] = GetSalutaion(contact.Salutation);
            xrm_contact.MobilePhone = contact.MobilePhone;
            xrm_contact["jobtitle"] = contact.JobTitle;
            xrm_contact[XrmHajirContact.Schema.RHSAddress] = contact.Address;
            xrm_contact["telephone1"] = contact.BusinessPhone;
            xrm_contact.Telephone1 = contact.BusinessPhone;
            xrm_contact.EmailAddress1 = contact.Email;


            var account = this.GetAccountByExternalId(contact.AccontId);
            if (account != null && Guid.TryParse(account.Id, out var acc_id))
            {
                xrm_contact[XrmContact.Schema.ParentCustomerId] = new EntityReference(XrmAccount.Schema.LogicalName, acc_id);
            }


            var id = this.dataServices
                .GetRepository<XrmContact>()
                .Upsert(xrm_contact);
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
        public IntegrationAccount ImportLegacyAccount(IntegrationAccount account)
        {

            //var xrm_account1 = this.dataServices.GetXrmOrganizationService()
            //   .CreateQuery<XrmAccount>()
            //   .FirstOrDefault(x => (string)x[XrmHajirAccount.Schema.ExternalId] == account.Id) ?? new XrmAccount();
            //var xrm_account = xrm_account1.ToEntity<XrmHajirAccount>();
            var xrm_account = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == account.Id) ?? new XrmHajirAccount();
            xrm_account.Name = account.Name;
            xrm_account[XrmHajirAccount.Schema.ExternalId] = account.Id;
            xrm_account.AccountType = GetAccountType(account.gn_hesab_no);
            xrm_account.IndustryId = GetIndustry(account.Industry);
            var desc = xrm_account.Description;
            /// Fixing Description
            /// 
            var import_start = "==== Import Start ====";
            if (!string.IsNullOrWhiteSpace(desc))
            {

                var idx1 = desc.IndexOf(import_start);
                if (idx1 > -1)
                {
                    desc = desc.Remove(idx1);
                }
            }


            xrm_account.Description = desc + import_start + "\r\n" + account.GetImportantIntegrationValuesAsText();

            var id = this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Upsert(xrm_account);
            return LoadAccount(id.ToString());



        }

        public IntegrationAccount GetAccountByExternalId(string externalId)
        {
            return string.IsNullOrWhiteSpace(externalId) ? null : this.dataServices.GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.ExternalId == externalId)
                .ToIntegrationAccount();
        }
    }
}
