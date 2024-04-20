using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationQuote:DynamicEntity
    {
        public IntegrationQuote() : base() { }
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
            
            
        public string AccountId => Account?.Id;
        
    }
    public class IntegrationQuoteProduct : DynamicEntity
    {

        public string ProductDescription => this.GetAttributeValue<string>("productdescription");

    }
}
