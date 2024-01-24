using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }


        public override string ToString()
        {
            return $"{Id} {Name}";
        }

    }
}
