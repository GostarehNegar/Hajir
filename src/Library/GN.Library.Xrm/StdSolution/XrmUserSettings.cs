using Microsoft.AspNetCore.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmUserSettings : XrmEntity<XrmUserSettings>
	{
		public new class Schema
		{
			public const string LogicalName = "usersettings";
			public const string UserSettingsId = LogicalName + "id";
			public const string SystemUserId = "systemuserid";
			public const string LocaleId = "localeid";
			public const string TimezoneCode = "timezonecode";
		}

		public XrmUserSettings() : base(Schema.LogicalName)
		{

		}
		[AttributeLogicalName(Schema.UserSettingsId)]
		public System.Nullable<System.Guid> UserSettingsId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.UserSettingsId);
			}
			set
			{
				this.SetAttributeValue(Schema.UserSettingsId, value);
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

		[AttributeLogicalName(Schema.UserSettingsId)]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				this.UserSettingsId = value;
			}
		}

		[AttributeLogicalName(Schema.SystemUserId)]
		public Guid? SystemUserId

		{
			get { return this.GetAttributeValue<Guid?>(Schema.SystemUserId); }
			set { this.SetAttribiuteValue(Schema.SystemUserId, value); }
		}

		[AttributeLogicalName(Schema.LocaleId)]
		public int? LocaleId

		{
			get { return this.GetAttributeValue<int?>(Schema.LocaleId); }
			set { this.SetAttribiuteValue(Schema.LocaleId, value); }
		}
		[AttributeLogicalName(Schema.TimezoneCode)]
		public int? TimezoneCode

		{
			get { return this.GetAttributeValue<int?>(Schema.TimezoneCode); }
			set { this.SetAttribiuteValue(Schema.TimezoneCode, value); }
		}

	}
}
