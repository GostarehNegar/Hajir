using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationContact
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Type { get; set; }
        public int? Class { get; set; }

        public string FullName => string.IsNullOrWhiteSpace(LastName) ? Name : $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{Id} {FullName}";
        }

    }
}
