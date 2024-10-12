using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Features.Sales
{
    public class SaleQuote : ISaleQuote
    {
        public SaleAccount Customer { get; set; }
        public SaleContact Contact { get; set; }

        private List<SaleQuoteLine> _lines;
        public string QuoteNumber { get; private set; }
        public bool IsOfficial { get; set; }
        public bool NonCash { get; set; }
        public double? PyamentDeadline { get; set; }
        public string QuoteId { get; }
        public IEnumerable<SaleQuoteLine> Lines => this._lines;
        private List<SaleAggergateProduct> aggergareProducts = new List<SaleAggergateProduct>();
        public IEnumerable<SaleAggergateProduct> AggregateProducts => aggergareProducts;
        public PriceList PriceList { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Remarks { get; set; }
        public string RebuildRemarks()
        {
            var str = HajirCrmConstants.QuoteComments.STR_Gaurantee + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_BATTERY_GAURANTEE + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_BATTERY_GAURANTEE_SABA + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_FREE_CABLES + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_SUPPORT_CONTRACT + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_DELIVERY + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_DELIVERY_POINT + "\r\n";
                

            return str;
        }
        public SaleQuote()
        {
            this._lines = new List<SaleQuoteLine>();
        }
        public SaleQuote AddLine(SaleQuoteLine line)
        {
            //line = line ?? new SaleQuoteLine();
            this._lines.Add(line ?? new SaleQuoteLine());
            return this;
        }
        public SaleQuote RemoveLine(SaleQuoteLine line)
        {
            this._lines.Remove(line);
            return this;
        }
        public SaleQuote(string quoteId, string quoteNumber, IEnumerable<SaleQuoteLine> lines,
            IEnumerable<SaleAggergateProduct> aggregates, PriceList pl)
        {
            QuoteId = quoteId;
            QuoteNumber = quoteNumber;
            this._lines = new List<SaleQuoteLine>(lines ?? new List<SaleQuoteLine>());
            this.aggergareProducts = new List<SaleAggergateProduct>(aggregates ?? new List<SaleAggergateProduct>());
            this.PriceList = pl;
        }

        public void AddBundle(ProductBundle bundle, int quantity = 1, decimal manualDiscount = 0M)
        {
            var ag_product = new SaleAggergateProduct()
            {
                Quantity = quantity,
                ManualDiscount = manualDiscount,
                Name = bundle.Name
            };
            foreach (var r in bundle.Rows)
            {
                var l = new SaleQuoteLine { ProductId = r.Product.Id, Quantity = r.Quantity };
                ag_product.AddLine(l);
                this._lines.Add(l);
            }
            this.aggergareProducts.Add(ag_product);
        }
        public void RemoveAggregate(SaleAggergateProduct product)
        {
            this.aggergareProducts = this.aggergareProducts.Where(x => x.Id != product.Id).ToList();

        }

    }
}
