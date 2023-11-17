using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.Services.EntityWatcher
{
    public class XrmEntityWatcherOptions
    {
        public XrmEntityWatcherOptions()
        {
            this.Delay = 5 * 1000;
        }
        public int Delay { get; set; }

        internal int GetDelay()
        {
            this.Delay = this.Delay > 5000 ? this.Delay : 5 * 1000;
            return this.Delay;
        }


    }
}
