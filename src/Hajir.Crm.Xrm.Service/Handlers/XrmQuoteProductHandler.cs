using GN.Library.Xrm.StdSolution;
using GN.Library.Xrm;
using System.Threading.Tasks;
using GN.Library.Xrm.Services.Bus;

namespace Hajir.Crm.Xrm.Service.Handlers
{
    public class XrmQuoteProductHandler : XrmValidationHandler<XrmQuoteDetail>
    {
        public XrmQuoteProductHandler()
        {

        }
        public override async Task Handle(XrmMessage message)
        {
            await Task.CompletedTask;
            var line = message.Entity.ToEntity<XrmQuoteDetail>();
            

            
        }
    }
}
