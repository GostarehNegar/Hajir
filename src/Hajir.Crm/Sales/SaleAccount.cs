using GN.Library.Shared.Entities;
using GN.Library.Xrm.StdSolution;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Sales
{
    public class SaleAccount : DynamicEntity
    {

        public SaleAccount()
        {
            this.LogicalName = XrmAccount.Schema.LogicalName;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
        public string Name
        {
            get => this.GetAttributeValue<string>(HajirCrmConstants.Schema.Account.Name);
            set => this.SetAttributeValue(HajirCrmConstants.Schema.Account.Name, value);
        }
        public string Mobile => this.GetAttributeValue<string>("mobilephone");
        public string FullName => this.GetAttributeValue<string>("fullname");
        public string Telephone => this.GetAttributeValue<string>(XrmAccount.Schema.Telephone1);
        public string AccountId => this.GetAttributeValue<DynamicEntityReference>(XrmContact.Schema.ParentCustomerId)?.Id;
    }
    public class SaleContact : DynamicEntity
    {
        //public string Id { get; set; }
        //public string Name { get; set; }
        public SaleContact()
        {
            this.LogicalName = XrmContact.Schema.LogicalName;

        }
        public override string ToString()
        {
            return $"{Name}";
        }
        public string Mobile => this.GetAttributeValue<string>("mobilephone");
        public string FullName => this.GetAttributeValue<string>("fullname");
        public string AccountId => this.GetAttributeValue<DynamicEntityReference>(XrmContact.Schema.ParentCustomerId)?.Id;

    }
    public class SalePriceList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
    public class SaleQuoteBySoheil : DynamicEntity
    {
        public override string ToString()
        {
            return $"{Name}";
        }
        //public string Mobile => this.GetAttributeValue<string>("mobilephone");
        public string QuoteNumber => this.GetAttributeValue<string>("quoteNumber");
        //public string FullName => this.GetAttributeValue<string>("fullname");
        //public string AccountId => this.GetAttributeValue<DynamicEntityReference>(XrmContact.Schema.ParentCustomerId)?.Id;
    }
}
