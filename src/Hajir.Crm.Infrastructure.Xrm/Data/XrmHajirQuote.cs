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
            public const string QuoteType = SOlutionPerfix + "type";
            public const string HajirQuoteId = SOlutionPerfix + "quoteid";
            public const string ExternalId = SOlutionPerfix + "externalid";

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
    }
}
