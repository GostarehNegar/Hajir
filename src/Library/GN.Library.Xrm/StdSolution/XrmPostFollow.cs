using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Xrm.StdSolution
{
    /// <summary>
    /// 
    /// https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/postfollow?view=op-9-1
    /// </summary>
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmPostFollow : XrmEntity<XrmPostFollow, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "postfollow";
            public const string PostFollowId = LogicalName + "id";
            public const string RegardingObjectId = "regardingobjectid";
        }
        public XrmPostFollow() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.PostFollowId)]
        public System.Nullable<System.Guid> PostFollowId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PostFollowId);
            }
            set
            {
                this.SetAttributeValue(Schema.PostFollowId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }
        [AttributeLogicalName(Schema.PostFollowId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.PostFollowId = value;
            }
        }

        [AttributeLogicalName(Schema.RegardingObjectId)]
        public EntityReference RegardingObjectId
        {
            get => this.GetAttributeValue<EntityReference>(Schema.RegardingObjectId);
            set => this.SetAttribiuteValue(Schema.RegardingObjectId, value);
        }

    }


    public static class XrmPostFollowExtensions
    {
        public static IEnumerable<EntityReference> GetFollowingEntities<T>(this IXrmRepository<T> repo, Guid userId, int skip = 0, int take = 1000) where T : XrmPostFollow
        {
            return repo.Queryable
                .Where(x => x.Owner.Id == userId)
                .Select(x => x.RegardingObjectId)
                .Skip(skip)
                .Take(take)
                .ToArray();
        }
        public static IEnumerable<T> GetFollowers<T>(this IXrmRepository<T> repo, EntityReference post, int skip =0, int take =1000) where T : XrmPostFollow
        {
            return repo.Queryable
                .Where(x => x.RegardingObjectId.Id == post.Id)
                .Skip(skip)
                .Take(take)
                .ToArray();
                

        }
    }
}
