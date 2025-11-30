using GN.Library;
using GN.Library.AI.Agents;
using GN.Library.AI.Tools;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using GN.Library.Xrm;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class AccountSearchToolOptions
    {

    }
    internal class AccountSearchTool : BaseTool
    {
        public AccountSearchTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata("search_accounts")
        {
            subject = $"ai.agent.tools.search_accounts",
            description = "Searches for accounts (companies) in CRM system",
            name = "search_accounts",
            domain = "crm",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name="search_text",
                    description ="the text to search for in account names.",
                    type="string",
                    required=true,
                }
            },
            returns = new Dictionary<string, object>
            {
                { "type", "List" },
                { "description", "List of accouts that match the search text." }
            }
        }.Validate();

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            var repo = this.serviceProvider.GetService<IXrmDataServices>().GetRepository<XrmHajirAccount>();
            var query = repo.Queryable;
            var text = context.GetParameterValue<string>(this.MetaData.parameters[0].name);
            if (string.IsNullOrWhiteSpace(text))
                throw new Exception("Search Text is Null or Empty");
            foreach (var part in text.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()))
            {
                query = query.Where(x => x.Name.Contains(part) && x.StateCode==0);
            }
            return query
                .Take(15)
                .ToArray()
                .Select(x => new
                {
                    Name = x.Name,
                    AccountNumber =x.AccountNumber,
                    Id = x.Id,
                    EconomicCode=x.EconomicCode,
                    NationalId =x.NationalId,
                    Telephone =x.Telephone1
                })
                .ToArray();


        }
    }
}
