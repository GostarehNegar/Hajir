using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment.Components.Base
{
    public class ServiceManagementComponentBase : ComponentBase, IDisposable
    {
        [Inject]
        public IServiceManagementBlazorServices Services { get; set; }

        protected List<IDisposable> Disposables = new List<IDisposable>();
        protected void AddDisposable(IDisposable disposable) => this.Disposables.Add(disposable);

        public void Dispose()
        {
            (Disposables ?? new List<IDisposable>())
                .ForEach(x => x.Dispose());
        }
    }
}
