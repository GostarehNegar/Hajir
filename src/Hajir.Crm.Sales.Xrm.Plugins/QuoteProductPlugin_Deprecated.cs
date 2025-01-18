using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Sales.Xrm.Plugins
{
    public class QuoteProductPlugin_Deprecated : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            try
            {
                Entity entity = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] as Entity : null;
                Entity pre_image = context.PreEntityImages.Contains("Target") ? context.PreEntityImages["Target"] as Entity : null;
                Entity post_image = context.PostEntityImages.Contains("Target") ? context.PostEntityImages["Target"] as Entity : null;
                var percentTax = entity.GetAttributeValue<int?>("hajir_" + "percenttax");
                if (percentTax.HasValue)
                {
                    entity["tax"] = new Money(percentTax.Value);
                    //message.Change(XrmQuoteDetail.Schema.Tax, new Money(tax));
                }


            }
            catch (Exception err)
            {
                throw;
            }
        }
    }
}
