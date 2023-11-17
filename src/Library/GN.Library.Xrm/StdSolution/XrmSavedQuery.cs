using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	/// <summary>
	/// 
	/// https://learn.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/savedquery?view=op-9-1
	/// </summary>
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmSavedQuery : XrmEntity<XrmSavedQuery>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "savedquery";
			public const string SavedQueryId = LogicalName + "id";
			public const string Fetchxml = "fetchxml";
			public const string Name = "name";
        }
        public XrmSavedQuery() : base(Schema.LogicalName) { }

		[AttributeLogicalName(Schema.SavedQueryId)]
		public Guid? SavedQueryId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.SavedQueryId);
			}
			set
			{
				this.SetAttributeValue(Schema.SavedQueryId, value);
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
		[AttributeLogicalName(Schema.SavedQueryId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.SavedQueryId = value;
			}
		}

		[AttributeLogicalName(Schema.Fetchxml)]
		public string FetchXml
        {
			get => this.GetAttributeValue<string>(Schema.Fetchxml);
			set => this.SetAttribiuteValue(Schema.Fetchxml, value);
        }
		[AttributeLogicalName(Schema.Name)]
		public string Name
		{
			get => this.GetAttributeValue<string>(Schema.Name);
			set => this.SetAttribiuteValue(Schema.Name, value);
		}

	}
}
