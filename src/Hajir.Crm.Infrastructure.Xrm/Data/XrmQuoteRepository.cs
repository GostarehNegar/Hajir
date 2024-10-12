using GN;
using GN.Library.Pipelines.WithBlockingCollection;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Features.Common;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    public class XrmQuoteRepository : IQuoteRepository
    {
        private readonly IXrmDataServices dataServices;
        private readonly ICacheService cache;

        public XrmQuoteRepository(IXrmDataServices dataServices, ICacheService cache)
        {
            this.dataServices = dataServices;
            this.cache = cache;
        }

        public SaleQuote CreateQuote(SaleQuote quote)
        {
            var xrm_quote = new XrmHajirQuote();
            var xrm_quote_id = this.dataServices
                .GetRepository<XrmHajirQuote>()
                .Upsert(xrm_quote);
            return LoadQuote(xrm_quote_id.ToString());

        }

        public void DeleteAggregateProduct(string id)
        {
            if (Guid.TryParse(id, out var _id))
            {
                var repo = this.dataServices
                    .GetRepository<XrmHajirQuoteDetail>();
                repo
                .Queryable
                .GetAggregateDetails(_id)
                .ToList()
                .ForEach(x => repo.Delete(x));
                this.dataServices
                    .GetRepository<XrmHajirAggregateProduct>()
                    .Delete(new XrmHajirAggregateProduct { AggregateProductId = _id });
            }
        }

        public IEnumerable<PriceList> LoadAllPriceLists()
        {
            var result = new List<PriceList>();
            var pricelists = this.dataServices
                .GetRepository<XrmPriceList>()
                .Queryable
                .ToArray();
            foreach (var pl in pricelists)
            {
                var skip = 0;
                var take = 100;
                var _pl = new PriceList
                {
                    Id = pl.Id.ToString(),
                    Name = pl.Name,

                };
                result.Add(_pl);
                while (true)
                {
                    var items = this.dataServices
                        .GetRepository<XrmPriceListItem>()
                        .Queryable
                        .Where(x => x.PriceListId == pl.Id)
                        .Skip(skip)
                        .Take(take)
                        .ToArray();
                    _pl.AddItems(items.Select(x => new PriceListItem
                    {
                        Id = x.Id.ToString(),
                        Price = x.Amount ?? 0M,
                        ProductId = x.ProcuctId?.ToString()

                    }).ToArray()); ;


                    skip += take;


                    if (items.Length < take)
                        break;
                }
            }
            return result;

        }

        private SaleQuoteLine ToSaleQuoteLine(XrmHajirQuoteDetail x)
        {
            if (x == null)
            {
                return null;
            }
            var result = new SaleQuoteLine()
            {
                ProductId = x.ProductId?.ToString(),
                Quantity = Convert.ToDecimal((x.Quantity ?? 0)),
                AggregateId = x.AggregateProductId?.ToString(),
                Id = x.Id.ToString(),
                Name = this.cache.Products.FirstOrDefault(p => p.Id == x.ProductId?.ToString())?.Name,
                PricePerUnit = x.PricePerUnit,
                Discount = x.ManualDiscountAmount ?? 0,
                ExtendedAmount = x.ExtendedAmount,
                BaseAmount = x.BaseAmount,
                GuaranteeMonth = x.GuaranteeMonths,
                PercentDiscount = x.PercentDiscount,
                PercentTax = x.PercentTax,
                Tax = x.Tax,
            };
            if (x.IsProductOverridden.HasValue && x.IsProductOverridden.Value)
            {
                result.Name = x.ProductDescription;
            };
            return result;

        }
        public SaleQuote LoadQuote(string id)
        {
            SaleQuote quote = null;
            if (Guid.TryParse(id, out var _id))
            {
                var xrm_quote = this.dataServices
                    .GetRepository<XrmHajirQuote>()
                    .Queryable
                    .FirstOrDefault(x => x.QuoteId == _id);

                if (xrm_quote != null)
                {
                    var lines = this.dataServices
                        .GetRepository<XrmHajirQuoteDetail>()
                        .Queryable
                        .GetDetails(xrm_quote.Id)
                        .Select(x => ToSaleQuoteLine(x));
                    var aggregates =
                        this.dataServices
                        .GetRepository<XrmHajirAggregateProduct>()
                        .GetByquoteId(xrm_quote.Id)
                        .Select(x => new SaleAggergateProduct()
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name,
                            Quantity = Convert.ToInt32(x.Quantity),
                            Amount = x.Amount,
                            ManualDiscount = x.ManualDiscount

                        }).ToArray();
                    foreach (var agg in aggregates)
                    {
                        lines.Where(l => l.AggregateId == agg.Id).ToList().ForEach(l => agg.AddLine(l));
                    }
                    //aggregates.ToList().ForEach(x =>
                    //{
                    //	var ggg = lines.Where(l => l.AggregateId == x.Id).ToList();
                    //	lines.Where(l => l.AggregateId == x.Id).ToList().ForEach(l => x.AddLine(l));
                    //});
                    var pl = cache.PriceLists.FirstOrDefault(x => x.Id == xrm_quote.PriceLevelId?.ToString());

                    quote = new SaleQuote(xrm_quote.QuoteId.ToString(), xrm_quote.HajirQuoteId, lines, aggregates, pl);
                    quote.PyamentDeadline = xrm_quote.PaymentDeadLine;
                    quote.IsOfficial = xrm_quote.QuoteType;
                    quote.ExpirationDate = xrm_quote.ValidityPeriod;
                    quote.NonCash = !xrm_quote.Cash;
                    quote.Remarks = xrm_quote.GetAttributeValue<string>("hajir_remarks");
                    if (xrm_quote.AccountId.HasValue)
                    {
                        var acc = this.dataServices.GetRepository<XrmAccount>()
                            .Queryable
                            .FirstOrDefault(x => x.AccountId == xrm_quote.AccountId);
                        if (acc != null)
                        {
                            quote.Customer = new SaleAccount
                            {
                                Id = acc.Id.ToString(),
                                Name = acc.Name
                            };
                        }
                    }
                    if (xrm_quote.HajirContactId.HasValue)
                    {
                        var acc = this.dataServices.GetRepository<XrmContact>()
                            .Queryable
                            .FirstOrDefault(x => x.ContactId == xrm_quote.HajirContactId);
                        if (acc != null)
                        {
                            quote.Contact = new SaleContact
                            {
                                Id = acc.Id.ToString(),
                                Name = acc.FullName
                            };
                        }
                    }
                }

            }

            return quote;

        }

        public SaleQuote LoadQuoteByNumber(string quoteNumber)
        {
            var id = this.dataServices
                .GetRepository<XrmHajirQuote>()
                .Queryable
                .FirstOrDefault(x => x.HajirQuoteId == quoteNumber)
                ?.Id;
            if (!id.HasValue)
            {
                return null;
                throw new Exception($"Quote Not Found.");
            }
            return LoadQuote(id.ToString());

        }

        public async Task Test(QuoteEditModel q)
        {
            var l = new XrmHajirQuoteDetail
            {
            };
            l.ProductDescription = "test";
            l.QuoteId = q.Id;
            l.SetAttribiuteValue("isproductoverridden", true);
            this.dataServices.GetRepository<XrmHajirQuoteDetail>()
                .Insert(l);


        }

        public SaleQuote UpdateQuote(SaleQuote quote)
        {
            /// Updating the sale quote.
            /// 
            var xrm_quote = quote.ToXrmQuote();
            var uoms = this.cache.UnitOfMeasurements;

            foreach (var ap in quote.AggregateProducts)
            {
                if (!Guid.TryParse(ap.Id, out var ap_id))
                {
                    var xrm_ag = ap.ToXrmAggergateProduct();
                    xrm_ag.QuoteId = xrm_quote.Id;
                    ap_id = this.dataServices.GetRepository<XrmHajirAggregateProduct>()
                        .Upsert(xrm_ag);
                }
                foreach (var l in ap.Lines.Select(x => x.ToXrmQuoteDetail()))
                {
                    l.QuoteId = xrm_quote.Id;
                    l.AggregateProductId = ap_id;
                    var p = this.cache.Products.FirstOrDefault(x => x.Id == l.ProductId?.ToString());
                    if (p != null && Guid.TryParse(p.UOMId, out var _uom))
                    {
                        l.UnitOfMeasureId = _uom;
                    }

                    //l.UnitOfMeasureId=
                    if (l.Id == Guid.Empty)
                    {
                        this.dataServices.GetRepository<XrmHajirQuoteDetail>()
                            .Upsert(l);
                    }

                }
            }
            return this.LoadQuote(quote.QuoteId);
        }

        public async Task<SaleQuoteLine> SaveLine(SaleQuoteLine line)
        {
           

            var l = line.ToXrmQuoteDetail();

            l.IsProductOverridden = l.ProductId == null;
            var _id = this.dataServices
                .GetRepository<XrmHajirQuoteDetail>()
                .Upsert(l);
            await Task.CompletedTask;
            line.Id = _id.ToString();
            return line;
        }


        public async Task<IEnumerable<SaleAccount>> SearchAccount(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new SaleAccount[] { };
            var a = this.dataServices
                .WithImpersonatedDbContext(ctx =>
                {

                    var items = text.Split(' ');
                    var q = ctx.AddEntity<XrmAccount>()
                    .Query<XrmAccount>();
                    foreach (var item in text.Split(' '))
                    {
                        q = q.Where(x => x.Name.Contains(item));
                    }
                    return q.ToArray();
                    ; return ctx.AddEntity<XrmAccount>()
                                        .Query<XrmAccount>()
                                        .Where(x => x.Name.Contains(text))
                                        .ToArray();


                });
            return a.Select(x => new SaleAccount
            {
                Name = x.Name,
                Id = x.Id.ToString(),
            }).ToArray();
            return this.dataServices
                .GetRepository<XrmHajirAccount>()
                .Queryable
                .Where(x => x.Contains(text))
                .ToArray()
                .Select(x => new SaleAccount
                {
                    Name = x.Name,
                    Id = x.Id.ToString()
                })
                .ToArray();

        }

        public async Task<IEnumerable<SaleContact>> GetAccountContacts(string accountId)
        {
            if (Guid.TryParse(accountId, out var _accountId))
            {
                return this.dataServices
                    .GetRepository<XrmContact>()
                    .Queryable
                    .Where(x => x.AccountId == _accountId)
                    .ToArray()
                    .Select(x => new SaleContact
                    {
                        Id = x.Id.ToString(),
                        Name = x.FullName
                    })
                    .ToArray();

            }
            return new SaleContact[] { };

        }

        public async Task<IEnumerable<PriceList>> SearchPriceList(string text)
        {
            return this.dataServices
                .GetRepository<XrmPriceList>()
                .Queryable
                .ToArray()
                .Select(x => new PriceList
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                });

        }

        public SaleQuote UpsertQuote(SaleQuote quote)
        {
            var x = new XrmHajirQuote();
            if (Guid.TryParse(quote.QuoteId, out var _quoteId))
            {
                x.Id = _quoteId;
            }
            if (quote.Customer != null && Guid.TryParse(quote.Customer.Id, out var _accid))
            {
                x.AccountId = _accid;
            }
            if (quote.PriceList != null && Guid.TryParse(quote.PriceList.Id, out var _priceListId))
            {
                x.PriceLevelId = _priceListId;
            }
            x.QuoteType = quote.IsOfficial;
            //x["rhs_type"] = quote.NoOfficial;
            x["rhs_paymentdeadline"] = quote.PyamentDeadline ?? 0;

            this.dataServices
                .GetRepository<XrmHajirQuote>()
                .Upsert(x);
            return quote;


            //throw new NotImplementedException();
        }

        public Task DeleteQuoteDetailLine(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out var _id))
            {
                this.dataServices
                    .GetRepository<XrmQuoteDetail>()
                    .Delete(new XrmQuoteDetail { Id = _id });
            }
            return Task.CompletedTask;
        }
    }
}
