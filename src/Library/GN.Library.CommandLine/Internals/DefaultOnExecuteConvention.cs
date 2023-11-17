using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.CommandLines.Internals
{
	class DefaultOnExecuteConvention<T> : IConvention where T : class
	{
		public void Apply(ConventionContext context)
		{
			var execute = context.Application.GetRootApplication()?.GetOnExcuteFactory()?.Create<T>();
			if (execute != null)
			{
				context.Application.OnExecuteAsync((ct) =>
				{
					var _context = new CommandLineExecutionContext(context.Application);
					return execute(_context, ct);
				});
			}
		}
	}
}
