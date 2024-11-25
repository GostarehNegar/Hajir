using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hajir.Crm.Features.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hajir.Crm.Features.Integration
{
    internal static partial class IntegrationServiceExtentions
    {
        public static async Task<IntegrationServiceContext> ImportLegacyContact(this IntegrationServiceContext context, IntegrationContact contact, bool shallow = false)
        {
            if (contact != null)
            {
                try
                {
                    await context.EnsureUsers(contact);
                    if (!string.IsNullOrWhiteSpace(contact.AccontId))
                    {
                        //var account = context.Store.GetAccountByExternalId(contact.AccontId);
                        //if (account == null && !shallow)
                        //{
                        //    account = await context.ImportAccountById(contact.AccontId);
                        //}
                    }
                    context.Store.ImportLegacyContact(contact);
                    context.Logger.LogInformation($"Contact {contact} Successfully Imported.");
                }
                catch (Exception err)
                {
                    context.Logger.LogError(
                        $"An error occured while trying to import this contact:{contact}. Err:{err.Message}");
                    throw;
                }
            }
            return context;
        }
        public static async Task<IntegrationContact> ImportContactById(this IntegrationServiceContext context, string contactId, bool shallow = false)
        {
            var contact = context.LegacyCrmStore.GetContact(contactId);
            await context.ImportLegacyContact(contact, shallow);
            return contact;

        }
        public static async Task<IntegrationServiceContext> ImportContacts(this IntegrationServiceContext context)
        {
            await Task.Delay(100);
            var store = context.LegacyCrmStore;
            var skip = 0;
            var take = 100;
            var total = 0;
            var success = 0;
            var total_contacts = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    total_contacts = total_contacts > 0 ? total_contacts : store.GetContatCount();
                    var contacts = store.ReadContacts(skip, take).ToArray().Where(x => x.State == 0).ToArray();
                    total += contacts.Length;

                    foreach (var contact in contacts)
                    {
                        if (context.CancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        try
                        {
                            await context.ImportLegacyContact(contact);
                            context.Logger.LogInformation($"Contact {contact} successfully imported.");
                            success++;
                        }
                        catch (Exception err)
                        {
                            context.Logger.LogWarning(
                                $"An error occured while trying to imprt contact: '{contact}'. Err:{err.Message}");
                        }
                    }
                    context.Logger.LogInformation(
                        $"Successfully imported '{contacts.Length}' legacy contacts. We have successfully imported '{success}' out of {total} thus far ({total * 100 / total_contacts}%).");
                    if (contacts.Length > take)
                    {
                        break;
                    }
                    skip += take;
                }
                catch (Exception err)
                {
                    context.Logger.LogError(
                        $"An error occured wile trying to import legacy contacts. Err:{err.Message}");
                    await Task.Delay(10 * 1000, context.CancellationToken);
                }
            }

            return context;

        }
    }
}
