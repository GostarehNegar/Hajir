using MassTransit;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.CommandLines.Internals;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using McMaster.Extensions.CommandLineUtils.Conventions;

namespace GN.Library.CommandLines
{
	[Command(name: "SomeCommand")]
	public class DoCommand
	{
		[Option]
		public string Args { get; set; }
	}
	public static class Utils
	{
		private static CommandLineApplicationEx application;
		internal static string TrimMatchingQuotes(this string input, char quote)
		{
			if ((input.Length >= 2) &&
				(input[0] == quote) && (input[input.Length - 1] == quote))
				return input.Substring(1, input.Length - 2);

			return input;
		}
		internal static IEnumerable<string> SplitCommandLine(this string commandLine)
		{
			bool inQuotes = false;

			return commandLine.Split(c =>
			{
				if (c == '\"')
					inQuotes = !inQuotes;

				return !inQuotes && c == ' ';
			})
							  .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
							  .Where(arg => !string.IsNullOrEmpty(arg));
		}
		internal static IEnumerable<string> Split(this string str,
											Func<char, bool> controller)
		{
			int nextPiece = 0;

			for (int c = 0; c < str.Length; c++)
			{
				if (controller(str[c]))
				{
					yield return str.Substring(nextPiece, c - nextPiece);
					nextPiece = c + 1;
				}
			}

			yield return str.Substring(nextPiece);
		}
		
		public static CommandLineApplicationEx GetRootApplication(this CommandLineApplication command)
		{
			if (command.Parent != null)
				return command.Parent.GetRootApplication();
			if (command as CommandLineApplicationEx != null)
				return command as CommandLineApplicationEx;
			return CommandLineApplicationEx.GetInstance();

		}
		public static TextWriter GetWriter(this CommandLineApplication app)
		{
			return app.GetRootApplication().Writer;
		}
		//public static async Task<int> _Execute<T>(T model, CancellationToken cancellation) where T : class
		//{
		//	var result = 0;
		//	var client = AppHost.GetService<IRequestClient<T>>();
		//	var response = await client.GetResponse<CommandLineResponse>(model, cancellation);
		//	return result;

		//}

		public static void WriteLine(this CommandLineApplication cmd, string fmt, params object[] args)
		{
			cmd?.GetRootApplication().Out.WriteLine(fmt??"", args);
		}

		public static CommandLineApplication Search(this CommandLineApplication current, string name)
		{
			CommandLineApplication result = null;
			if (current.Name == name)
				return current;
			foreach(var command in current.Commands)
			{
				result = command.Search(name);
				if (result != null)
					return result;
			}
			return null;
		}

	}
}
