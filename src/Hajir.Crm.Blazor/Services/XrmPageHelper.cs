using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Services
{
    public class OnChageEventArgs
    {

    }

    public class XrmPageHelper
    {
        private readonly WebResourceBus bus;
        public event EventHandler<OnChageEventArgs> OnChange;
        public XrmPageHelper(WebResourceBus bus)
        {
            this.bus = bus;
        }
        private async Task Handle(WebResourceMessage message)
        {
            await Task.CompletedTask;
            if (message?.Subject == "onchange")
            {

                this.OnChange?.Invoke(this,new OnChageEventArgs { });
            }

        }
        public async Task Initialize()
        {
            var res = await this.bus.Evaluate("2*2");
            this.bus.Subscribe(m=> this.Handle(m));
        }
    }
}
