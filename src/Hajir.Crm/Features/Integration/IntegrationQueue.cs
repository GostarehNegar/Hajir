using GN;
using GN.Library.Data;
using GN.Library.Data.Lite;
using GN.Library.Shared.Entities;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationEntry
    {
        public string Id { get; set; }
        public string LogicalName { get; set; }
        public int Status { get; set; }
    }
    public interface IIntegrationQueue
    {
        Task<int> Enqueue(DynamicEntity[] items);
        Task<IEnumerable<DynamicEntity>> GetNext(int count = 100, int status = 1);
        Task<int> UpdateStatus(DynamicEntity item, int status = 1);
        Task<Tuple<int, int>> GetStats(int status = 1);
    }

    internal class IntegrationQueue
    {
        IRepository<string, IntegrationEntry> repo;
        public IntegrationQueue(IServiceProvider serviceProvider)
        {
            repo = serviceProvider.CreateScope()
                .ServiceProvider
                .GetService<ILocalDocumentStore>()
                .GetRepository<string, IntegrationEntry>();
        }
        public void Upsert(string id, string logicalName)
        {

            this.repo.Upsert(new IntegrationEntry { Id = id, LogicalName = logicalName });

        }
        public void Enqueue(DynamicEntity[] items)
        {
            foreach (var item in items)
            {
                if (this.repo.Get(item.Id) == null)
                    Upsert(item.Id, item.LogicalName);
            }
        }

    }

    public class IntegrationQueueEx : LiteDatabaseEx, IIntegrationQueue
    {
        public IntegrationQueueEx(HajirIntegrationOptions opt) : base(opt.GetQueueStorageConnectionString())
        {

        }
        async Task<DisposableCollection<IntegrationEntry>> GetCollection()
        {
            var result = await this.GetCollection<IntegrationEntry>(false, default);
            //result.Collection.EnsureIndex(x => x.AsteriskId, unique: true);
            //result.Collection.EnsureIndex(x => x.AsteriskFullPath, unique: true);
            return result;
        }
        public async Task<int> Enqueue(DynamicEntity[] items)
        {
            var result = 0;
            using (var col = await this.GetCollection())
            {
                foreach (var item in items)
                {
                    if (col.Collection.FindById(new BsonValue(item.Id)) == null)
                    {
                        col.Collection.Upsert(new IntegrationEntry { Id = item.Id, LogicalName = item.LogicalName });
                        result++;
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<DynamicEntity>> GetNext(int count = 100, int status = 1)
        {
            using (var c = await this.GetCollection())
            {
                return c.Collection.Find(x => x.Status < status, limit: count)
                    .Select(x => new DynamicEntity { Id = x.Id, LogicalName = x.LogicalName })
                    .ToArray();
            }
        }

        public async Task<int> UpdateStatus(DynamicEntity item, int status = 1)
        {
            var result = -1;
            using (var c = await this.GetCollection())
            {
                var _item = c.Collection.FindById(new BsonValue(item.Id));
                if (_item == null)
                {
                    throw new Exception($"Item Not Found :{item}");
                }
                if (_item.Status < status)
                {
                    _item.Status = status;
                    c.Collection.Update(_item);
                }
                return _item.Status;
            }
            throw new NotImplementedException();
        }

        public async Task<Tuple<int, int>> GetStats(int status =1)
        {
            using (var c = await this.GetCollection())
            {
                var total = c.Collection.Count();
                var remaining = c.Collection.Count(x => x.Status >= status);
                return Tuple.Create(total, remaining);

            }

            
        }
    }

}
