using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.TaskScheduling
{
    class BackgroundBlockingTaskHostedService : BackgroundService
    {
        private BlockingCollection<Func<CancellationToken, Task>> queue;
        private int count;

        public BackgroundBlockingTaskHostedService(int capacity = 0)
        {
            this.queue = capacity < 1
                ? new BlockingCollection<Func<CancellationToken, Task>>()
                : new BlockingCollection<Func<CancellationToken, Task>>(capacity);
        }
        public bool Enqueue(Func<CancellationToken, Task> producer)
        {
            count++;
            return this.queue.TryAdd(producer);
        }
        public bool Enqueue(Func<CancellationToken, Task> producer, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            count++;
            return this.queue.TryAdd(producer, millisecondsTimeout, cancellationToken);
        }
        public int Count => this.count;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (this.queue.TryTake(out var item, 2000, stoppingToken))
                    {
                        if (item != null)
                        {
                            try
                            {
                                await item.Invoke(stoppingToken);
                            }
                            catch (Exception err)
                            {
                                //throw;
                            }
                            count--;
                        }
                    }
                }
            });
        }
    }
    public class BackgroundMultiBlockingTaskHostedService : BackgroundService
    {
        private List<BackgroundBlockingTaskHostedService> queues;

        public BackgroundMultiBlockingTaskHostedService(int no = 1, int capacity = 0)
        {
            this.queues = Enumerable.Range(0, no)
                .Select(x => new BackgroundBlockingTaskHostedService(capacity))
                .ToList();
        }
        public int Total => this.queues.Sum(x => x.Count);

        public virtual bool Enqueue(Func<CancellationToken, Task> producer)
        {

            return this.queues.OrderBy(x => x.Count).First().Enqueue(producer);
        }
        public virtual bool Enqueue(Func<CancellationToken, Task> producer, int millisecondsTimeout, CancellationToken cancellationToken)
        {

            return this.queues.OrderBy(x => x.Count).First().Enqueue(producer, millisecondsTimeout, cancellationToken);
        }

        public async override Task  StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            await Task.WhenAll(this.queues.Select(x => x.StartAsync(cancellationToken)));
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {

            await Task.WhenAll(this.queues.Select(x => x.StopAsync(cancellationToken)));
            await base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
