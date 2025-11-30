using GN.Library.AI.Tools;
using GN.Library.Xrm;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Agents.Tools
{
    public abstract class HajirBaseTool : BaseTool
    {
        protected HajirBaseTool(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        protected IXrmDataServices DataServices => this.serviceProvider.GetService<IXrmDataServices>();
        protected IXrmRepository<T> GetRepository<T>() where T:XrmEntity=> this.DataServices.GetRepository<T>();
    }
}
