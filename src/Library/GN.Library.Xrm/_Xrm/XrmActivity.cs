using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm
{
    public class XrmActivity<TEntity, TStates, TStatus> : XrmEntity<TEntity, TStates, TStatus>
            where TEntity : XrmEntity, new()
            where TStates : struct
            where TStatus : struct
    {
        public new class Schema : XrmEntity.Schema
        {
            //public const string SolutionPerfix = "gndyn_";
            //public const string LogicalName = "activitypointer";
            public const string ActivityId = "activityid";
            public const string Subject = "subject";

            public const string ActivityTypeCode = "activitytypecode";
            public const string ActualDurationMinutes = "actualdurationminutes";
            public const string ActualEnd = "actualend";
            public const string ActualStart = "actualstart";
            public const string Description = "description";
            public const string ScheduleEnd = "scheduledend";
            public const string ScheduleStart = "scheduledstart";
            public const string PriorityCode = "prioritycode";
            

        }
        public XrmActivity(string logicalName) : base(logicalName) { }

        #region Guid Property

        [AttributeLogicalNameAttribute(Schema.ActivityId)]
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

        [AttributeLogicalNameAttribute(Schema.ActivityId)]
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

        #endregion //Guid Property

        #region String Property

        [AttributeLogicalNameAttribute(Schema.Subject)]
        public string Subject
        {
            get { return this.GetAttributeValue<string>(Schema.Subject); }
            set { this.SetAttributeValue(Schema.Subject, value); }
        }

        [AttributeLogicalNameAttribute(Schema.Description)]
        public string Description
        {
            get { return this.GetAttributeValue<string>(Schema.Description); }
            set { this.SetAttributeValue(Schema.Description, value); }
        }

        #endregion String Property

        #region Int Property

        [AttributeLogicalNameAttribute(Schema.ActivityTypeCode)]
        public Nullable<int> ActivityTypeCode
        {
            get
            {
                var result = this.GetAttributeValue<OptionSetValue>(Schema.ActivityTypeCode);
                return result == null ? 0 : result.Value;
            }
            set
            {
                this.SetAttributeValue(Schema.ActivityTypeCode,
                    value == null ? null : new OptionSetValue(value.Value));
            }
        }

        [AttributeLogicalNameAttribute(Schema.ActualDurationMinutes)]
        public Nullable<int> ActualDurationMinutes
        {
            get
            {
                return this.GetAttributeValue<int?>(Schema.ActualDurationMinutes);
            }
            set
            {
                this.SetAttributeValue(Schema.ActualDurationMinutes, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.PriorityCode)]
        public Nullable<int> PriorityCode
        {
            get
            {
                var result = this.GetAttributeValue<OptionSetValue>(Schema.PriorityCode);
                return result == null ? 0 : result.Value;
            }
            set
            {
                this.SetAttributeValue(Schema.PriorityCode,
                    value == null ? null : new OptionSetValue(value.Value));
            }
        }
        #endregion //Int Property

        #region DateTimeProperty

        [AttributeLogicalNameAttribute(Schema.ActualStart)]
        public DateTime? ActualStart
        {
            get
            {
                return this.GetAttributeValue<Nullable<DateTime>>(Schema.ActualStart);
            }
            set
            {
                this.SetAttributeValue(Schema.ActualStart, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.ActualEnd)]
        public DateTime? ActualEnd
        {
            get
            {
                return this.GetAttributeValue<Nullable<DateTime>>(Schema.ActualEnd);
            }
            set
            {
                this.SetAttributeValue(Schema.ActualEnd, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.ScheduleStart)]
        public DateTime? ScheduleStart
        {
            get
            {
                return this.GetAttributeValue<Nullable<DateTime>>(Schema.ScheduleStart);
            }
            set
            {
                this.SetAttributeValue(Schema.ScheduleStart, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.ScheduleEnd)]
        public DateTime? ScheduleEnd
        {
            get
            {
                return this.GetAttributeValue<Nullable<DateTime>>(Schema.ScheduleEnd);
            }
            set
            {
                this.SetAttributeValue(Schema.ScheduleEnd, value);
            }
        }

        #endregion //DateTimeProperty
    

    }


}
