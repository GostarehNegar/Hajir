using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GN.Library.Xrm.StdSolution
{
	class StdSolutionModule : IAppModule
	{
		public void AddServices(IServiceCollection services)
		{
			services.AddStdSolution();
		}

		public void UseServices(IApplicationBuilder app, IHostingEnvironment env)
		{
			
		}
	}
}
