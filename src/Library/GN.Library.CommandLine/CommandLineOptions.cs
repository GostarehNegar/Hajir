using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.CommandLines
{
	public class CommandLineOptions
	{
		public Action<CommandLineApplicationEx> Configure { get; set; }
	}
}
