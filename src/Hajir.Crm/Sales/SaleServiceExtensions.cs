using Hajir.Crm.Internals;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            var pl = quote.PriceList;
            foreach (var agg in quote.AggregateProducts)
            {
                agg.PricePerUint = 0M;
                foreach (var line in agg.Lines)
                {
                    var price = pl.GetPrice(line.ProductId) ?? 0;
                    agg.PricePerUint += price * line.Quantity;
                }
            }

        }
        public static SaleQuote LoadQuoteByQuoteNumber(this IHajirCrmServiceContext context, string quoteNumber)
        {
            return context.GetService<IQuoteRepository>().LoadQuoteByNumber(quoteNumber);
        }
    }
}
