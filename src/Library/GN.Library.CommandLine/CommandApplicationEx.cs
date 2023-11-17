using GN.Library.CommandLines.Internals;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.CommandLines
{
	public class CommandLineApplicationEx : CommandLineApplication, IServiceProvider
	{
		private static CommandLineApplicationEx _instance;
		private ICommandOnExecuteFactory _onExecuteFactory;
		private IServiceProvider _provider;
		public TextWriter Writer { get; private set; }
		public static CommandLineApplicationEx CreateInstance(IServiceProvider serviceProvider = null, ICommandOnExecuteFactory factory = null)
		{
			var result = new CommandLineApplicationEx(serviceProvider, factory);
			return result;
		}
		public static CommandLineApplicationEx GetInstance(bool reset = false, IServiceProvider serviceProvider = null, ICommandOnExecuteFactory factory = null)
		{
			if (_instance == null || reset)
			{
				_instance = CreateInstance(serviceProvider, factory);
			}
			return _instance;
		}
		public CommandLineApplicationEx(IServiceProvider provider = null, ICommandOnExecuteFactory factory = null)
		{
			this.Name = "";
			this.Writer = new StringWriter();
			this.OnExecuteAsync( c =>
			{
				return Task.FromResult(0);
			});
			this.Out = this.Writer;
			this.Error = new StringWriter();
			this._onExecuteFactory = factory;
			this._provider = provider ?? AppHost.Services;
			this.Conventions.UseDefaultConventions().UseDefaultHelpOption();
			//this.Command("help",cfg=> { });

			//this.Execute("exec");
		}
		public CommandLineApplicationEx UseServiceProvider(IServiceProvider provider)
		{
			_provider = provider ?? _provider;
			return this;
		}
		public CommandLineApplicationEx UseOnExecuteFactory(ICommandOnExecuteFactory factory)
		{
			this._onExecuteFactory = factory ?? this._onExecuteFactory;
			return this;
		}

		public async Task<string> ExecuteAsync(string args)
		{
			//this.Writer = new StringWriter();
			//this.Out = this.Writer;
			await this.ExecuteAsync(args.SplitCommandLine().ToArray());
			return this.Writer.ToString();
		}
		public string Execute(string args)
		{
			var result = this.ExecuteAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
			return result;
		}
		public CommandLineApplicationEx AddCommand(Action<CommandLineApplication> configure = null)
		{
			var result = new CommandLineApplication();
			result.Parent = this;
			result.Conventions.UseDefaultConventions().UseDefaultHelpOption().UseConstructorInjection();
			configure?.Invoke(result);
			this.AddSubcommand(result);
			return this;
		}
		public CommandLineApplicationEx AddCommandLines(Assembly assembly, Func<CommandLineApplication, CommandLineApplication> configure = null)
		{
			Type[] types;
			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException err)
			{
				types = err.Types;
			}
			var commandTypes = types.Where(x => x.GetCustomAttribute<CommandAttribute>() != null).ToList();

			var func = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.Name == "AddCommand").ToArray()
				.Where(x => x.IsGenericMethod)
				.FirstOrDefault();
			if (func == null)
				throw new Exception("AddCommand method not found!");
			int GetOrder(CommandAttribute attrib)
			{
				return attrib == null || string.IsNullOrEmpty(attrib.Name)
					? 0
					: attrib.Name.Count(c => c == '.');
			}
			foreach (var type in types
				.Where(x => x.GetCustomAttribute<CommandAttribute>() != null)
				.OrderBy(x => GetOrder(x.GetCustomAttribute<CommandAttribute>()))
				.ToList())
			{
				var attrib = type.GetCustomAttribute<CommandAttribute>();
				CommandLineApplication command = null;
				Action<CommandLineApplication> Config = cmd =>
				{
					command = cmd;
				};

				func?.MakeGenericMethod(type)
					?.Invoke(this, new object[] { Config, false });
				command = configure == null ? command : configure(command);
				CommandLineApplication parent = this;
				if (command.Name.Contains("."))
				{
					var parts = command.Name.Split('.');
					command.Name = parts[parts.Length - 1];
					for (var level = 0; level < parts.Length - 1; level++)
					{
						var _p = parent.Search(parts[level]);
						if (_p == null)
						{
							_p = new CommandLineApplication();
							_p.Conventions.UseDefaultConventions().UseDefaultHelpOption();
							_p.Name = parts[level];
							_p.OnExecuteAsync(ct =>
							{
								return Task.FromResult(0);
							});
							_p.Conventions.UseDefaultConventions().UseDefaultHelpOption();
							_p.Parent = parent;
							_p.Out = parent.Out;
							parent.AddSubcommand(_p);
						}
						if (_p != null)
							parent = _p;

					}
				}
				if (command != null)
					parent.AddSubcommand(command);

			}


			return this;
		}
		public CommandLineApplication AddCommand<T>(Action<CommandLineApplication<T>> configure = null, bool add = true) where T : class
		{
			var result = new CommandLineApplication<T>();
			result.Parent = this;
			result.Conventions.UseDefaultConventions().UseDefaultHelpOption().UseConstructorInjection();
			result.Out = this.Out;
			var onExecute = GetOnExcuteFactory()?.Create<T>();
			if (onExecute != null)
			{
				result.OnExecuteAsync(ct =>
				{
					var ctx = new CommandLineExecutionContext(result);
					return onExecute(ctx, ct);
				});
			}
			configure?.Invoke(result);
			if (add)
				this.AddSubcommand(result);
			return this;
		}
		object IServiceProvider.GetService(Type type)
		{

			return null;
		}

		public IServiceProvider GetServiceProvider()
		{
			return this._provider ?? this;
		}

		public ICommandOnExecuteFactory GetOnExcuteFactory()
		{
			return this._onExecuteFactory ?? GetServiceProvider().GetServiceEx<ICommandOnExecuteFactory>();

		}

	}
}
