using GN.Library.AI.Agents;
using GN.Library.AI.Tools;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using Hajir.Crm;
using Hajir.Crm.Common;
using Hajir.Crm.Products.ProductCompetition;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class ProductPriceComparisonToolOptions
    {

    }
    internal class ProductPriceComparisonTool : BaseTool
    {
        public const string Name = "get_competitors";
        public ProductPriceComparisonTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {

            description = "get competitor prices for products based on the specified power",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name = "power",
                    description= "the power of ups in KVA. Prices are compared based on their power",
                    type="decimal",
                    required = true
                }
            },
            returns =
            {
                {"type","List" },
                {"description","retruns price list each row for a manufacturer" }
            }
        }.Validate();

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            await Task.CompletedTask;
            bool isStabilizer(string name)
            {
                return name != null && (name.Contains("استب") || name.ToLowerInvariant().Contains("stab") || name.ToLowerInvariant().Contains("stb"));
            }
            var _power = context.GetParameterValue<object>(this.MetaData.parameters[0].name)?.ToString();
            decimal? power = !string.IsNullOrWhiteSpace(_power) && decimal.TryParse(_power, out var _p) ? _p : (decimal?)null;
            if (power == null)
                throw new Exception("Power is required");
            var compe = this.serviceProvider.GetService<ICacheService>().GetCompetitors();
            var pl = this.serviceProvider.GetService<ICacheService>().GetPriceList(1);

            var hajir = this.serviceProvider.GetService<ICacheService>().Products.Where(
                x => x.GetKVA() == power && pl.GetPrice(x.Id) > 0 && isStabilizer(x.Name))
                .Select(x => new
                    Competitor.PriceItem
                {
                    Price = Convert.ToInt32(pl.GetPrice(x.Id)).ToString(),
                    Name = x.Name,
                    Manufacturer = "hajir"
                })
                .FirstOrDefault();
            var result = new List<Competitor.PriceItem>();
            result.Add(hajir);
            foreach (var c in compe)
            {
                result.Add(c.Items.FirstOrDefault(x => x.GetPrice() > 0 && x.GetKVA() == power && isStabilizer(x.Name)));
            }
            result = result.Where(x => x != null).ToList();
            result.ToList().ForEach(x => x.Price = $"{x.GetPrice().ToString()} ریال");
            return result.Where(x => x != null).ToArray();

        }
        private Task HitCache(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    var target = this.serviceProvider.GetService<ICacheService>();
                    target.GetCompetitors();
                    var p = target.Products;
                    target.GetPriceList(1);
                    await Task.Delay(10 * 60 * 1000);

                }


            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(base.ExecuteAsync(stoppingToken), HitCache(stoppingToken));
        }
    }
}
