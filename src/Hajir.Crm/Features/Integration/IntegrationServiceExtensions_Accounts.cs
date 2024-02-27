using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    internal static partial class IntegrationServiceExtentions
    {
        public static async Task<IntegrationAccount> ImportAccountById(this IntegrationServiceContext context, string accountId)
        {

            return await context.ImportAccount(context.LegacyCrmStore.GetAccount(accountId));


        }
        public static async Task<IntegrationAccount> ImportAccount(this IntegrationServiceContext context, IntegrationAccount account)
        {
            await Task.CompletedTask;
            context.AddJob(account.Id);
            var result= context.Store.ImportLegacyAccount(account);
            context.RemoveJob(account.Id);
            return result;
        }

    }
}
