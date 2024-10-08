﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames
{
    public class OnChageEventArgs
    {

    }

    public class XrmPageHelperEx
    {
        private readonly XrmFrameAdapter bus;
        public event EventHandler<OnChageEventArgs> OnChange;
        public XrmPageHelperEx(XrmFrameAdapter bus)
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
            var res = await this.bus.Evaluate<int>("2*2",15000);
            this.bus.Subscribe(m=> this.Handle(m));
        }
    }
}
