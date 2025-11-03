using GN;
using GN.Library.Pipelines.WithBlockingCollection;
using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Common;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Hajir.Crm.Sales;
using Hajir.Crm.Sales.PriceLists;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Sales
{
    public class XrmSalesRepository
        : IQuoteRepository, IPriceListRepository
    {
        private readonly IXrmDataServices dataServices;
        private readonly ICacheService cache;
        private readonly ILogger<XrmSalesRepository> logger;

        public XrmSalesRepository(IXrmDataServices dataServices, ICacheService cache, ILogger<XrmSalesRepository> logger)
        {
            this.dataServices = dataServices;
            this.cache = cache;
            this.logger = logger;
        }

        public SaleQuote CreateQuote(SaleQuote quote)
        {
            var xrm_quote = new XrmHajirQuote();
            var xrm_quote_id = dataServices
                .GetRepository<XrmHajirQuote>()
                .Upsert(xrm_quote);
            return LoadQuote(xrm_quote_id.ToString());

        }

        public void DeleteAggregateProduct(string id)
        {
            if (Guid.TryParse(id, out var _id))
            {
                var repo = dataServices
                    .GetRepository<XrmHajirQuoteDetail>();
                repo
                .Queryable
                .GetAggregateDetails(_id)
                .ToList()
                .ForEach(x => repo.Delete(x));
                dataServices
                    .GetRepository<XrmHajirAggregateProduct>()
                    .Delete(new XrmHajirAggregateProduct { AggregateProductId = _id });
            }
        }

        private void EnsurePriceLists()
        {
            HajirCrmConstants.GetPriceListName(1);
            var pricelists = dataServices
                .GetRepository<XrmPriceList>()
                .Queryable
                .ToArray();
            if (!pricelists.Any(x => x.Name == HajirCrmConstants.GetPriceListName(1)))
            {
                dataServices.GetRepository<XrmPriceList>()
                    .Insert(new XrmPriceList { Name = HajirCrmConstants.GetPriceListName(1) });
            }
            if (!pricelists.Any(x => x.Name == HajirCrmConstants.GetPriceListName(2)))
            {
                dataServices.GetRepository<XrmPriceList>()
                    .Insert(new XrmPriceList { Name = HajirCrmConstants.GetPriceListName(2) });
            }

        }
        public IEnumerable<PriceList> LoadAllPriceLists()
        {
            var result = new List<PriceList>();
            EnsurePriceLists();
            var pricelists = dataServices
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
                    var items = dataServices
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
                Quantity = Convert.ToDecimal(x.Quantity ?? 0),
                ParentBundleId = x.ParentBundleId?.ToString(),
                Id = x.Id.ToString(),
                Name = cache.Products.FirstOrDefault(p => p.Id == x.ProductId?.ToString())?.Name,
                PricePerUnit = x.PricePerUnit,
                Discount = x.ManualDiscountAmount ?? 0,
                ExtendedAmount = x.ExtendedAmount,
                BaseAmount = x.BaseAmount,
                GuaranteeMonth = x.GuaranteeMonths,
                PercentDiscount = x.PercentDiscount,
                PercentTax = x.PercentTax,
                LineItemNumber = x.LineItemNumber,
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
                var xrm_quote = dataServices
                    .GetRepository<XrmHajirQuote>()
                    .Queryable
                    .FirstOrDefault(x => x.QuoteId == _id);

                if (xrm_quote != null)
                {
                    var lines = dataServices
                        .GetRepository<XrmHajirQuoteDetail>()
                        .Queryable
                        .GetDetails(xrm_quote.Id)
                        .Select(x => ToSaleQuoteLine(x));
                    //var aggregates =
                    //    this.dataServices
                    //    .GetRepository<XrmHajirAggregateProduct>()
                    //    .GetByquoteId(xrm_quote.Id)
                    //    .Select(x => new SaleAggergateProduct()
                    //    {
                    //        Id = x.Id.ToString(),
                    //        Name = x.Name,
                    //        Quantity = Convert.ToInt32(x.Quantity),
                    //        Amount = x.Amount,
                    //        ManualDiscount = x.ManualDiscount

                    //    }).ToArray();
                    //foreach (var agg in aggregates)
                    //{
                    //    lines.Where(l => l.AggregateId == agg.Id).ToList().ForEach(l => agg.AddLine(l));
                    //}
                    //aggregates.ToList().ForEach(x =>
                    //{
                    //	var ggg = lines.Where(l => l.AggregateId == x.Id).ToList();
                    //	lines.Where(l => l.AggregateId == x.Id).ToList().ForEach(l => x.AddLine(l));
                    //});
                    var pl = cache.PriceLists.FirstOrDefault(x => x.Id == xrm_quote.PriceLevelId?.ToString());

                    quote = new SaleQuote(xrm_quote.QuoteId.ToString(), xrm_quote.QuoteNumber, lines, pl);
                    quote.PyamentDeadline = xrm_quote.PaymentDeadLine;
                    quote.IsOfficial = xrm_quote.QuoteType;
                    quote.ExpirationDate = xrm_quote.ExpiresOn;
                    quote.NonCash = !xrm_quote.Cash;
                    quote.Remarks = xrm_quote.GetAttributeValue<string>("hajir_remarks");
                    quote.PrintHeader = xrm_quote.PrintHeader ?? true;
                    quote.PrintBundle = xrm_quote.PrintBundle ?? false;
                    quote.EffectiveFrom = xrm_quote.EffectiveFrom;
                    quote.EffectiveTo = xrm_quote.EffectiveTo;
                    quote.PaymentTermCode = xrm_quote.PaymentTermsCode?.Value;
                    if (xrm_quote.AccountId.HasValue)
                    {
                        var acc = dataServices.GetRepository<XrmAccount>()
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
                        var acc = dataServices.GetRepository<XrmContact>()
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

            return quote.RecalculateBundles();

        }

        public SaleQuote LoadQuoteByNumber(string quoteNumber)
        {
            var id = dataServices
                .GetRepository<XrmHajirQuote>()
                .Queryable
                .FirstOrDefault(x => x.QuoteNumber == quoteNumber)
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
            dataServices.GetRepository<XrmHajirQuoteDetail>()
                .Insert(l);


        }

        public SaleQuote UpdateQuote(SaleQuote quote)
        {
            /// Updating the sale quote.
            /// 
            var xrm_quote = quote.ToXrmQuote();
            var uoms = cache.UnitOfMeasurements;

            foreach (var ap in quote.AggregateProducts)
            {
                if (!Guid.TryParse(ap.Id, out var ap_id))
                {
                    var xrm_ag = ap.ToXrmAggergateProduct();
                    xrm_ag.QuoteId = xrm_quote.Id;
                    ap_id = dataServices.GetRepository<XrmHajirAggregateProduct>()
                        .Upsert(xrm_ag);
                }
                foreach (var l in ap.Lines.Select(x => x.ToXrmQuoteDetail()))
                {
                    l.QuoteId = xrm_quote.Id;
                    l.AggregateProductId = ap_id;
                    var p = cache.Products.FirstOrDefault(x => x.Id == l.ProductId?.ToString());
                    if (p != null && Guid.TryParse(p.UOMId, out var _uom))
                    {
                        l.UnitOfMeasureId = _uom;
                    }

                    //l.UnitOfMeasureId=
                    if (l.Id == Guid.Empty)
                    {
                        dataServices.GetRepository<XrmHajirQuoteDetail>()
                            .Upsert(l);
                    }

                }
            }
            return LoadQuote(quote.QuoteId);
        }

        public async Task<SaleQuoteLine> SaveLine(SaleQuoteLine line)
        {

            await Task.CompletedTask;

            var l = line.ToXrmQuoteDetail();
            l.IsProductOverridden = l.ProductId == null;
            if (!l.IsProductOverridden ?? false)
            {
                l.UnitOfMeasureId = Guid.TryParse(cache.UnitOfMeasurements.FirstOrDefault()?.Id, out var _v) ? _v : (Guid?)null;
            }


            var existing = l.Id == Guid.Empty ? null : dataServices.GetRepository<XrmHajirQuoteDetail>()
                .Queryable
                .FirstOrDefault(x => x.QuoteDetailId == l.Id);


            if (existing != null)
            {
                dataServices
                .GetRepository<XrmHajirQuoteDetail>()
                .Update(l);
            }
            else
            {
                Guid? _save_parent = null;
                if (l.ParentBundleId == l.Id)
                {
                    _save_parent = l.ParentBundleId;
                    l.ParentBundleId = null;
                }
                line.Id = dataServices
                    .GetRepository<XrmHajirQuoteDetail>()
                    .Insert(l).ToString();
                if (_save_parent.HasValue)
                {
                    dataServices
                        .GetRepository<XrmHajirQuoteDetail>()
                        .Update(new XrmHajirQuoteDetail
                        {
                            QuoteDetailId = Guid.Parse(line.Id),
                            ParentBundleId = _save_parent
                        });

                }

            }
            return line;
        }


        public async Task<IEnumerable<SaleAccount>> SearchAccount(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new SaleAccount[] { };
            var a = dataServices
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
            return dataServices
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
                return dataServices
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
            return dataServices
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
            x.PaymentDeadLine = quote.PyamentDeadline ?? 0;

            dataServices
                .GetRepository<XrmHajirQuote>()
                .Upsert(x);
            return quote;


            //throw new NotImplementedException();
        }

        public Task DeleteQuoteDetailLine(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && Guid.TryParse(id, out var _id))
            {
                dataServices
                    .GetRepository<XrmQuoteDetail>()
                    .Delete(new XrmQuoteDetail { Id = _id });
            }
            return Task.CompletedTask;
        }

        public async Task<int> ImportExcelPriceList(PriceList priceList)
        {
            var pl1 = this.LoadPriceList("1");
            var pl2 = this.LoadPriceList("2");
            var products = this.cache.Products;
            var result = 0;



            foreach (var item in priceList.Items)
            {
                var product = products.FirstOrDefault(x => x.ProductNumber == item.ProductNumber);
                await Task.CompletedTask;
                try
                {
                    if (product != null)
                    {
                        var uom = this.cache.UnitOfMeasurements.FirstOrDefault(x => x.Id == product.UOMId);
                        if (uom == null)
                        {
                            throw new Exception($"UOM not found. Product:{product.Name}");
                        }
                        var p1 = pl1.Items.FirstOrDefault(x => x.ProductId == product.Id);
                        var p2 = pl2.Items.FirstOrDefault(x => x.ProductId == product.Id);
                        var change = false;
                        if (p1 == null)
                        {
                            change = true;
                            this.dataServices
                                .GetRepository<XrmPriceListItem>()
                                .Insert(new XrmPriceListItem
                                {
                                    Amount = item.Price1,
                                    PriceListId = Guid.Parse(pl1.Id),
                                    ProcuctId = product.GetId<Guid>(),
                                    [XrmPriceListItem.Schema.uomid] = new EntityReference(XrmUnitOfMeasure.Schema.LogicalName, Guid.Parse(uom.Id)),
                                    [XrmPriceListItem.Schema.uomscheduleid] = new EntityReference(XrmUnitOfMeasurementGroup.Schema.LogicalName, Guid.Parse(uom.UnitId))

                                });
                        }
                        else if (p1.Price != item.Price1)
                        {
                            change = true;
                            this.dataServices
                                .GetRepository<XrmPriceListItem>()
                                .Update(new XrmPriceListItem
                                {
                                    Id = Guid.Parse(p1.Id),
                                    Amount = item.Price1,
                                });
                        }
                        if (p2 == null)
                        {
                            change = true;
                            this.dataServices
                                .GetRepository<XrmPriceListItem>()
                                .Insert(new XrmPriceListItem
                                {
                                    Amount = item.Price2,
                                    PriceListId = Guid.Parse(pl2.Id),
                                    ProcuctId = product.GetId<Guid>(),
                                    [XrmPriceListItem.Schema.uomid] = new EntityReference(XrmUnitOfMeasure.Schema.LogicalName, Guid.Parse(uom.Id)),
                                    [XrmPriceListItem.Schema.uomscheduleid] = new EntityReference(XrmUnitOfMeasurementGroup.Schema.LogicalName, Guid.Parse(uom.UnitId))

                                });
                        }
                        else if (p2.Price != item.Price2)
                        {
                            change = true;
                            this.dataServices
                                .GetRepository<XrmPriceListItem>()
                                .Update(new XrmPriceListItem
                                {
                                    Id = Guid.Parse(p2.Id),
                                    Amount = item.Price2,
                                });
                        }
                        if (change)
                        {
                            result++;
                            this.dataServices
                                .GetRepository<XrmPriceRecord>()
                                .Insert(new XrmPriceRecord
                                {
                                    ProductId = Guid.Parse(product.Id),
                                    Name = $"{product.ProductNumber} {product.Name}",
                                    Price1 = new Money(item.Price1 ?? 0),
                                    Price2 = new Money(item.Price2 ?? 0),
                                });
                        }


                    }
                    else
                    {
                        this.logger.LogWarning(
                            $"Product Not Found. Product:'{item.ProductNumber}'");
                    }
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        $"An error occured while trying to import pric list. Product:{item.ProductNumber}");
                }
            }

            return result;
            
        }
        public PriceList LoadPriceList(string name)
        {
            var pl = dataServices
                .GetRepository<XrmPriceList>()
                .Queryable
                .ToArray()
                .FirstOrDefault(x => x.Name.Contains(name));
            if (pl == null)
                throw new Exception($"Pricelist not found. Name:{name}");
            var skip = 0;
            var take = 100;
            var result = new PriceList
            {
                Id = pl.Id.ToString(),
                Name = pl.Name,

            };
            while (true)
            {
                var items = dataServices
                    .GetRepository<XrmPriceListItem>()
                    .Queryable
                    .Where(x => x.PriceListId == pl.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToArray();
                result.AddItems(items.Select(x => new PriceListItem
                {
                    Id = x.Id.ToString(),
                    Price = x.Amount ?? 0M,
                    ProductId = x.ProcuctId?.ToString(),
                }).ToArray()); ;
                skip += take;
                if (items.Length < take)
                    break;
            }
            return result;

        }
    }
}
