using GN.Library.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.ServiceBus
{
	class DynamicsWebHookMiddelware : IMiddleware
	{
		private readonly XrmSettings options;
		private readonly ILogger logger;
		private string path;
		public DynamicsWebHookMiddelware(XrmSettings settings, ILogger<DynamicsWebHookMiddelware> logger)
		{
			this.options = settings;
			this.logger = logger;
			this.path = settings.WebHookPath;
		}
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			bool processed = false;
			if (!string.IsNullOrWhiteSpace(this.path) && context != null &&
				!string.IsNullOrWhiteSpace(context?.Request?.Path) &&
				context.Request.Path.Value.ToLowerInvariant().EndsWith(this.path.ToLowerInvariant()))
			{
				try
				{
					WebHookExecutionContext executionContext = null;
					using (var reader = new StreamReader(context.Request.Body))
					{
						var body = reader.ReadToEnd();
						executionContext = WebHookExecutionContext.Deseriallize(body);
						var target = executionContext.GetTarget();
					}
					var bus = AppHost.GetService<IMessageBus_Deprecated_2>();
					bus.Publish(new MessageContext(executionContext));
					context.Response.StatusCode = 200;
				}
				catch (Exception err)
				{
				}
				processed = true;
			}
			if (!processed)
				await next(context);
		}
	}
}
