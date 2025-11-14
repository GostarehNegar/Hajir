using GN.Library.AI.Agents;
using GN.Library.AI.Tools;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using Hajir.Crm;
using Hajir.Crm.Common;
using Hajir.Crm.Products;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class SearchProdcutsToolOptions
    {

    }
    internal class SearchProdcutsTool : BaseTool
    {
        public SearchProdcutsTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata
        {
            name = "search_products",
            description = "searches for products.",
            subject= "ai.agent.tools.seachproducts",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name ="search_text",
                    description="the text to search in product names and product numbers.",
                    required=true,
                    type="string"
                }
            }

        };

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            await Task.CompletedTask;
            var text = context.GetParameterValue<string>(this.MetaData.parameters[0].name);
            var pl = this.serviceProvider.GetService<ICacheService>().GetPriceList(1);
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new Exception($"Search text is NULL or Empty.");
            }

            var parts = text.Split(' ');
            IQueryable<Product> q = this.serviceProvider.GetService<ICacheService>().Products.AsQueryable()
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.ProductNumber));
            foreach(var part in text.Split(' '))
            {
                q = q.Where(x =>x.ProductNumber.Contains(text) || x.Name.ToLowerInvariant().Contains(text.ToLowerInvariant()));
            }
            var result =  q
                .ToArray()
                .Select(x => new { 
                    Name = x.Name,
                    Id = x.Id,
                    ProductNumber=x.ProductNumber,
                    Price = pl.GetPrice(x.Id),
                    Type = x.ProductType.ToString(),
                })
                .ToArray();
            if (false && result.Length > 1500)
            {
                throw new Exception("Too Many Products. Please narrow the search.");
            }
            return result.Take(10).ToArray();
        }
    }
}
