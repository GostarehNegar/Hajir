using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration
{
    public class IntegrationUser : DynamicEntity
    {

        public override string ToString()
        {
            return $"{GetAttributeValue<string>("lastname")}";
        }
        public string FullName => GetAttributeValue<string>("fullname");
    }
}
