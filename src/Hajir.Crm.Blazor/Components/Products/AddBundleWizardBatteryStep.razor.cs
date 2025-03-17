using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{
    public partial class AddBundleWizardBatteryStep
    {
        public int NumberOfBatteries { get; set; }
        public int[] GetSupportedNumberOfBatteries()
        {
            return new int[] { 1, 2, 3 };
        }
    }
}
