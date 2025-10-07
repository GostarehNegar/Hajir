using GN.Library;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Nats;
using System.Collections;
using Hajir.Crm.Common;
using System.Linq;
using Hajir.Crm;

namespace Hajir.AI.Agents.PriceAgent
{
   
    //"""A product object."""

    public class ProductSearchResult
    {
        public class Product
        {
            public string id { get; set; }
            public string name { get; set; }
            public float price { get; set; }
            public string category { get; set; }
            public bool in_stock { get; set; }

        }
        public Product[] products { get; set; }
        public int total_found { get; set; }
        public string category_searched { get; set; }
    }
    public class ProductSearchRequest
    {
        public string Name { get; set; }
    }
    internal class PriceAgentService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ICacheService cache;

        public PriceAgentService(IServiceProvider serviceProvider, ICacheService cache)
        {
            this.serviceProvider = serviceProvider;
            this.cache = cache;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var con = this.serviceProvider.CreateNatsConnection();

            await con.GetSubscriptionBuilder()
                .WithSubjects("pricelist")
                .SubscribeAsync(async a =>
                {
                    var req = a.GetData<ProductSearchRequest>();
                    var ff = new ProductSearchResult
                    {
                        products = this.cache.GetPriceList(1).Items//.Where(1==1)
                            .Select(x => new ProductSearchResult.Product
                            {
                                name = this.cache.Products.FirstOrDefault(p => p.Id == x.ProductId)?.Name,
                                category = "ups",
                                id = x.Id,
                                in_stock = true,
                                price = float.Parse(x.Price.ToString())
                            })
                            .ToArray(),
                        total_found = 6,
                        category_searched = "ups"
                    };

                    await a.Reply(new ProductSearchResult
                    {
                        products = this.cache.GetPriceList(1).Items//.Where(1==1)
                            .Select(x => new ProductSearchResult.Product
                            {
                                name = this.cache.Products.FirstOrDefault(p=>p.Id==x.ProductId)?.Name,
                                category = "ups",
                                id = x.Id,
                                in_stock = true,
                                price = float.Parse(x.Price.ToString())
                            })
                            .ToArray(),
                        total_found = 6,
                        category_searched ="ups"
                    });
                    await Task.CompletedTask;
                });
            await base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
