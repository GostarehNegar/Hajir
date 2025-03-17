using Hajir.Crm.Blazor.Models;
using Hajir.Crm.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{
    public partial class AddBundleWizardUPSStep
    {
        [Parameter]
        
        public Product UPS { get; set; }
        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();

        public IEnumerable<Product> AllUpses => BundlingService.GetAllUpses();


        public async Task<IEnumerable<Product>> SearchUps(string e)
        {
            
            if (!string.IsNullOrWhiteSpace(e))
                return AllUpses.Where(x => x.Name.ToLower().Contains(e.ToLower()));
            return AllUpses;
        }

        private static string ProductToString(Product e)
        {
            return e?.Name;
        }
        public void SelectUPS(Product ups)
        {
            this.State.SetState(x => {

                x.Bundle.UPS = ups;
                x.Step = 2;
            
            });
        }

        private void ClearUps()
        {
            //BundleModel = new BundleEditModel();
            //DesignedBundle = new CabinetSet(null);
            //CabinetDesign = Array.Empty<CabinetSet>();
            //Battery = new Product();
            //HasBattery = HasCabinet = HasParallel = HasSNMP = false;
        }

    }
}
