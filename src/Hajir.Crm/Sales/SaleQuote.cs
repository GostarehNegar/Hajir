﻿using Hajir.Crm.Features.Sales;
using Hajir.Crm.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Sales
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
        public IEnumerable<SaleQuoteLine> Lines => _lines;
        private List<SaleAggergateProduct> aggergareProducts = new List<SaleAggergateProduct>();
        public IEnumerable<SaleAggergateProduct> AggregateProducts => aggergareProducts;
        public PriceList PriceList { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Remarks { get; set; }
        public bool PrintHeader { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int? PaymentTermCode { get; set; }
        public string RebuildRemarks()
        {
            var str = HajirCrmConstants.QuoteComments.STR_Gaurantee + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_BATTERY_GAURANTEE + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_BATTERY_GAURANTEE_SABA + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_FREE_CABLES + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_SUPPORT_CONTRACT + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_DELIVERY + "\r\n" +
                HajirCrmConstants.QuoteComments.STR_DELIVERY_POINT + "\r\n" +
                HajirCrmConstants.QuoteComments.STR1 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR2 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR3 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR4 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR5 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR6 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR7 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR8 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR9 + "\r\n" +
                HajirCrmConstants.QuoteComments.STR10 + "\r\n";
            return str;
        }
        public QuoteRemarksModel GetekarksModel()
        {
            var result = new QuoteRemarksModel { };
            result.AddLine(new QuoteRemarkLine { Selected = true, Text = HajirCrmConstants.QuoteComments.STR_Gaurantee });
            result.AddLine(new QuoteRemarkLine { Selected = true, Text = HajirCrmConstants.QuoteComments.STR_BATTERY_GAURANTEE });
            result.AddLine(new QuoteRemarkLine { Selected = true, Text = HajirCrmConstants.QuoteComments.STR_BATTERY_GAURANTEE_SABA });
            result.AddLine(new QuoteRemarkLine { Selected = true, Text = HajirCrmConstants.QuoteComments.STR_FREE_CABLES });
            return result;


        }
        public SaleQuote()
        {
            _lines = new List<SaleQuoteLine>();
        }
        public SaleQuote AddLine(SaleQuoteLine line)
        {
            //line = line ?? new SaleQuoteLine();
            _lines.Add(line ?? new SaleQuoteLine());
            return this;
        }
        public SaleQuote RemoveLine(SaleQuoteLine line)
        {
            _lines.Remove(line);
            return this;
        }
        public SaleQuote(string quoteId, string quoteNumber, IEnumerable<SaleQuoteLine> lines,
             PriceList pl)
        {
            QuoteId = quoteId;
            QuoteNumber = quoteNumber;
            _lines = new List<SaleQuoteLine>(lines ?? new List<SaleQuoteLine>());
            aggergareProducts = new List<SaleAggergateProduct>();
            PriceList = pl;
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
                _lines.Add(l);
            }
            aggergareProducts.Add(ag_product);
        }
        public void RemoveAggregate(SaleAggergateProduct product)
        {
            aggergareProducts = aggergareProducts.Where(x => x.Id != product.Id).ToList();

        }

    }
}
