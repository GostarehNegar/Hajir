using Hajir.Crm.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.UserAccount
{
    public partial class UserAccountComponent
    {
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }
        
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var user = await AuthenticationState;


            await base.OnParametersSetAsync();
        }
        public async Task Logout()
        {
            await this.ServiceProvider.GetService<IPortalAuthenticationStateProvider>()
                .UpdateAuthenicationState(null);
        }
        public void Dispose()
        {

            throw new NotImplementedException();
        }
    }
}
