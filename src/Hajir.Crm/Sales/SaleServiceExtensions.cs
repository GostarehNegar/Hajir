using Hajir.Crm.Common;
using Hajir.Crm.Internals;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Sales
{
    public static partial class SaleServiceExtensions
    {
        public static bool Validate(this IHajirCrmServiceContext context, SaleQuote quote)
        {
            return false;
        }
        public static void RecalculateQuote(this IHajirCrmServiceContext context, SaleQuote quote)
        {
            // Refresh Bundles

            var cache = context.GetService<ICacheService>();
            var pl = quote.PriceList;
            
            var bundles = quote.GetBundleIds();
            foreach (var bundle in quote.GetBundleIds())
            {
                var _bundle = quote.GetBundle(bundle);
                _bundle.Where(x => !x.IsParentBundle)
                    .ToList()
                    .ForEach(x =>
                    {
                       
                        x.PricePerUnit = x.PricePerUnit ?? pl.GetPrice(x.ProductId) ;
                        x.PercentTax = quote.PercentTax;
                        x.Recalculate();
                    });
                var _bundle_product = _bundle.FirstOrDefault(x => x.IsParentBundle);
                if (_bundle_product != null)
                {
                    _bundle_product.PricePerUnit = _bundle.Where(x => !x.IsParentBundle)
                        .Sum(x => x.PricePerUnit * x.Quantity);
                }
            }
            foreach(var line in quote.Lines)
            {
                if (string.IsNullOrWhiteSpace(line.ParentBundleId))
                {
                    line.PercentTax = quote.PercentTax;
                    line.Recalculate();
                }
            }
            //foreach (var agg in quote.AggregateProducts)
            //{
            //    agg.PricePerUint = 0M;
            //    foreach (var line in agg.Lines)
            //    {
            //        var price = pl.GetPrice(line.ProductId) ?? 0;
            //        agg.PricePerUint += price * line.Quantity;
            //    }
            //}

        }
        public static SaleQuote LoadQuoteByQuoteNumber(this IHajirCrmServiceContext context, string quoteNumber)
        {
            return context.GetService<IQuoteRepository>().LoadQuoteByNumber(quoteNumber);
        }
    }
}
