using Hajir.Crm.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{
    public partial class SelectProduct
    {
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Parameter]
        public Product Product { get; set; } = new Product();
        [Parameter]
        public string Label { get; set; }
        [Parameter]
        public string Placeholder { get; set; }

        public IProductBundlingService BundlingService => this.ServiceProvider.GetService<IProductBundlingService>();

        public IEnumerable<Product> AllUpses => BundlingService.GetAllUpses();
        
        [Parameter]
        public Action<Product> OnSelected { get; set; } = null;
        [Parameter]
        public HajirCrmConstants.Schema.Product.ProductTypes? ProductTypes { get; set; }

        public void ValueChanged(Product product)
        {
            this.OnSelected?.Invoke(product);
        }
        public async Task<IEnumerable<Product>> SearchUps(string e)
        {
            var items = BundlingService.Products
                    .Where(x => x.ProductType == this.ProductTypes);

            if (!string.IsNullOrWhiteSpace(e))
            {
                return items
                    .Where(x => x.Name.ToLower().Contains(e.ToLower()))
                    .ToArray();

            }
            return items;
        }

        private static string ProductToString(Product e)
        {
            return e?.Name;
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
