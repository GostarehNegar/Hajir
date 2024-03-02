using GN.Library.Xrm.StdSolution;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmHajirAccount : XrmAccount
    {
        public new class Schema : XrmAccount.Schema
        {
            public const string ExternalId = "rhs_externalid";
            public const string Description = "description";
            public const string rhs_companytype = "rhs_companytype";
            public const string rhs_industry = "rhs_industry";
            public const string rhs_connectiontype = "rhs_connectiontype";
            public const string MethodIntroduction = "rhs_methodintroduction";
        }

        [AttributeLogicalName(Schema.ExternalId)]
        public string ExternalId
        {
            get
            {

                return this.GetAttributeValue<string>(Schema.ExternalId);
            }
            set
            {
                this.SetAttributeValue(Schema.ExternalId, value);
            }
        }
        [AttributeLogicalName(Schema.Description)]
        public string Description
        {
            get
            {

                return this.GetAttributeValue<string>(Schema.Description);
            }
            set
            {
                this.SetAttributeValue(Schema.Description, value);
            }
        }

        [AttributeLogicalName(Schema.rhs_companytype)]
        public OptionSetValue AccountType
        {
            get
            {

                return this.GetAttributeValue<OptionSetValue>(Schema.rhs_companytype);
            }
            set
            {
                this.SetAttributeValue(Schema.rhs_companytype, value);
            }
        }
        [AttributeLogicalName(Schema.rhs_companytype)]
        public int? AccountTypeCode
        {
            get
            {

                return this.GetAttributeValue<OptionSetValue>(Schema.rhs_companytype)?.Value;
            }
            set
            {
                this.AccountType = value.HasValue ? new OptionSetValue(value.Value) : null;
            }
        }
        
        [AttributeLogicalName(Schema.rhs_industry)]
        public EntityReference Industry
        {
            get
            {

                return this.GetAttributeValue<EntityReference>(Schema.rhs_industry);
            }
            set
            {
                this.SetAttributeValue(Schema.rhs_industry, value);
            }
        }
        
        [AttributeLogicalName(Schema.rhs_industry)]
        public Guid? IndustryId
        {
            get
            {

                return this.Industry?.Id;
            }
            set
            {
                this.Industry = value.HasValue ? new EntityReference(XrmHajirIndustry.Schema.LogicalName, value.Value) : null;
            }
        }

        [AttributeLogicalName(Schema.rhs_companytype)]
        public OptionSetValue ConectionType
        {
            get => this.GetAttributeValue<OptionSetValue>(Schema.rhs_connectiontype);
            set => this.SetAttributeValue(Schema.rhs_connectiontype, value);
        }
        [AttributeLogicalName(Schema.rhs_companytype)]
        public int? ConectionTypeCode
        {
            get => this.ConectionType?.Value;
            set => this.ConectionType = value.HasValue ? new OptionSetValue(value.Value) : null;
        }

        [AttributeLogicalName(Schema.MethodIntroduction)]
        public EntityReference MethodIntroduction
        {
            get
            {

                return this.GetAttributeValue<EntityReference>(Schema.MethodIntroduction);
            }
            set
            {
                this.SetAttributeValue(Schema.MethodIntroduction, value);
            }
        }

        [AttributeLogicalName(Schema.rhs_industry)]
        public Guid? MethodIntroductionId
        {
            get
            {

                return this.MethodIntroduction?.Id;
            }
            set
            {
                this.MethodIntroduction = value.HasValue ? new EntityReference(XrmHajirMethodIntroduction.Schema.LogicalName, value.Value) : null;
            }
        }
    }
}
