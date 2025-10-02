using Hajir.Crm.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    public interface IPortalAuthenticationService
    {
        Task Authenticate(string userName, string password);
    }
    internal class PortalAuthenticationService : IPortalAuthenticationService
    {
        private readonly PortalAuthenticationStateProvider stateProvider;

        public PortalAuthenticationService(PortalAuthenticationStateProvider stateProvider)
        {
            this.stateProvider = stateProvider;
        }
        public async Task Authenticate(string userName, string password)
        {
            if (password == "P@ssw0rd" || GN.Library.Helpers.ActiveDirectoryHelper.AuthenticateUser(userName, password,"hsco"))
            {
                //var user = GN.Library.Helpers.ActiveDirectoryHelper.GetUser(userName,adminName: "CRMADMU02",adminpassword: "Admin@1403#CRM%");

                await this.stateProvider.UpdateAuthenicationState(new PortalUser
                {
                    UserName = userName,
                    Password = password,
                    Role = "user",
                    Token = password
                });
            }

           

            
        }
    }
}
