using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.CommandLines.Internals
{
	class CommandLineExecutionContext : ICommandLineExecutionContext
	{
		private ConcurrentDictionary<string, object> _properties = new ConcurrentDictionary<string, object>();
		public IDictionary<string, object> Properties => _properties;
		public CommandLineExecutionContext()
		{

		}
		public CommandLineExecutionContext(CommandLineApplication command)
		{
			this.Properties.AddOrUpdateObjectValue<CommandLineApplication>(() => command);

		}


	}
}
