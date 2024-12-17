using GN.Library.Shared.Entities;
using GN.Library.TaskScheduling;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        private IIntegrationQueue _queue;
        private int targetVersion = 1;
        private ConcurrentDictionary<string, object> items = new ConcurrentDictionary<string, object>();



        public IntegrationBackgroundServiceEx(ILogger<IntegrationBackgroundServiceEx> logger, IServiceProvider serviceProvider, HajirIntegrationOptions options)
            : base(4, 100)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.options = options;
            this._queue = serviceProvider.GetService<IIntegrationQueue>();//  new IntegrationQueue();

        }
        private IntegrationServiceContext CTX;
        public bool DoEnqueue(object item, int timeout, CancellationToken token)
        {
            //this.CTX = this.CTX?? new IntegrationServiceContext(this.serviceProvider, "Import Context", token, item);
            //var ctx = this.CTX;
            if (item is DynamicEntity de)
            {
                if (this.items.TryGetValue(de.Id, out var _))
                    return true;
                this.items.TryAdd(de.Id, de);
            }

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
                    await this._queue.UpdateStatus(item as DynamicEntity, 1);
                    this.items.TryRemove((item as DynamicEntity).Id, out var _);
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
                    await Task.Delay(20 * 1000);

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


        private async Task<int> EnqueueItems(string logicalName, ILegacyCrmStore store, CancellationToken stoppingToken)
        {
            await Task.Delay(1000);
            int skip = 0;
            int take = 100;
            int result = 0;
            //var store = this.serviceProvider.GetService<ILegacyCrmStore>();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var items = store.ReadItems(logicalName, skip, take).ToArray();//.Where(x => x.State == 0).ToArray();
                    var no = await this._queue.Enqueue(items);
                    result += no;
                    //var no = 0;
                    //foreach (var item in items.Where(x => x.State == 0))
                    //{

                    //    this.DoEnqueue(item, 60 * 60 * 1000, stoppingToken);
                    //    no++;
                    //    if (stoppingToken.IsCancellationRequested)
                    //    {
                    //        break;
                    //    }
                    //}
                    if (items.Length < take)
                    {
                        break;
                    }
                    skip += take;
                    this.logger.LogInformation($"{no} {logicalName} Enqueued.");
                    await Task.Delay(5 * 1000);

                }
                catch (Exception err)
                {
                    this.logger.LogError(
                       $"An error occured wile trying to enque items. Err:{err.Message}");
                    await Task.Delay(10 * 1000, stoppingToken);
                }
            }
            return result;
        }

        private async Task EnqueueItems(ILegacyCrmStore store, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var tasks = new List<Task>();
                tasks.Add(this.EnqueueItems("contact", store, stoppingToken));
                tasks.Add(this.EnqueueItems("account", store, stoppingToken));
                tasks.Add(this.EnqueueItems("quote", store, stoppingToken));
                //tasks.Add(this.EnqueueItems("systemuser", store, stoppingToken));
                await Task.WhenAll(tasks);
                await Task.Delay(5 * 1000 * 60);
            }
        }


        private async Task DequeueItems(ILegacyCrmStore store, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var items = await this._queue.GetNext();
                    foreach (var item in items)
                    {
                        try
                        {
                            DynamicEntity target = null;
                            switch (item.LogicalName)
                            {
                                case "contact":
                                    target = store.GetContact(item.Id);
                                    break;
                                case "account":
                                    target = store.GetAccount(item.Id);
                                    break;
                                case "quote":
                                    target = store.GetQuote(item.Id);
                                    break;
                                case "systemuser":
                                    target = store.GetUser(item.Id);
                                    break;
                                default:
                                    throw new Exception($"Invalid Logical Name:{item.LogicalName}");
                                    break;
                            }
                            if (!this.DoEnqueue(target, 10 * 60 * 1000, stoppingToken))
                            {

                            }
                        }
                        catch (Exception err)
                        {
                            this.logger.LogError(
                                $"An error occured while trying to Dequeue item. Err:{err.Message}");
                        }
                    }

                    this.logger.LogInformation($"{items.Count()} Items Dequeued.");
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        $"An error occured while trying to Dequeue items. Err:{err.Message}");
                }
                await Task.Delay(2000, stoppingToken);
            }
        }
        private async Task ImportQuotes(ILegacyCrmStore store, CancellationToken stoppingToken)
        {
            await Task.Delay(1000);
            int skip = 0;
            int take = 10000;
            //var store = this.serviceProvider.GetService<ILegacyCrmStore>();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var _items = store.ReadItems(XrmAccount.Schema.LogicalName, skip, take);
                    this._queue.Enqueue(_items.ToArray());
                    var items = store.ReadQuotes(skip, take).ToArray();//.Where(x => x.State == 0).ToArray();


                    var no = 0;
                    foreach (var item in items.Where(x => x.State == 0))
                    {
                        //this.DoEnqueue(item, 60 * 60 * 1000, stoppingToken);
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
                    await Task.Delay(1 * 1000);

                }
                catch (Exception err)
                {
                    this.logger.LogError(
                       $"An error occured wile trying to import legacy contacts. Err:{err.Message}");
                    await Task.Delay(10 * 1000, stoppingToken);
                }
            }
        }


        private async Task EnsureLookUpTables(IIntegrationStore store, CancellationToken stoppingToken)
        {
            
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoData>(File.ReadAllText("geo.dat"));
            store.ImportGeoData(data);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();
            await Task.Delay(1000);

            


            using (var scope = this.serviceProvider.CreateScope())
            {
                await EnsureLookUpTables(scope.ServiceProvider.GetService<IIntegrationStore>(), stoppingToken);
                var store = scope.ServiceProvider.GetService<ILegacyCrmStore>();
                //var total = store.GetContatCount() + store.GetAccountsCount();
                //tasks.Add(ImportAccounts(store, stoppingToken));
                //tasks.Add(ImportContacts(store, stoppingToken));
                //tasks.Add(ImportQuotes(store, stoppingToken));
                tasks.Add(EnqueueItems(store, stoppingToken));
                tasks.Add(DequeueItems(store, stoppingToken));
                tasks.Add(Task.Run(async () =>
                {
                    var a = new System.Diagnostics.Stopwatch();
                    a.Start();
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var stats = await this._queue.GetStats(this.targetVersion);
                        if (stats.Item1 > 0)
                        {
                            this.logger.LogInformation(
                                $"********** {stats.Item2} of {stats.Item1} Imported ({stats.Item2 * 100 / stats.Item1}%). Failures:'{this._failure}', Success:'{this._success}' Queue:{this.items.Count()}, Rate:{(double)_success * 1000 / (double)a.ElapsedMilliseconds:0.00}");
                        }
                        //this.logger.LogInformation(
                        //    $"{_success} of {total} items (%{_success * 100 / total}) Processed in {a.ElapsedMilliseconds / 1000} Seconds. {(double)_success * 1000 / (double)a.ElapsedMilliseconds:0.00}. Queue: {this.Total} ");
                        await Task.Delay(5 * 1000, stoppingToken);
                    }
                }));

                await Task.WhenAll(tasks);
            }
        }

    }
}
