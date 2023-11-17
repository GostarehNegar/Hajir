using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmFeedback : XrmEntity<XrmFeedback, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "feedback";
            public const string FeadbackId = LogicalName + "id";
			public const string Title = "title";
        }
        public XrmFeedback() : base(Schema.LogicalName)
        {

        }
		[AttributeLogicalName(Schema.FeadbackId)]
		public System.Nullable<System.Guid> FeedbackId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.FeadbackId);
			}
			set
			{
				this.SetAttributeValue(Schema.FeadbackId, value);
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



		[AttributeLogicalName(Schema.FeadbackId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.FeedbackId = value;
			}
		}

		[AttributeLogicalName(Schema.Title)]
		public string Title
		{
			get
			{
				return this.GetAttributeValue<string>(Schema.Title);
			}
			set
			{
				this.SetAttributeValue(Schema.Title, value);
			}
		}

	}
}
