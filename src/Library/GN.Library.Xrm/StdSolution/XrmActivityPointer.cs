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
    /// 
    /// https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/activitypointer?view=op-9-1#BKMK_ActivityId
    /// 
    /// </summary>
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmActivityPointer : XrmEntity<XrmActivityPointer>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "activitypointer";
            public const string ActivityId = "activityid";
            public const string ActivityTypeCode = "activitytypecode";

        }
        public XrmActivityPointer() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.ActivityId)]
        public Guid? ActivityId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.ActivityId);
            }
            set
            {
                this.SetAttributeValue(Schema.ActivityId, value);
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

        [AttributeLogicalName(Schema.ActivityId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.ActivityId = value;
            }
        }
        //[AttributeLogicalName(Schema.ActivityTypeCode)]
        public string ActivityType
        {
            get
            {
                switch (this.GetAttributeValue<object>(Schema.ActivityTypeCode))
                {
                    case string _s:
                        return _s;
                    case int _i:
                        return Entities.GetNameByCode(_i);

                }
                return null;
            }

            //set => this.SetAttribiuteValue(Schema.ActivityTypeCode, value);

        }
        [AttributeLogicalName(Schema.ActivityTypeCode)]
        public int ActivityTypeCode
        {
            get => this.GetAttributeValue<int>(Schema.ActivityTypeCode);
            set => this.SetAttribiuteValue(Schema.ActivityTypeCode, value);

        }

    }

    public static class ActivityPointerExtensions
    {
        public static IEnumerable<T> GetMyActivities<T>(this IXrmRepository<T> repo, Guid userId, DateTime modifiedAfter, int skip = 0, int take = 1000) where T : XrmActivityPointer
        {
            return repo
                .Queryable
                .Where(x => x.Owner.Id == userId && x.ModifiedOn >= modifiedAfter)
                .Skip(skip)
                .Take(take)
                .ToArray();

        }
    }
}
