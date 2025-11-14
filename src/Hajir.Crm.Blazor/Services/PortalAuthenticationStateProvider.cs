using Blazored.LocalStorage;

using Hajir.Crm.Portal;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Hajir.Crm.Blazor.Services
{
    public interface IPortalAuthenticationStateProvider
    {
        Task UpdateAuthenicationState(PortalUser user);
    }
    internal class PortalAuthenticationStateProvider : AuthenticationStateProvider, IPortalAuthenticationStateProvider
    {
        //private readonly IBlazorAppServices appServices;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ILocalStorageService localStorage;
        private readonly IServiceProvider serviceProvider;

        public PortalAuthenticationStateProvider(ILocalStorageService localStorage, IServiceProvider serviceProvider)
        {
            this.localStorage = localStorage;
            this.serviceProvider = serviceProvider;
            //this.appServices = appServices;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                await Task.CompletedTask;
                //var se = await this.appServices.ReadUserAccountModel(null);
                var user = await this.localStorage.GetItemAsync<PortalUser>("user");
                if (user != null)
                {
                    //Console.WriteLine(se.UserId);
                    this.serviceProvider.GetState<PortalUser>().SetState(user);
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, user.UserName),

                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.Hash, "babak"),
                    }, "JwtAuth")));
                }

            }
            catch (Exception err)
            {
                return new AuthenticationState(_anonymous);
            }
            return new AuthenticationState(_anonymous);

        }
        public async Task UpdateAuthenicationState(PortalUser user)
        {
            ClaimsPrincipal claimsPrincipal = _anonymous;
            if (user != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role,user.Role)
                }, "JwtAuth"));
                //await this.appServices.WriteUserAccountModel(session);
                await this.localStorage.SetItemAsync("user", user);
                this.serviceProvider.GetState<PortalUser>().SetState(user);


            }
            else
            {
                await this.localStorage.RemoveItemAsync("user");
                this.serviceProvider.GetState<PortalUser>().SetState(new PortalUser());
                //await this.appServices.RemoveUserAccount();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
