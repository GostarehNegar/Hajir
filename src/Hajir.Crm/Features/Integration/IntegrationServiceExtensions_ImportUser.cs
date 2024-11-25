using GN.Library.Shared.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Features.Integration
{
    internal static partial class IntegrationServiceExtentions
    {
        public static async Task<IntegrationServiceContext> ImportUser(this IntegrationServiceContext ctx, IntegrationUser user)
        {
            if (user != null)
            {
                try
                {
                    ctx.Store.ImportLegacyUser(user);
                }
                catch (Exception err)
                {
                    ctx.Logger.LogError(
                        $"An error occured while trying to import user :{user}. Err:{err.GetBaseException().Message} ");
                    throw;
                }
            }
            return ctx;

        }
        
        public static async Task<IntegrationServiceContext> EnsureUsers(this IntegrationServiceContext ctx, DynamicEntity entity)
        {
            if (entity != null)
            {
                try
                {
                    var owner_id = entity.GetOwnerId();
                    if (owner_id.HasValue && ctx.Store.GetUserById(owner_id.ToString())==null)
                    {
                        var user = ctx.LegacyCrmStore.GetUser(owner_id.Value.ToString());
                        await ctx.ImportUser(user);
                    }
                    var created_by = entity.GetCreatedBy();
                    if (created_by.HasValue && ctx.Store.GetUserById(created_by.ToString()) == null)
                    {
                        var user = ctx.LegacyCrmStore.GetUser(created_by.Value.ToString());
                        await ctx.ImportUser(user);
                    }
                    var modified_by = entity.GetCreatedBy();
                    if (modified_by.HasValue && ctx.Store.GetUserById(modified_by.ToString()) == null)
                    {
                        var user = ctx.LegacyCrmStore.GetUser(modified_by.Value.ToString());
                        await ctx.ImportUser(user);
                    }


                }
                catch (Exception err)
                {
                    //ctx.Logger.LogError(
                    //    $"An error occured while trying to import user :{user}. Err:{err.GetBaseException().Message} ");
                    throw;
                }
            }
            return ctx;

        }
    }
}
