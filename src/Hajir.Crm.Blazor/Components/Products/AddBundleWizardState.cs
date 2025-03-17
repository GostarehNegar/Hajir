using Hajir.Crm.Products;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Products
{
    public class AddBundleWizardState
    {
        public int Step { get; set; } = 1;
        public ProductBundle Bundle { get; set; } = new ProductBundle();

        public string StepName
        {
            get
            {
                switch (Step)
                {
                    case 3:
                        return "ویزارد انتخاب محصول گام سوم: انتخاب کابینت";
                    case 2:
                        return "ویزارد انتخاب محصول گام دوم: انتخاب باتری";
                    case 1:
                    default:
                        return "ویزارد انتخاب محصول گام اول: انتخاب یوپی‌اس";
                        
                }
            }
        }
    }
}
