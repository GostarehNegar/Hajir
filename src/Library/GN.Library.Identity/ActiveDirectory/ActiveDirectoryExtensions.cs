using GN.Library.Shared.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace GN.Library.Identity.ActiveDirectory
{
	public static partial class ActiveDirectoryExtensions
	{
		public static IServiceCollection AddActiveDirectory(this IServiceCollection services, IConfiguration configuration, Action<ActiveDirectoryOptions> configure)
		{
			var options = new ActiveDirectoryOptions();
			configuration.GetSection("identity").Bind("activedirectory", options);
			services.AddSingleton(options);
			services.AddMemoryCache();


			services.AddSingleton<IAuthenticationProvider, ActiveDirectoryProvider>();
			services.AddSingleton<IUserIdentityProvider, ActiveDirectoryProvider>();
			return services;
		}

		public static UserIdentityEntity ToIdentityUser(this ActiveDirectoryIdentity acUser)
		{
			var result = new UserIdentityEntity
			{
				UserName = acUser.LogonName?.ToLowerInvariant(),
				DisplayName = acUser.DisplayName,
				IpPhoneExtension = acUser.Extension,
				Email = acUser.Mail,
				IsDisabled = acUser.IsDisabled,
				Title = acUser.Title,
				IsAdmin = acUser.IsAdmin,
				DomaiName = acUser.DomainName,
				AccountName = acUser.AccountName

			};
			result.AddGroupNames(acUser.GetGroupNames());
			foreach (var item in acUser.Attributes)
			{
				result.SetAttributeValue("ac_" + item.Key, item.Value);
			}

			return result;
		}
		public static bool ActiveDirectoryAuthenticate(string username, string password)
		{
			bool result = false;
			using (DirectoryEntry _entry = new DirectoryEntry())
			{
				_entry.Username = username;
				_entry.Password = password;
				DirectorySearcher _searcher = new DirectorySearcher(_entry);
				_searcher.Filter = "(objectclass=user)";
				try
				{
					SearchResult _sr = _searcher.FindOne();
					string _name = _sr.Properties["displayname"][0].ToString();
					result = true;
				}
				catch
				{ /* Error handling omitted to keep code short: remember to handle exceptions !*/ }
			}

			return result; //true = user authenticated!
		}
	}
	
}
