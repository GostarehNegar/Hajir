using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public class OnChageEventArgs
    {

    }

    public class XrmPageHelper
    {
        private readonly XrmFrameAdapter bus;
        public event EventHandler<OnChageEventArgs> OnChange;
        public XrmPageHelper(XrmFrameAdapter bus)
        {
            this.bus = bus;
        }
        private async Task Handle(XrmFrameMessage message)
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
