using GN.Library.Xrm.StdSolution;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
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
            public const string RhsEconomicCode = "rhs_economiccode";
            public const string RhsNationalCode = "rhs_nationalcode";
            public const string RhsRegistrationNauber = "rhs_registrationnumber";
            public const string rhs_city = "rhs_city";
            public const string rhs_state = "rhs_state";
            public const string DegreeImportance = "rhs_degreeimportance";
            public const string rhs_MainPhone = "rhs_mainphone";
            public const string rhs_MainCityCode = "rhs_maincitycode";
            public const string rhs_BrandName = "rhs_brandname";
            public const string NationalId = HajirCrmConstants.HajirSolutionPerfix + "nationalid";
            public const string RegistrationNumber = HajirCrmConstants.HajirSolutionPerfix + "registrationnumber";
            public const string EconomicCode = HajirCrmConstants.HajirSolutionPerfix + "economiccode";
            public const string IntroductionMethod = HajirCrmConstants.HajirSolutionPerfix + "introductionmethod";
            public const string RelationTypeCode = "customertypecode";
            public const string AccountType = HajirCrmConstants.HajirSolutionPerfix + "accounttype";
            public const string BrandName = HajirCrmConstants.HajirSolutionPerfix + "brandname";
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
        public OptionSetValue RhsAccountType
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
        public int? RhsAccountTypeCode
        {
            get
            {

                return this.GetAttributeValue<OptionSetValue>(Schema.rhs_companytype)?.Value;
            }
            set
            {
                this.RhsAccountType = value.HasValue ? new OptionSetValue(value.Value) : null;
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
        public int? RhsConectionTypeCode
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
        [AttributeLogicalName(Schema.RhsEconomicCode)]
        public string RhsEconomicCode
        {
            get => this.GetAttributeValue<string>(Schema.RhsEconomicCode);
            set => this.SetAttributeValue(Schema.RhsEconomicCode, value);
        }
        [AttributeLogicalName(Schema.RhsNationalCode)]
        public string RhsNationalCode
        {
            get => this.GetAttributeValue<string>(Schema.RhsNationalCode);
            set => this.SetAttributeValue(Schema.RhsNationalCode, value);
        }
        [AttributeLogicalName(Schema.RhsRegistrationNauber)]
        public string RhsRegistrationNauber
        {
            get => this.GetAttributeValue<string>(Schema.RhsRegistrationNauber);
            set => this.SetAttributeValue(Schema.RhsRegistrationNauber, value);
        }

        [AttributeLogicalName(Schema.IntroductionMethod)]
        public OptionSetValue IntroductionMethod
        {
            get => this.GetAttributeValue<OptionSetValue>(Schema.IntroductionMethod);
            set => this.SetAttribiuteValue(Schema.IntroductionMethod, value);
        }
        [AttributeLogicalName(Schema.IntroductionMethod)]
        public int? IntroductionMethodCode
        {
            get => this.GetAttributeValue<OptionSetValue>(Schema.IntroductionMethod)?.Value;
            set => this.IntroductionMethod = value.HasValue ? new OptionSetValue(value.Value) : null;


        }
        [AttributeLogicalName(Schema.RelationTypeCode)]
        public OptionSetValue RelationTypeCode
        {
            get => this.GetAttributeValue<OptionSetValue>(Schema.RelationTypeCode);
            set => this.SetAttribiuteValue(Schema.RelationTypeCode, value);
        }
        [AttributeLogicalName(Schema.RelationTypeCode)]
        public int? RelationType
        {
            get => this.RelationTypeCode?.Value;
            set => this.RelationTypeCode = value.HasValue ? new OptionSetValue(value.Value) : null;
        }

        [AttributeLogicalName(Schema.AccountType)]
        public OptionSetValue AccountType
        {
            get => this.GetAttributeValue<OptionSetValue>(Schema.AccountType);
            set => this.SetAttribiuteValue(Schema.AccountType, value);
        }
        [AttributeLogicalName(Schema.AccountType)]
        public int? AccountTypeCode
        {
            get => this.AccountType?.Value;
            set => this.AccountType = value.HasValue ? new OptionSetValue(value.Value) : null;
        }


    }
}
