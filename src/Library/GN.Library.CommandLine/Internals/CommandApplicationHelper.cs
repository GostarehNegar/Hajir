using GN.Library.CommandLines;
using GN.Library.Messaging;
using MassTransit;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.CommandLines.Internals
{
	public class CommandApplicationHelperEx : ICommandOnExecuteFactory
	{
//		private IClientFactoryEx factory;
		private IServiceProvider _serviceProvider;
		public CommandApplicationHelperEx()
        {

        }
		public CommandApplicationHelperEx(IServiceProvider service)
        {
			this._serviceProvider = service;
        }


		public Func<ICommandLineExecutionContext, CancellationToken, Task<int>> Create<T>() where T : class
		{
			Func<ICommandLineExecutionContext, CancellationToken, Task<int>> result = null;
			result = async (ctx, ct) =>
			{
				var _result = await Task.FromResult(-1);
				if (ctx.Properties.TryGetObjectValue<CommandLineApplication>(out var command) && command as CommandLineApplication<T> != null)
				{
					var _command = command as CommandLineApplication<T>;
					if (_command != null && true)
					{
						try
						{
							var _response = await AppHost.Bus.GetResponse<T, object>(_command.Model);
							if (_response != null)
							{
								try
								{
									_command.WriteLine(_response.GetType().GetProperty("Log").GetValue(_response).ToString());
								}
								catch { }
							}
							//_command.WriteLine(_response.Message.Log ?? "");

						}
						catch (Exception err)
						{
							_command.WriteLine($"Error: {err.GetBaseException().Message}");
						}
					}
				}
				return _result;
			};
			return result;
		}

		private CommandLineApplication Configure(CommandLineApplication application)
		{
			var modelType = (application as IModelAccessor)?.GetModelType();

			return application;
		}
		public CommandLineApplicationEx GetCommandLineApplication(bool refresh=true)
		{
			var result = CommandLineApplicationEx.GetInstance(refresh,this._serviceProvider,this)
				.UseServiceProvider(this._serviceProvider)
				.UseOnExecuteFactory(this);
			if (true)
			{
				//result.AddCommandLines(typeof().Assembly, Configure);
			}
			return result;
		}
		public async Task<string> Execute(string command, IServiceProvider provider)
		{
			//var helper = new CommandApplicationHelper();
			
			//helper.GetCommandLineApplication();
			this._serviceProvider = provider;
			//this.factory = provider.GetServiceEx<IClientFactoryEx>();
			await Task.CompletedTask;
			var app = this.GetCommandLineApplication(false);
			
			app.UseOnExecuteFactory(this);
			if (command!=null && command.ToLowerInvariant() == "help")
            {
				command = "-h";
            }
			var result = await app.ExecuteAsync(command);
			return result;
		}
	}
}
