using GN.Library.Xrm.StdSolution;
using GN.Library.Xrm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Xrm.Services.Bus;

namespace Hajir.Crm.Infrastructure.Xrm.Sales.Handlers
{
    public class XrmEmailHanler :  XrmUpdateHandler<XrmAppointment>
    {
        public override Task Handle(XrmMessage message)
        {
            var ff = message.Entity.ToEntity<XrmAppointment>();
            var ggg = message.PreImage.ToEntity<XrmAppointment>();

            return Task.CompletedTask;
        }
    }
}
