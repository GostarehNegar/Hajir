using GN.Library.AI.Agents;
using GN.Library.ServerBase;
using GN.Library.Xrm;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm;
using Hajir.Crm.Sales.PriceLists;
using Hajir.AI.Agents;
using Hajir.AI.Agents.Tools;
using GN.Library.Nats;
using GN.Dynamic.Communication.Messaging.SMS.Providers.MelliPayamak;

namespace Hajir.AI.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProgramBaseEx.Run(args, o =>
            o.AsConsole()
            .WithServiceName("Hajir.AI.Service","Hajir AI Service")
            .WithServices((c, s) => {


                //s.AddAiAgentsServices(c, opt => { });
                s.AddXrmServices(c, cfg =>
                {
                    cfg.ConnectionOptions = ConnectionOptions.OrganizationService;
                });
                s.AddNatsServices(c, cfg => { });
                s.AddHajirCrm(c, cfg => { });
                s.AddHajirPriceListServices(c, opt => { });
                s.AddHajirSalesInfrastructure(c);
                s.AddHairAgents();
                s.AddSerarchContactTool(new SearchContactToolOptions { })
                 .AddRegisterPhoneCallTool(new RegisterPhoneCallToolOptions { })
                 .AddCreateOpportunityCallTool(new CreateOpportunityToolOptions { })
                 .AddListOpportunityTool(new ListOpportunityToolOptions { })
                 .AddSearchProductTool(new SearchProdcutsToolOptions { })
                 .AddSearchAccountTool(new AccountSearchToolOptions { })
                 .AddAccountInformationTool(new AccountInformationToolOptions { })
                 .AddPriceCaomparisonTool(new ProductPriceComparisonToolOptions { })
                 .AddReminderTool(new CreateReminderToolOptions { })
                 .AddCreateAccountTool(new CreateAccountToolOptions { })
                 .AddMyTasksTool(new MyTasksToolOptions { });
                s.AddSingleton< MelliPayamakSMSProvider>()
                .AddHostedService<SendSMSTool>();


            }));

            return;
            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.

            //builder.Services.AddControllers();
            //builder.Services.AddAiAgentsServices(builder.Configuration, opt => { });

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.

            //app.UseAuthorization();


            //app.MapControllers();

            //app.Run();
        }
    }
}