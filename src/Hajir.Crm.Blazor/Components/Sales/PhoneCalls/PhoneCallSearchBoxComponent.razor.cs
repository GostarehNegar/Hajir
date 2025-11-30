using GN.Library.Shared.Entities;
using Hajir.Crm.Blazor.Services;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hajir.Crm.Blazor.Components.Sales.PhoneCalls
{
    public partial class PhoneCallSearchBoxComponent
    {
        [Parameter] 
        public EventCallback<DynamicEntity> OnItemSelected { get; set; }

        [Inject]
        public IBlazorAppServices Api { get; set; }

        private List<DynamicEntity> searchEntities { get; set; } = new();
        private bool isSearching = false;

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = (value);
                this.isSearching = true;
                this.StateHasChanged();
                this.searchEntities.Clear();
                this.AppServices.StartSearch(value, x =>
                {


                    this.searchEntities.AddRange(x.Where(i => i != null).Take(10));
                    this.isSearching = false;
                    this.InvokeAsync(this.StateHasChanged);
                });


            }
        }


 
        public async Task ItemClicked(DynamicEntity entity)
        {
            await OnItemSelected.InvokeAsync(entity);
            this.searchEntities.Clear();

        }

    }
}
