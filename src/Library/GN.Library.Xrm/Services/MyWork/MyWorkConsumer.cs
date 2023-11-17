using GN.Library.Shared.Chats.UserMessages;
using GN.Library.Shared.Entities;
using GN.Library.Xrm.StdSolution;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;

namespace GN.Library.Xrm.Services.MyWork
{
    class MyWorkConsumer
    {
        private ClaimsIdentity identity;
        public DateTime LastHit { get; private set; }
        private List<XrmEntity> follwoingEntities;
        private IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => this._serviceProvider;
        public Guid UserId => this.identity?.GetCrmUserId() ?? Guid.Empty;
        private ILogger logger;

        public MyWorkConsumer WithIdentity(ClaimsIdentity identity)
        {
            this.identity = identity;
            return this;
        }
        public bool Busy { get; private set; }
        public MyWorkConsumer(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;

        }
        public MyWorkConsumer Hit()
        {
            this.LastHit = DateTime.UtcNow;
            return this;
        }
        public IEnumerable<XrmEntity> GetFollowingEntities(bool refersh = false)
        {
            if (this.follwoingEntities == null || refersh)
            {
                var id = this.identity.GetCrmUserId().Value;
                this.follwoingEntities = this.ServiceProvider
                   .GetServiceEx<IXrmDataServices>()
                   .GetRepository<XrmPostFollow>()
                   .GetFollowingEntities(id)
                   .Select(x => new XrmEntity(x.LogicalName) { Id = x.Id })
                   .ToList();
            }
            return this.follwoingEntities;
        }
        public Entity[] RetrieveMultipleEx(string logicalName, DateTime dt, params Guid[] ids)
        {
            List<Entity> result = new List<Entity>();
            var skip = 0;
            var take = 200;
            var fin = false;
            while (!fin)
            {
                var _ids = ids.AsQueryable()
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                if (_ids.Count < 1)
                {
                    break;
                }
                var q = new Microsoft.Xrm.Sdk.Query.QueryExpression(logicalName);
                var filter = new FilterExpression();
                var schema = this.ServiceProvider.GetServiceEx<IXrmDataServices>()
                    .GetSchemaService()
                    .GetSchema(logicalName);
                filter.AddCondition(schema.PrimaryAttibuteName, ConditionOperator.In, _ids.Select(x => (object)x).ToArray());
                filter.AddCondition("modifiedon", ConditionOperator.GreaterEqual, (object)dt);
                q.Criteria = filter;
                q.ColumnSet = new ColumnSet(true);
                var entities = this.ServiceProvider
                    .GetServiceEx<IXrmDataServices>()
                    .GetXrmOrganizationService()
                    .GetOrganizationService()
                    .RetrieveMultiple(q);
                result.AddRange(entities.Entities);
                skip += take;

            }
            return result.ToArray();


        }
        public Entity[] RetrieveMultiple(string logicalName, params Guid[] ids)
        {
            List<Entity> result = new List<Entity>();
            var skip = 0;
            var take = 200;
            var fin = false;
            while (!fin)
            {
                var _ids = ids.AsQueryable()
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                if (_ids.Count < 1)
                {
                    break;
                }
                var q = new Microsoft.Xrm.Sdk.Query.QueryExpression(logicalName);
                var filter = new FilterExpression();
                var schema = this.ServiceProvider.GetServiceEx<IXrmDataServices>()
                    .GetSchemaService()
                    .GetSchema(logicalName);
                filter.AddCondition(schema.PrimaryAttibuteName, ConditionOperator.In, _ids.Select(x => (object)x).ToArray());
                q.Criteria = filter;
                var entities = this.ServiceProvider
                    .GetServiceEx<IXrmDataServices>()
                    .GetXrmOrganizationService()
                    .GetOrganizationService()
                    .RetrieveMultiple(q);
                result.AddRange(entities.Entities);
                skip += take;

            }
            return result.ToArray();


        }
        //public void Start(ClaimsIdentity identity, DateTime last, Action<MyUpdate> enqueue)
        //{
        //    this.identity = identity;
        //    var update = new MyUpdate();
        //    if (!this.Busy)
        //    {
        //        try
        //        {
        //            last = last < DateTime.UtcNow.AddYears(-1) ? DateTime.UtcNow.AddYears(-1) : last;





