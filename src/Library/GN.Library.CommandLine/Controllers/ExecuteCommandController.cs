using GN.Library.CommandLines.Internals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.CommandLines.Controllers
{
	[Route("api/[controller]")]
	public class ExecuteCommandController : ControllerBase
	{
		private readonly IServiceProvider serviceProvider;

		public ExecuteCommandController(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		[HttpGet]
		public async Task<string> Execute(string command)
		{
			await Task.CompletedTask;
			var result = "";
			var helper = new CommandApplicationHelperEx();
			serviceProvider.GetServiceEx<CommandLineOptions>()?.Configure?.Invoke(helper.GetCommandLineApplication());
			try
			{
				result = await helper.Execute(command, this.serviceProvider);
			}
			catch (Exception err)
			{
				result ="Error:"+ err.GetBaseException().Message;
			}


			return result;

		}
	}
}
