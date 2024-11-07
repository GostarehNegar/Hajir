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
            public const string SOlutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const int SolutionBaeIndex = HajirCrmConstants.RahsamSolutionIndex;
            public const string QuoteType = SOlutionPerfix + "type";
            public const string HajirQuoteId = SOlutionPerfix + "quoteid";
            public const string ExternalId = SOlutionPerfix + "externalid";
            public const string PaymentDeadLine = SOlutionPerfix + "paymentdeadline";
            public const string ValidityPeriod = SOlutionPerfix + "validityperiod";
            public const string PaymentMethod = SOlutionPerfix + "paymentmethod";
            public const string Contact = SOlutionPerfix + "contact";
            public const string PrintHeader = HajirCrmConstants.HajirSolutionPerfix + "printheader";
            public const string Remarks = HajirCrmConstants.HajirSolutionPerfix + "remarks";
            public enum PaymentMethods
            {
                Cash = SolutionBaeIndex,
                NonCash = SolutionBaeIndex + 1


            }

        }
        [AttributeLogicalName(Schema.Contact)]
        public EntityReference Contact
        {
            get => this.GetAttributeValue<EntityReference>(Schema.Contact);
            set => this.SetAttribiuteValue(Schema.Contact,value);
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
        [AttributeLogicalName(Schema.QuoteType)]
        public bool QuoteType
        {
            get => this.GetAttributeValue<bool>(Schema.QuoteType);
            set => this.SetAttributeValue(Schema.QuoteType, value);

        }
        [AttributeLogicalName(Schema.PaymentDeadLine)]
        public double? PaymentDeadLine
        {
            get => this.GetAttributeValue<double?>(Schema.PaymentDeadLine);
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
            get => (Schema.PaymentMethods) this.PaymentMethod?.Value;
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
    
       
    }
}
