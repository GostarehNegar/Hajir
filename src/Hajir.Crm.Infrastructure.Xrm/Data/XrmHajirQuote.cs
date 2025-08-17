using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirQuote : XrmQuote
    {
        public new class Schema : XrmQuote.Schema
        {
            public const string SOlutionPerfix_Deprecated = HajirCrmConstants.RahsamSolutionPerfix;
            public const string SolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
            public const int SolutionBaeIndex = HajirCrmConstants.RahsamSolutionIndex;
            public const string RhsQuoteType = SOlutionPerfix_Deprecated + "type";
            public const string HajirQuoteId = SOlutionPerfix_Deprecated + "quoteid";
            public const string ExternalId = SOlutionPerfix_Deprecated + "externalid";
            public const string PaymentDeadLine =SolutionPerfix+ "paymentdeadline";
            public const string ValidityPeriod = SOlutionPerfix_Deprecated + "validityperiod";
            public const string PaymentMethod = SOlutionPerfix_Deprecated + "paymentmethod";
            public const string RhsContact = SOlutionPerfix_Deprecated + "contact";
            public const string PrintHeader = HajirCrmConstants.HajirSolutionPerfix + "printheader";
            public const string PrintBundle = SolutionPerfix + "printbundle";
            public const string Remarks = HajirCrmConstants.HajirSolutionPerfix + "remarks";
            public const string Contact = HajirCrmConstants.HajirSolutionPerfix + "contactid";
            public const string QuoteType = HajirCrmConstants.HajirSolutionPerfix + "type";

            public enum PaymentMethods
            {
                Cash = SolutionBaeIndex,
                NonCash = SolutionBaeIndex + 1


            }

        }
        [AttributeLogicalName(Schema.RhsContact)]
        public EntityReference RhsContact
        {
            get => this.GetAttributeValue<EntityReference>(Schema.RhsContact);
            set => this.SetAttribiuteValue(Schema.RhsContact,value);
        }
        [AttributeLogicalName(Schema.Contact)]
        public EntityReference Contact
        {
            get => this.GetAttributeValue<EntityReference>(Schema.Contact);
            set => this.SetAttribiuteValue(Schema.Contact, value);
        }

        public Guid? HajirContactId
        {
            get => this.Contact?.Id;
            set => this.Contact = value.HasValue ? new EntityReference(XrmContact.Schema.LogicalName, value.Value) : null;
        }

        [AttributeLogicalName(Schema.HajirQuoteId)]
        public string HajirQuoteId
        {
            get => this.GetAttributeValue<string>(Schema.HajirQuoteId);
            set => this.SetAttributeValue(Schema.HajirQuoteId, value);
        }
        [AttributeLogicalName(Schema.ExternalId)]
        public string ExternalId
        {
            get => this.GetAttributeValue<string>(Schema.ExternalId);
            set => this.SetAttributeValue(Schema.ExternalId, value);
        }
        [AttributeLogicalName(Schema.RhsQuoteType)]
        public bool RhsQuoteType
        {
            get => this.GetAttributeValue<bool>(Schema.RhsQuoteType);
            set => this.SetAttributeValue(Schema.RhsQuoteType, value);

        }
        [AttributeLogicalName(Schema.QuoteType)]
        public bool QuoteType
        {
            get => this.GetAttributeValue<bool>(Schema.QuoteType);
            set => this.SetAttributeValue(Schema.QuoteType, value);

        }
        [AttributeLogicalName(Schema.PaymentDeadLine)]
        public int? PaymentDeadLine
        {
            get => this.GetAttributeValue<int?>(Schema.PaymentDeadLine);
            set => this.SetAttributeValue(Schema.PaymentDeadLine, value);

        }
        [AttributeLogicalName(Schema.ValidityPeriod)]
        public DateTime? ValidityPeriod
        {
            get => this.GetAttributeValue<DateTime?>(Schema.ValidityPeriod);
            set => this.SetAttributeValue(Schema.ValidityPeriod, value);

        }
        [AttributeLogicalName(Schema.PaymentMethod)]
        public OptionSetValue PaymentMethod
        {
            get => this.GetAttributeValue<OptionSetValue>(Schema.PaymentMethod);
            set => this.SetAttributeValue(Schema.PaymentMethod, value);

        }
        public Schema.PaymentMethods? PaymentMethodCode
        {
            get => (Schema.PaymentMethods?) this.PaymentMethod?.Value;
            set => this.PaymentMethod = value.HasValue ? new OptionSetValue((int) value) : null;

        }
        public bool Cash
        {
            get => this.PaymentMethodCode == null || this.PaymentMethodCode == Schema.PaymentMethods.Cash;
            set => this.PaymentMethodCode = value ? Schema.PaymentMethods.Cash : Schema.PaymentMethods.NonCash;
        }

        [AttributeLogicalName(Schema.PrintHeader)]
        public bool? PrintHeader
        {
            get => this.GetAttributeValue<bool?>(Schema.PrintHeader);
            set => this.SetAttribiuteValue(Schema.PrintHeader, value);
        }
        [AttributeLogicalName(Schema.PrintBundle)]
        public bool? PrintBundle
        {
            get => this.GetAttributeValue<bool?>(Schema.PrintBundle);
            set => this.SetAttribiuteValue(Schema.PrintBundle, value);
        }

    }
}
