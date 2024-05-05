using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Features.Integration
{
    public class IntegrationProduct
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string CatName { get; set; }
        public int? CatCode { get; set; }
        public string GroupName { get; set; }
        public int? GroupId { get; set; }
    }
}
