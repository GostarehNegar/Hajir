using GN.Library.AI.Agents;
using GN.Library.AI.Tools;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using GN.Library.Xrm;
using Hajir.Crm;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class AccountInformationToolOptions
    {

    }
    internal class AccountInformationTool : BaseTool
    {
        public AccountInformationTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata("account_information")
        {
            description = "gives further information about accounts (companies)",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name = "account_id",
                    description="account id",
                    type="string",
                    required=true,
                }
            },
            returns =
            {
                {"type","object"},
                {"description","Furtrhe information about the account." }

            }


        }.Validate();

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            await Task.CompletedTask;
            var _account_id = context.GetParameterValue<string>(this.MetaData.parameters[0].name);
            var account_id = !string.IsNullOrWhiteSpace(_account_id) && Guid.TryParse(_account_id, out var _id)
                ? _id
                : (Guid?)null;
            if (!account_id.HasValue)
            {
                throw new Exception("Invalid Account Id");
            }
            var dataServices = this.serviceProvider.GetService<IXrmDataServices>();
            var account = dataServices.GetRepository<XrmHajirAccount>()
                .Queryable
                .FirstOrDefault(x => x.AccountId == account_id);
            try
            {

                if (account == null)
                {
                    throw new Exception($"Not Found");
                }
                var contacts = dataServices.GetRepository<XrmHajirContact>()
                    .Queryable
                    .Where(x => x.AccountId == account_id)
                    .ToArray();
                var quotes = dataServices.GetRepository<XrmHajirQuote>()
                    .Queryable
                    .Where(x => x.AccountId == account_id).ToArray();
                var opportunities = dataServices.GetRepository<XrmHajirOpportunity>()
                    .Queryable
                    .Where(x => x.AccountId == account_id).ToArray();

                return new
                {
                    Name = account.Name,
                    AccountNumber = account.AccountNumber,
                    Phone = account.Telephone1,
                    Address = $"{account.Address1_Line1} {account.Address1_Line2} {account.Address1_Line3}",
                    City = account.Address1_City,
                    Owner = account.Owner.Name,
                    Contacts = contacts.Select(x => new
                    {
                        FullName = x.FullName,
                        MobilePhone = x.MobilePhone,
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    }).ToArray(),
                    Quotes = quotes.OrderByDescending(x => x.CreatedOn).Select(x => new
                    {
                        QuoteNumber = x.QuoteNumber,
                        Date = HajirCrmAbstractExtensions.FormatPersianDate(x.CreatedOn),
                        Amount = x.TotalAmount
                    }).ToArray(),
                    Opportunities = opportunities.OrderByDescending(x => x.CreatedOn).Select(x => new
                    {
                        Topic = x.Topic,
                        Revenue =x.EstimavetRevenue,
                    }).ToArray(),
                };
            }
            catch (Exception err)
            {
                throw;
            }

        }
    }
}