        //            var rawEntities = new List<Entity>();
        //            var updated_enties = new List<Entity>();
        //            var dataServices = this.ServiceProvider.GetServiceEx<IXrmDataServices>()
        //                .Clone(UserId);
        //            var posts = dataServices
        //                .GetRepository<XrmPost>()
        //                .GetMyPosts(this.UserId, last);
        //            posts.ToList()
        //                .ForEach(x => rawEntities
        //                            .AddRange(x.ExtractEntitiesFromPostText()
        //                            .Select(y => new Entity(y.LogicalName, y.Id))));
        //            rawEntities.AddRange(posts);
        //            rawEntities.AddRange(dataServices
        //                .GetRepository<XrmActivityPointer>()
        //                .GetMyActivities(this.UserId, last));
        //            rawEntities.AddRange(this.GetFollowingEntities());
        //            rawEntities = rawEntities.DistinctBy(x => $"{x.Id}-{x.LogicalName}").ToList();
        //            var ignore_types = new string[] { "bulkoperation", "quoteclose", "opportunityclose" };
        //            rawEntities.GroupBy(x => x.LogicalName == XrmActivityPointer.Schema.LogicalName ? (string)x[XrmActivityPointer.Schema.ActivityTypeCode] : x.LogicalName)
        //                .Select(x => x.Key)
        //                .ToList()
        //                .ForEach(t =>
        //                {
        //                    var items = rawEntities
        //                        .Where(x => x.LogicalName == t ||
        //                                (x.LogicalName == XrmActivityPointer.Schema.LogicalName && (string)x[XrmActivityPointer.Schema.ActivityTypeCode] == t))
        //                        .ToArray();
        //                    switch (t)
        //                    {
        //                        case XrmPost.Schema.LogicalName:
        //                            updated_enties.AddRange(items);
        //                            break;
        //                        case var exp when ignore_types.Contains(exp):
        //                            break;
        //                        default:
        //                            updated_enties.AddRange(this.RetrieveMultipleEx(t, last, items.Select(x => x.Id).ToArray()));
        //                            break;

        //                    }
        //                });
        //            ///
        //            /// Prepare update:
        //            /// 
        //            /// filter activities
        //            /// 
        //            bool IsActivity(Entity entity)
        //            {
        //                return entity != null && entity.GetAttributeValue<object>("activityid") != null;

        //            }
        //            bool IsMyActivity(Entity entity)
        //            {
        //                return IsActivity(entity) && entity.GetAttributeValue<EntityReference>(XrmEntity.Schema.Owner)?.Id == UserId;
        //            }
        //            bool IsPost(Entity entity)
        //            {
        //                return entity != null && entity.LogicalName == XrmPost.Schema.LogicalName;
        //            }
        //            update.MyActivities = updated_enties
        //                    .Where(x => IsMyActivity(x))
        //                    .Select(x => x.ToDynamic())
        //                    .ToArray();
        //            update.MyPosts = updated_enties
        //                .Where(x => IsPost(x))
        //                .Select(x => x.ToDynamic())
        //                .ToArray();
        //            update.Entities = updated_enties
        //                .Where(x => !IsActivity(x) && !IsPost(x))
        //                .Select(x => x.ToDynamic())
        //                .ToArray();
        //            update.Untill = DateTime.UtcNow.AddMinutes(-1).Ticks;

        //        }
        //        catch (Exception err)
        //        {

        //        }




        //    }
        //    this.Busy = true;

        //}

