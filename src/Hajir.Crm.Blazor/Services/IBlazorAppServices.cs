using GN.Library.Shared.Entities;
using GN.Library.Xrm;
using Hajir.Crm.Infrastructure.Xrm.Sales;
using Hajir.Crm.Sales;
using Hajir.Crm.Sales.PhoneCalls;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    //public class AccountInfoResult
    //{
    //    public DynamicEntity[] Contacts { get; set; }
    //    public DynamicEntity[] Quotes { get; set; }
    //}
    public interface IBlazorAppServices : IServiceProvider
    {
        //Task<IEnumerable<SaleContact>> Search(string searchTerm, int count = 10);
        Task StartSearch(string searchTerm, Action<DynamicEntity[]> callback, int count = 10);
        Task<IEnumerable<SaleContact>> GetContactbyAccountId(string id);
        Task<IEnumerable<SaleQuoteBySoheil>> GetQuotebyAccountId(string id);
        Task<SaleAccount> GetAccountbyAccountId(string id);
    }
    class BlazorAppServices : IBlazorAppServices, IScopedHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public BlazorAppServices(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }

        public Task InitializeAsync(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task OnAfterRenderAsync(IServiceProvider serviceProvider, bool isfirst)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task OnConnectAsync(string circuitId)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task OnDisconnectAsync(string circuitId)
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<SaleContact>> Search(string searchTerm, int count = 10)
        //{
        //    return string.IsNullOrWhiteSpace(searchTerm)
        //        ? Enumerable.Empty<SaleContact>()
        //        : await serviceProvider.GetService<XrmSalesRepository>().SearchContactsAsync(searchTerm, count);

        //}

        public async Task StartSearch(string searchTerm, Action<DynamicEntity[]> callback, int count)
        {
            var contacts = string.IsNullOrWhiteSpace(searchTerm)
                                  ? Enumerable.Empty<SaleContact>()
                                  : await serviceProvider.GetService<XrmSalesRepository>().SearchContactsAsync(searchTerm, count);
            callback(contacts.ToArray());



            var accounts = string.IsNullOrWhiteSpace(searchTerm)
                        ? Enumerable.Empty<SaleAccount>()
                        : await serviceProvider.GetService<XrmSalesRepository>().SearchAccountAsync(searchTerm, count);
            callback(accounts.ToArray());

            foreach (var contact in contacts)
            {
                var account = await serviceProvider.GetService<XrmSalesRepository>().GetAccount(contact.AccountId);
                if (account != null && accounts.Any(x => x.Id != account.Id))
                    callback(new DynamicEntity[] { account });
            }

            var quotes = string.IsNullOrWhiteSpace(searchTerm)
            ? Enumerable.Empty<SaleQuoteBySoheil>()
            : await serviceProvider.GetService<XrmSalesRepository>().SearchQuoteAsync(searchTerm, count);
            callback(quotes.ToArray());

            //foreach (var quote in quotes)
            //{
            //    var account = await serviceProvider.GetService<XrmSalesRepository>().GetAccount(.AccountId);
            //    if (account != null && accounts.Any(x => x.Id != account.Id))
            //        callback(new DynamicEntity[] { account });
            //}




        }

        public async Task<IEnumerable<SaleContact>> GetContactbyAccountId(string id)
        {
            var service = serviceProvider.GetService<XrmSalesRepository>();

            var contacts = await service.SearchContactByAccountIdAsync(id);

            return contacts;

        }
        public async Task<IEnumerable<SaleQuoteBySoheil>> GetQuotebyAccountId(string id)
        {
            var service = serviceProvider.GetService<XrmSalesRepository>();

            var quotes = await service.SearchQuoteByAccountIdAsync(id);

            return quotes;

        }

        public async Task<SaleAccount> GetAccountbyAccountId(string id)
        {
            var service = serviceProvider.GetService<XrmSalesRepository>();

            var accounts = await service.GetAccount(id);

            return accounts;
        }
    }
}
