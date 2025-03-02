using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmUnitOfMeasure : XrmEntity<XrmUnitOfMeasure, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema
		{
			public const string LogicalName = "uom";
			public const string UnitOfMeasureId = "uom" + "id";
			public const string Name = "name";
			public const string ScheduleId = "uomscheduleid";

		}
		public XrmUnitOfMeasure() : base(Schema.LogicalName) { }

		[AttributeLogicalNameAttribute(Schema.UnitOfMeasureId)]
		public System.Nullable<System.Guid> UnitOfMeasureId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.UnitOfMeasureId);
			}
			set
			{
				this.SetAttributeValue(Schema.UnitOfMeasureId, value);
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

		[AttributeLogicalNameAttribute(Schema.UnitOfMeasureId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.UnitOfMeasureId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttribiuteValue(Schema.Name, value); }
		}
		[AttributeLogicalName(Schema.ScheduleId)]
		public EntityReference ScheduleIdRef
		{
			get { return this.GetAttributeValue<EntityReference>(Schema.ScheduleId); }
			set { this.SetAttribiuteValue(Schema.ScheduleId, value); }
		}
		[AttributeLogicalName(Schema.ScheduleId)]
		public Guid? ScheduleId
		{
			get => this.ScheduleIdRef?.Id;
			set => this.ScheduleIdRef = value.HasValue ? new EntityReference("uomschedule", value.Value) : null;
		}

	}
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmUnitOfMeasurementGroup : XrmEntity<XrmUnitOfMeasure, DefaultStateCodes, DefaultStatusCodes>
	{
        public new class Schema
        {
            public const string LogicalName = "uomschedule";
            public const string UnitScheduleId =LogicalName + "id";
            public const string Name = "name";
            

        }
		public XrmUnitOfMeasurementGroup() : base(Schema.LogicalName) { }
        [AttributeLogicalNameAttribute(Schema.UnitScheduleId)]
        public System.Nullable<System.Guid> UnitOfMeasureScheduleId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.UnitScheduleId);
            }
            set
            {
                this.SetAttributeValue(Schema.UnitScheduleId, value);
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

        [AttributeLogicalNameAttribute(Schema.UnitScheduleId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.UnitOfMeasureScheduleId = value;
            }
        }

    }
}
