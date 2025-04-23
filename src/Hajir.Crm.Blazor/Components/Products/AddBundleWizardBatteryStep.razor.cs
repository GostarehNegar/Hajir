using Hajir.Crm.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{
    public partial class AddBundleWizardBatteryStep
    {
        ProductBundle Bunlde => this.State.Value.Bundle;
        public int NumberOfBatteries
        {
            get
            {
                return this.Bunlde.BatteryCount ?? 0;


            }
            set
            {
                
                this.Bunlde.SetBattery(this.Bunlde.Battery, value);
            }
        }
        public void SelectBattery(Product ups)
        {
            this.State.SetState(x => {

                x.Bundle.SetBattery(ups,0);
                x.Step = 2;

            });
        }
        public int[] GetSupportedNumberOfBatteries()
        {
            var ups = this.State.Value.Bundle.UPS;
            if (ups != null)
            {
                var specs = ups.GetSupportedBatteryConfig();
                return specs.Select(x => x.Number)
                    .ToArray();

            }

            return new int[] { };
        }
    }
}
