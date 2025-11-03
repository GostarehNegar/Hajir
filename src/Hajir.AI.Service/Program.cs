using GN.Library.AI.Agents;
using GN.Library.ServerBase;
using GN.Library.Xrm;
using Microsoft.Extensions.DependencyInjection;
using Hajir.Crm;
using Hajir.Crm.Sales.PriceLists;
using Hajir.AI.Agents;
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


                s.AddAiAgentsServices(c, opt => { });
                s.AddXrmServices(c, cfg =>
                {
                    cfg.ConnectionOptions = ConnectionOptions.WebAPI;
                });
                s.AddHajirCrm(c, cfg => { });
                s.AddHajirPriceListServices(c, opt => { });
                s.AddHajirSalesInfrastructure(c);
                s.AddHairAgents();

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