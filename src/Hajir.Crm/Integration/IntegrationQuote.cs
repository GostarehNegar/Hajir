using GN.Library.Shared.Entities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hajir.Crm.Integration
{
    public class IntegrationQuote : DynamicEntity
    {
        public IntegrationQuote() : base()
        {
            LogicalName = "quote";
        }
        //public IntegrationQuote(IEnumerable<KeyValuePair<string, object>> attribs)
        //{
        //    attribs.ToList().ForEach(x => this.SetAttributeValue(x.Key, x.Value));
        //}



        private List<IntegrationQuoteProduct> _prodcuts = new List<IntegrationQuoteProduct>();
        public IEnumerable<IntegrationQuoteProduct> Products => _prodcuts;

        public IntegrationQuote AddProducts(params IntegrationQuoteProduct[] products)
        {
            _prodcuts.AddRange(products);
            return this;
        }
        public DynamicEntityReference Account
        {
            get
            {
                var result = GetAttributeValue<DynamicEntityReference>("customerid");
                return result?.LogicalName == "account" ? result : null;
            }
        }

        public int State => GetAttributeValue<int?>("statecode") ?? 0;
        public string AccountId => Account?.Id;

        public string QuoteNumber => GetAttributeValue<string>("quotenumber");
        public string OwningLoginName { get; set; }

        public DateTime? CreatedOn => GetAttributeValue<DateTime?>("createdon");
        public DateTime? ModifiedOn => GetAttributeValue<DateTime?>("modifiedon");
        public override string ToString()
        {
            return $"Quote: {Id}";
        }

    }
    public class IntegrationQuoteProduct : DynamicEntity
    {

        public string ProductDescription => GetAttributeValue<string>("productdescription");
        public string ProcuctId => GetAttributeValue<DynamicEntityReference>("productid")?.Id;

       
        public string ProductNumber { get; set; }


    }
}