        public Task GetUpdate(GetEntityUpdate request, Func<MyUpdate, Task> send)
        {
            return Task.Run(async () =>
            {
                using (var dataService = this.ServiceProvider.GetServiceEx<IXrmDataServices>().Clone(UserId))
                {
                    if (request == null || request.Entity == null || string.IsNullOrWhiteSpace(request.Entity.Id)
                        || !Guid.TryParse(request.Entity.Id, out var id))
                    {
                        throw new Exception($"Invalid GetUpdate Request:{request}");
                    }

                    try
                    {
                        var entity = dataService.GetXrmOrganizationService()
                            .GetOrganizationService()
                            .Retrieve(request.Entity.LogicalName, id, new ColumnSet(true));
                        if (entity == null)
                        {
                            throw new Exception($"Not Found");
                        }
                        var isActivity = entity.GetAttributeValue<Guid?>("activityid").HasValue;

                        var update = new MyUpdate
                        {
                            Entities = !isActivity ? new DynamicEntity[] { entity.ToDynamic() } : new DynamicEntity[] { },
                            Activities = isActivity ? new DynamicEntity[] { entity.ToDynamic() } : new DynamicEntity[] { }
                        };
                        await send(update);
                    }
                    catch (Exception err)
                    {
                        this.logger?.LogError(
                            $"An error occured while trying to handle getupdate. {err.GetBaseException().Message}");
                        throw;
                    }

                }
            });


        }
        public Task StartEx(ClaimsIdentity identity, long since1, Func<MyUpdate, Task> send, CancellationToken cancellationToken)
        {
            this.identity = identity;
            this.logger = this.ServiceProvider.GetServiceEx<ILoggerFactory>().CreateLogger("MyWorkConsumer");
            var userId = identity?.GetCrmUserId();
            var since = new DateTime(since1, DateTimeKind.Utc);
            since = since < DateTime.UtcNow.AddYears(-1) ? DateTime.UtcNow.AddYears(-1) : since;
            if (this.Busy)
                return Task.CompletedTask;
            this.Busy = true;
            if (!userId.HasValue)
            {
                throw new Exception("Invalid UserId");
            }
            return Task.Run(async () =>
            {
                await Task.Delay(10);
                var fin = false;
                try
                {
                    using (var dataService = this.ServiceProvider.GetServiceEx<IXrmDataServices>().Clone(userId.Value))
                    {
                        var pageSize = 100;
                        var pageNumber = 1;
                        while (!fin)
                        {
                            List<EntityReference> relatedEntities = new List<EntityReference>();
                            if (cancellationToken.IsCancellationRequested)
                                break;
                            var posts = ((RetrievePersonalWallResponse)dataService
                                .GetXrmOrganizationService()
                                .GetOrganizationService()
                                .Execute(new RetrievePersonalWallRequest
                                {
                                    CommentsPerPost = 5,
                                    PageSize = pageSize,
                                    PageNumber = pageNumber,
                                    StartDate = since,
                                }))
                                .EntityCollection
                                .Entities
                                .Select(x => x.ToXrmEntity<XrmPost>());
                            foreach (var post in posts)
                            {
                                relatedEntities.Add(post.RegardingObjectId);
                                relatedEntities.AddRange(post.ExtractEntitiesFromPostText());
                            }
                            PackedDynamicEntityReference _To(EntityReference r)
                            {
                                return r == null ? null : new PackedDynamicEntityReference { I = r.Id.ToString(), N = r.Name, L = r.LogicalName };
                            }
                            PackedDynamicEntityReference _To1(XrmActivityPointer r)
                            {
                                return r == null ? null : new PackedDynamicEntityReference
                                {
                                    I = r.Id.ToString(),
                                    N = r.GetAttributeValue<string>("subject"),
                                    L = r.ActivityType
                                };
                            }
                            var activities = pageNumber == 1
                                ? dataService
                                    .GetRepository<XrmActivityPointer>()
                                    .GetMyActivities(this.UserId, since)
                                    .OrderBy(x => x.StateCode)
                                    .OrderByDescending(x => x.ModifiedOn)
                                    .Take(500)
                                    .Select(x => _To1(x))
                                    .ToArray()
                                : new PackedDynamicEntityReference[] { };

                            var skip = 0;
                            var take = 20;
                            while (true)
                            {
                                var update = new MyUpdate
                                {
                                    PackedEntities = relatedEntities
                                                .Where(x => !activities.Any(y => y.I == x.Id.ToString()))
                                                .Select(x => _To(x)).Skip(skip).Take(take).ToArray(),
                                    Posts = posts.Select(x => x.ToDynamic()).Skip(skip).Take(take).ToArray(),
                                    PackedActivities = activities.Skip(skip).Take(take).ToArray()
                                };
                                if (update.Count() == 0)
                                {
                                    break;
                                }
                                await send(update);
                                skip += take;
                            }
                            fin = posts.Count() == 0;
                            pageNumber++;
                        }
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var update = new MyUpdate
                            {
                                LastSynchedOn = DateTime.UtcNow.Ticks
                            };
                            await send(update);
                        }
                    }
                }
                catch (Exception err)
                {
                    this.logger.LogError(
                        $"An error occured while trying to start 'MyWrkConsumer'. Err:{err.GetBaseException()}");
                }
                this.Busy = false;
            });

        }
    }
}
