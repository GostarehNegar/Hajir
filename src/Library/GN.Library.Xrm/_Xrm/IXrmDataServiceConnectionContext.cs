using GN.Library.Xrm.StdSolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.Xrm
{
	public interface IXrmDataServiceConnectionContext
	{
		Guid? UserId { get; }
		DateTime ToUtcTime(DateTime dateTime);
		DateTime FromUtcTime(DateTime dateTime);

	}
	class XrmDataServiceConnectionContext : IXrmDataServiceConnectionContext
	{
		private XrmSystemUser user;
		private XrmUserSettings userSettings;
		private TimeZoneInfo timezoneinfo;
		public XrmDataServiceConnectionContext(XrmSystemUser user, XrmUserSettings settings)
		{
			this.user = user;
			this.userSettings = settings;
		}
		public XrmDataServiceConnectionContext(IXrmDataServices dataService)
		{
			try
			{
				var userId = dataService.GetXrmOrganizationService(true).GetServiceUserGuid();
				this.user = dataService.GetRepository<XrmSystemUser>().Queryable.FirstOrDefault(x => x.SystemUserId == userId);
				this.userSettings = dataService.GetRepository<XrmUserSettings>().Queryable.FirstOrDefault(x => x.SystemUserId == userId);
			}
			catch { }
		}

		public Guid? UserId => this.user?.Id;

		private TimeZoneInfo GetTimeZoneInfo()
		{
			if (this.timezoneinfo==null)
			{
				if (this.userSettings != null && this.userSettings.TimezoneCode.HasValue)
				{
					this.timezoneinfo = Helpers.CrmTimeZones.GetTymeZoneByCode(this.userSettings.TimezoneCode.Value)?.TimeZoneInfo;
				}
				if (this.timezoneinfo==null)
				{
					this.timezoneinfo = TimeZoneInfo.Local;
				}
			}
			return this.timezoneinfo;
		}
		public DateTime ToUtcTime(DateTime dateTime)
		{
			return TimeZoneInfo.ConvertTimeToUtc(new DateTime(dateTime.Ticks, DateTimeKind.Unspecified), GetTimeZoneInfo());
		}

		public DateTime FromUtcTime(DateTime dateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(dateTime.Ticks, DateTimeKind.Unspecified), GetTimeZoneInfo());
		}
	}
}
