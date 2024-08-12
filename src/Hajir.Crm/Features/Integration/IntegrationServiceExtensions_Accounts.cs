using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using GN.Library.Shared.Entities;

namespace Hajir.Crm.Features.Integration
{
    internal static partial class IntegrationServiceExtentions
    {
        public static async Task<IntegrationAccount> ImportAccountById(this IntegrationServiceContext context, string accountId, bool shallow = false)
        {

            return await context.ImportAccount(context.LegacyCrmStore.GetAccount(accountId));


        }
        public static async Task<IntegrationAccount> ImportAccount(this IntegrationServiceContext context, IntegrationAccount account, bool shallow = false)
        {
            await Task.CompletedTask;
            try
            {

                if (account != null)
                {
                    var primary = account.GetAttributeValue<DynamicEntityReference>("parentaccountid");
                    if (primary!=null && Guid.TryParse(primary.Id, out var _primaryId))
                    {
                        await context.ImportAccountById(primary.Id, true);
                    }
                    if (!string.IsNullOrWhiteSpace(account.PrimaryContactId) && !shallow && context.Store.GetContactByExternalId(account.PrimaryContactId) == null)
                    {
                        await context.ImportContactById(account.PrimaryContactId, true);
                    }
                    var result = context.Store.ImportLegacyAccount(account);
                    context.Logger.LogInformation(
                        $"Account:{account} Successfully Imported.");
                    return result;
                }
            }
            catch (Exception err)
            {
                context.Logger.LogError(
                    $"An error occured while trying to import Account:{account}, Err:{err.Message}");
                throw;
            }
            return null;
        }

        public static async Task<IntegrationServiceContext> ImportAccounts(this IntegrationServiceContext context)
        {
            await Task.Delay(1000);
            int total_accounts = 0;
            int total = 0;
            var store = context.LegacyCrmStore;
            int skip = 0;
            int take = 100;
            int success = 0;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    total_accounts = total_accounts == 0 ? context.LegacyCrmStore.GetAccountsCount() : total_accounts;
                    var accounts = store.ReadAccounts(skip, take).ToArray().Where(x => x.State == 0).ToArray();
                    total += accounts.Length;

                    foreach (var account in accounts)
                    {
                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        try
                        {
                            await context.ImportAccount(account);
                            context.Logger.LogInformation(
                                $"Account: {account} successfully imported.");
                            success++;
                        }
                        catch (Exception err)
                        {
                            context.Logger.LogWarning(
                                $"An error occured while trying to import account: '{account}'. Err:{err.Message}");
                        }
                    }
                    context.Logger.LogInformation(
                        $"Successfully imported '{accounts.Length}' legacy accounts. We have successfully imported '{success}' out of {total} thus far ({total * 100 / total_accounts}%).");
                    if (accounts.Length > take)
                    {
                        break;
                    }
                    skip += take;


                }
                catch (Exception err)
                {
                    context.Logger.LogError(
                       $"An error occured wile trying to import legacy accounts. Err:{err.Message}");
                    await Task.Delay(10 * 1000, context.CancellationToken);

                }
            }
            return context;
        }

    }
}
