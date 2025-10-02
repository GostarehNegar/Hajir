
using Hajir.Crm.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.UserAccount
{
    public partial class UserLoginComponent
    {
        [Inject]
        public IServiceProvider ServiceProvider { get; set; }
        public string UserName { get; set; } = "Admin@1403#CRM%@hsco.local";
        public string Password { get; set; } = "P@ssw0rd";// "Admin@1403#CRM%"
        public async Task Login()
        {
            await this.ServiceProvider.GetService<IPortalAuthenticationService>()
                .Authenticate(this.UserName, this.Password);

        }
    }
}
