﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationBackgroundService : BackgroundService
    {
        private readonly ILogger<IntegrationBackgroundService> logger;
        private readonly IServiceProvider serviceProvider;

        public IntegrationBackgroundService(ILogger<IntegrationBackgroundService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            return Task.Run(async () =>
            {
                await Task.CompletedTask;
            });

        }

    }
}
