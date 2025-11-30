using GN.Library.Shared.Entities;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Blazor.Services;
using Hajir.Crm.Sales;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hajir.Crm.Blazor.Components.Sales.PhoneCalls
{
    public partial class PhoneCallComponent
    {
        private SaleContact selectedContact { get; set; }
        private SaleAccount selectedAccount { get; set; }
        //private IBlazorAppServices Api { get; set; }
        private Dictionary<string, DynamicEntity> selectedEntities = new Dictionary<string, DynamicEntity>();
        private IEnumerable<DynamicEntity> selectedContacts =>
            this.selectedEntities.Values
                .Where(x => x.LogicalName == "contact" || x.LogicalName == "account");
        public async Task OnItemSelected(DynamicEntity selectedEntity)
        {
            selectedEntities.Clear();
            this.selectedContact = null;
            this.selectedContact = null;
            Console.WriteLine("clicked");
            string accountId = string.Empty;
            switch (selectedEntity.LogicalName)
            {
                case "quote":
                    accountId = selectedEntity.GetAttributeValue<DynamicEntity>(XrmQuote.Schema.CustomerId)?.Id;
                    break;
                case "contact":
                    accountId = selectedEntity.To<SaleContact>().AccountId;
                    selectedContact = selectedEntity.To<SaleContact>();
                    break;
                case "account":
                    accountId = selectedEntity.Id;
                    break;

                default:
                    break;
            }
            var contacts = await Api.GetContactbyAccountId(accountId);
            var quots = await Api.GetQuotebyAccountId(accountId);
            var account = await Api.GetAccountbyAccountId(accountId);
            this.selectedAccount = account;
            selectedEntities[account.Id] = account;
            foreach (var itam in contacts)
            {
                selectedEntities[itam.UniqueId] = itam;
            }

            foreach (var itam in quots)
            {
                selectedEntities[itam.UniqueId] = itam;
            }
            if (this.selectedContact == null)
            {
                var primaryContactId = account.GetAttributeValue<DynamicEntityReference>(XrmAccount.Schema.PrimaryContactId)?.Id;
                this.selectedContact = contacts.FirstOrDefault(x => x.Id == primaryContactId) ?? contacts.FirstOrDefault();

            }

        }


    }
}
