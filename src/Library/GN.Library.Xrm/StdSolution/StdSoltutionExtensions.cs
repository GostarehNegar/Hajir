using GN.Library.Xrm.StdSolution.Accounts;
using GN.Library.Xrm.StdSolution.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GN.Library.Xrm.StdSolution
{
	public static partial class StdSoltutionExtensions
	{
		private static ILogger logger = typeof(StdSoltutionExtensions).GetLoggerEx();
		private static T GetService<T>()
		{
			return AppHost.GetService<T>();
		}
		public static IXrmDataServices DataContext => GetService<IXrmDataServices>();
		
		
		public static void AddStdSolution(this IServiceCollection services)
		{
			if (!services.HasService<IXrmAccountRespository>())
			{
				services.AddTransient<IXrmAccountRespository, XrmAccountRepository>();
				services.AddTransient<IXrmRepository<XrmAccount>, XrmAccountRepository>();
				services.AddTransient<XrmAccountServices>();
				//services.AddTransient<IXrmPluginServices, XrmPluginServices>();
			}
		}
		public static IXrmRepository<XrmAccount> GetAccounts(this IAppDataServices This)
		{
			return This.AppContext.AppServices.GetService<IXrmRepository<XrmAccount>>();
		}
		


		public static void RunOnBehalfOf(this XrmSystemUser This, Action action)
		{
			using (var ctx = AppHost.Context.Push())
			{
				ctx.GetCurrentUser().Id = This.Id;
				action();
			}
		}
		public static T RunOnBehalfOf<T>(this XrmSystemUser This, Func<T> action)
		{
			using (var ctx = AppHost.Context.Push())
			{
				ctx.GetCurrentUser().Id = This.Id;
				return action();
			}
		}

	}
}
