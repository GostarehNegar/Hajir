using Hajir.Crm.Features.Sales;
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
        public IEnumerable<SaleQuoteLine> Lines => _lines.OrderBy(x => x.LineItemNumber);
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
        public List<SaleQuoteLine> DeletedLines = new List<SaleQuoteLine>();
        public int? PercentTax => this.IsOfficial ? 10 : (int?)null;
        public SaleQuote()
        {
            _lines = new List<SaleQuoteLine>();
        }
        public string[] GetBundleIds()
        {
            return this._lines
                .GroupBy(x => x.ParentBundleId)
                .Select(x => x.Key)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
        public SaleQuoteLine[] GetBundle(string id)
        {
            return this._lines.Where(x => x.ParentBundleId == id).ToArray();
        }
        public SaleQuote RecalculateBundles()
        {
            var bundles = this.GetBundleIds();
            foreach (var _bundle in bundles)
            {
                var _b = this.Lines.FirstOrDefault(x => x.Id == _bundle);
                if (_b != null && _b.ParentBundleId != _bundle)
                {
                    _b.ParentBundleId = _bundle;
                }
            }
            return this;
        }
        public SaleQuote AddLine(SaleQuoteLine line)
        {
            //line = line ?? new SaleQuoteLine();
            var l = line ?? new SaleQuoteLine();
            l.QuoteId = this.QuoteId;
            _lines.Add(l);
            return this;
        }
        public SaleQuote RemoveLine(SaleQuoteLine line)
        {
            this.DeletedLines.Add(line);
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


        public void ReorderLines()
        {
            var n = 0;
            this.Lines.ToList().ForEach(x => {
                n++;
                x.LineItemNumber = n;
            });
        }
        public void AddBundle(ProductBundle bundle, int quantity = 1, decimal manualDiscount = 0M)
        {
            var bundleId = Guid.NewGuid().ToString();
            // Add Parent Bundle
            var bundleProduct = new SaleQuoteLine
            {
                ParentBundleId = bundleId,
                Name = bundle.Name,
                Quantity = 0,
                PricePerUnit = 1000,
                //IsParentBundle = true,
                Id = bundleId,
                QuoteId = this.QuoteId
            };
            this.AddLine(bundleProduct);


            foreach (var r in bundle.Rows)
            {
                //var uom = r.Product.DefaultUomId;
                var l = new SaleQuoteLine { ProductId = r.Product.Id, Quantity = r.Quantity, Name = r.Product.Name, ParentBundleId = bundleId };
                this.AddLine(l);
                //bundleProduct.AddBundleLine(l);
            }
            return;

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
        public void MoveUp(SaleQuoteLine line)
        {
           
            var prev_line_num = this.Lines
                .Where(x => x.LineItemNumber < line.LineItemNumber)
                .Max(x => x.LineItemNumber);
            var prev_line = this.Lines.FirstOrDefault(x => x.LineItemNumber == prev_line_num);
            if (prev_line != null)
            {
                var save = prev_line.LineItemNumber;
                prev_line.LineItemNumber = line.LineItemNumber;
                line.LineItemNumber= save;
            }
            var n = 0;
            this.Lines.ToList().ForEach(x => {
                n++;
                x.LineItemNumber = n;
            });

        }
        public void MoveDown(SaleQuoteLine line)
        {

            var next_line_num = this.Lines
                .Where(x => x.LineItemNumber > line.LineItemNumber)
                .Min(x => x.LineItemNumber);
            var next_line = this.Lines.FirstOrDefault(x => x.LineItemNumber == next_line_num);
            if (next_line != null)
            {
                var save = next_line.LineItemNumber;
                next_line.LineItemNumber = line.LineItemNumber;
                line.LineItemNumber = save;
            }
            var n = 0;
            this.Lines.ToList().ForEach(x => {
                n++;
                x.LineItemNumber = n;
            });

        }

        public decimal ExtendedAmount => this.Lines.Sum(x => x.ExtendedAmount??0);
        public string FormattedExtendedAmount => HajirUtils.Instance.FormatAmount(this.ExtendedAmount);
        public string ExtendedAmountInText => HajirCrmExtensions.NumberToString(this.ExtendedAmount);

    }
}
