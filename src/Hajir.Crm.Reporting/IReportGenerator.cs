using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Reporting
{
    public interface IReportGenerator : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
    class ReportGenerator : IReportGenerator
    {
        private IServiceScope serviceScope;
        public IServiceProvider ServiceProvider => this.serviceScope.ServiceProvider;

        public ReportGenerator(IServiceProvider serviceProvider)
        {
            this.serviceScope = serviceProvider.CreateScope();

        }

        public void Dispose()
        {
            this.serviceScope?.Dispose();
            
        }
    }
}
