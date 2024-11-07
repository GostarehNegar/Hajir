using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    internal static partial class IntegrationServiceExtentions
    {
        public static async Task<IntegrationQuote> ImportQuote(this IntegrationServiceContext context, IntegrationQuote quote)
        {
            await Task.Delay(100);
            IntegrationQuote result = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(quote.AccountId) && context.Store.GetAccountByExternalId(quote.AccountId)==null)
                {
                    var account = await context.ImportAccountById(quote.AccountId);
                }
                result = context.Store.ImportLegacyQuote(quote);
                //context.LegacyCrmStore.UpdateQuoteImportStatus(quote);
                context.Logger.LogInformation($"Quote {quote} successfully imported.");

            }
            catch (Exception err)
            {
                context.Logger.LogError(
                    $"An error occured while trying to import quote:{quote}. Err:{err.Message}");
                throw;
            }
            return result;
        }
    }
}
