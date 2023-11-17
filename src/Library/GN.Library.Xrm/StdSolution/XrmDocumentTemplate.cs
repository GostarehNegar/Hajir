using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmDocumentTemplate : XrmEntity<XrmDocumentTemplate>
	{

		public new class Schema
		{
			public const string LogicalName = "documenttemplate";
			public const string DocumentTemplateId = LogicalName + "id";
			public const string Status = "status";
			public const string Name = "name";
			public const string DocumentType = "documenttype";

		}
		public XrmDocumentTemplate() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.DocumentTemplateId)]
		public System.Nullable<System.Guid> DocumentTemplateId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.DocumentTemplateId);
			}
			set
			{
				this.SetAttributeValue(Schema.DocumentTemplateId, value);
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

		[AttributeLogicalName(Schema.DocumentTemplateId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.DocumentTemplateId = value;
			}
		}

		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get { return this.GetAttributeValue<string>(Schema.Name); }
			set { this.SetAttributeValue(Schema.Name, value); }
		}
		


	}
}
