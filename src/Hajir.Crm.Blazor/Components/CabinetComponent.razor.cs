using Hajir.Crm.Features.Products;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components
{
    public partial class CabinetComponent
    {
        [Parameter]
        public Cabinet Cabinet { get; set; }

        public int NumberOfColumns => this.Cabinet?.NumberOfColumns ?? 0;
        public int NumberOfRows => this.Cabinet?.NumberOfRows ?? 0;

        public string GetClass(int i, int j)
        {
            if (this.Cabinet.GetLocation(i, j).Filled)
                return "filled cell";
            return "cell";
        }

    }
}
