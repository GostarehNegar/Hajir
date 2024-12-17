using GN.Library.Xrm;
using GN.Library.Xrm.Services.Bus;
using GN.Library.Xrm.StdSolution;
using System.Threading.Tasks;

namespace Hajir.Crm.Infrastructure.Xrm.Sales.Handlers
{
    public class XrmContactHandler : XrmUpdateHandler<XrmContact>
    {
        public override Task Handle(XrmMessage message)
        {
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }
    }
}
