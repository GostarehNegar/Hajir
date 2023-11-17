using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmPostComment : XrmEntity<XrmPostComment, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "postcomment";
            public const string PostCommentId = LogicalName + "id";
        }
		public XrmPostComment() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalName(Schema.PostCommentId)]
		public System.Nullable<System.Guid> PostCommentId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.PostCommentId);
			}
			set
			{
				this.SetAttributeValue(Schema.PostCommentId, value);
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
		[AttributeLogicalName(Schema.PostCommentId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.PostCommentId = value;
			}
		}
	}
}
