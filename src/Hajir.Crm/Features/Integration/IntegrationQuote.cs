using GN.Library.Shared.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationQuote : DynamicEntity
    {
        public IntegrationQuote() : base()
        {
            this.LogicalName = "quote";
        }
        //public IntegrationQuote(IEnumerable<KeyValuePair<string, object>> attribs)
        //{
        //    attribs.ToList().ForEach(x => this.SetAttributeValue(x.Key, x.Value));
        //}



        private List<IntegrationQuoteProduct> _prodcuts = new List<IntegrationQuoteProduct>();
        public IEnumerable<IntegrationQuoteProduct> Products => _prodcuts;

        public IntegrationQuote AddProducts(params IntegrationQuoteProduct[] products)
        {
            this._prodcuts.AddRange(products);
            return this;
        }
        public DynamicEntityReference Account
        {
            get
            {
                var result = this.GetAttributeValue<DynamicEntityReference>("customerid");
                return result?.LogicalName == "account" ? result : null;
            }
        }

        public int State => this.GetAttributeValue<int?>("statecode") ?? 0;
        public string AccountId => Account?.Id;

        public string QuoteNumber => this.GetAttributeValue<string>("quotenumber");
        public string OwningLoginName { get; set; }

        public DateTime? CreatedOn => this.GetAttributeValue<DateTime?>("createdon");
        public DateTime? ModifiedOn => this.GetAttributeValue<DateTime?>("modifiedon");
        public override string ToString()
        {
            return $"Quote: {this.Id}";
        }

    }
    public class IntegrationQuoteProduct : DynamicEntity
    {

        public string ProductDescription => this.GetAttributeValue<string>("productdescription");

    }
}
