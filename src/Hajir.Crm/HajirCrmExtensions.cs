using Hajir.Crm.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm
{
	public static partial class HajirCrmExtensions
	{
		public static IHajirCrmServiceContext CreateContext(this IServiceProvider serviceProvider)
		{
			return new HajirCrmServiceContext(serviceProvider);
		}
	}
}
