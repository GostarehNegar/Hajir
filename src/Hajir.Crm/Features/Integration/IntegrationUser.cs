using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationUser : DynamicEntity
    {

        public override string ToString()
        {
            return $"{this.GetAttributeValue<string>("lastname")}";
        }
    }
}
