using GN.Library.AI.Agents;
using GN.Library.AI.Tools;
using GN.Library.Nats;
using GN.Library.Shared.AI.Agents;
using GN.Library.Xrm.StdSolution;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Agents.Tools
{
    public class CreateAccountToolOptions
    {

    }
    internal class CreateAccountTool : HajirBaseTool
    {
        public const string Name = "create_account";

        public CreateAccountTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override ToolMetadata MetaData => new ToolMetadata(Name)
        {
            description = "creates an account (company) in CRM.",
            parameters = new ToolParameter[]
            {
                new ToolParameter
                {
                    name= "account_name",
                    description = "the name of company",
                    required = true,
                    type="string"
                },
                new ToolParameter
                {
                    name ="telephone",
                    description="company telephone number",
                    type="string",
                    required = true
                },
                 new ToolParameter
                {
                    name ="description",
                    description="a simple description of the company",
                    type="string",
                    required = true
                },
                new ToolParameter
                {
                    name ="type",
                    description=@"type of the account as a number:
1 موسسه
2 شرکت خصوصی
3 سازمان 
4 فروشگاه
5 آموزشگاه
6 کارخانه
7 وزارتخانه
",
                    type="int",
                    required = true
                },
            }

        }.Validate();

        protected override async Task<object> Handle(ToolInvokeContext context, NatsExtensions.IMessageContext ctx)
        {
            var name = context.GetParameterValue<string>(this.MetaData.parameters[0].name);
            var telephone = context.GetParameterValue<string>(this.MetaData.parameters[1].name);
            var description = context.GetParameterValue<string>(this.MetaData.parameters[2].name);
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Account Name is required.");
            }
            var id = this.GetRepository<XrmAccount>()
                .Insert(new XrmAccount
                {
                    Name = name,
                    Telephone1 = telephone,
                    ["description"] = description


                });

            return new
            {
                Successfull = true,
                AccountId = id.ToString()
            };
        }
    }
}
