using GN.Library.TaskScheduling;
using Hajir.Crm.Features.Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationBackgroundServiceEx : BackgroundMultiBlockingTaskHostedService
    {
        private readonly ILogger<IntegrationBackgroundServiceEx> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly HajirIntegrationOptions options;
        private int _total;
        private int _success;
        private int _failure;


        public IntegrationBackgroundServiceEx(ILogger<IntegrationBackgroundServiceEx> logger, IServiceProvider serviceProvider, HajirIntegrationOptions options)
            : base(4, 100)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.options = options;

        }
        private IntegrationServiceContext CTX;
        public bool DoEnqueue(object item, int timeout, CancellationToken token)
        {
            //this.CTX = this.CTX?? new IntegrationServiceContext(this.serviceProvider, "Import Context", token, item);
            //var ctx = this.CTX;
            var ctx = new IntegrationServiceContext(this.serviceProvider, "Import Context", token, item);
            var result = true;
            return this.Enqueue(async t =>
            {
                try
                {
                    //await ctx.Import();
                    switch (item)
                    {
                        case IntegrationContact contact:
                            await ctx.ImportLegacyContact(contact);
                            break;
                        case IntegrationAccount account:
                            await ctx.ImportAccount(account);
                            break;
                        case IntegrationQuote quote:
                            await ctx.ImportQuote(quote);
                            break;
                        default:
                            break;
                    }
                    Interlocked.Increment(ref this._success);
                }
                catch (Exception ex)
                {
                    //this.logger.LogError(
                    //    $"An error occured while trying to import item:'{item}'. Err:{ex.Message}");
                    Interlocked.Increment(ref this._failure);
                }
                finally
                {
                    ctx.Dispose();
                }
            }, timeout, token);

            return result;

        }

        private async Task ImportGoods(ILegacyCrmStore store, CancellationToken stoppingToken)
        {

        }
        private async Task ImportAccounts(ILegacyCrmStore store, CancellationToken stoppingToken)
        {
            await Task.Delay(1000);
            int skip = 0;
            int take = 100;
            //var store = this.serviceProvider.GetService<ILegacyCrmStore>();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var accounts = store.ReadAccounts(skip, take).ToArray();//.Where(x => x.State == 0).ToArray();
                    var no = 0;
                    foreach (var account in accounts.Where(x => x.State == 0))
                    {
                        this.DoEnqueue(account, 60 * 60 * 1000, stoppingToken);
                        no++;
                        if (stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    if (accounts.Length < take)
                    {
                        break;
                    }
                    skip += take;
                    this.logger.LogInformation($"{no} Accounts Enqueued.");
                    await Task.Delay(20  * 1000);

                }
                catch (Exception err)
                {
                    this.logger.LogError(
                       $"An error occured wile trying to import legacy accounts. Err:{err.Message}");
                    await Task.Delay(10 * 1000, stoppingToken);
                }
            }
        }

        private async Task ImportContacts(ILegacyCrmStore store, CancellationToken stoppingToken)
        {
            await Task.Delay(1000);
            int skip = 0;
            int take = 100;
            //var store = this.serviceProvider.GetService<ILegacyCrmStore>();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var items = store.ReadContacts(skip, take).ToArray();//.Where(x => x.State == 0).ToArray();
                    var no = 0;
                    foreach (var item in items.Where(x => x.State == 0))
                    {
                        this.DoEnqueue(item, 60 * 60 * 1000, stoppingToken);
                        no++;
                        if (stoppingToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    if (items.Length < take)
                    {
                        break;
                    }
                    skip += take;
                    this.logger.LogInformation($"{no} Contacts Enqueued.");
                    await Task.Delay(20 * 1000);

                }
                catch (Exception err)
                {
                    this.logger.LogError(
                       $"An error occured wile trying to import legacy contacts. Err:{err.Message}");
                    await Task.Delay(10 * 1000, stoppingToken);
                }
            }
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();
            await Task.Delay(1000);
            using (var scope = this.serviceProvider.CreateScope())
            {
                var store = scope.ServiceProvider.GetService<ILegacyCrmStore>();
                var total = store.GetContatCount() + store.GetAccountsCount();
                tasks.Add(ImportAccounts(store, stoppingToken));
                tasks.Add(ImportContacts(store, stoppingToken));
                tasks.Add(Task.Run(async () =>
                {
                    var a = new System.Diagnostics.Stopwatch();
                    a.Start();
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        this.logger.LogInformation(
                            $"{_success} of {total} items (%{_success * 100 / total}) Processed in {a.ElapsedMilliseconds / 1000} Seconds. {(double)_success * 1000 / (double)a.ElapsedMilliseconds:0.00}. Queue: {this.Total} ");
                        await Task.Delay(5 * 1000, stoppingToken);
                    }
                }));

                await Task.WhenAll(tasks);
            }
        }

    }
}
