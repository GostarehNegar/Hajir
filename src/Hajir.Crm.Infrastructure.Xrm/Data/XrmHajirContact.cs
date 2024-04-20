using GN.Library.Xrm.StdSolution;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirContact:XrmContact
    {
        public new class Schema : XrmContact.Schema
        {
            public const string RHSAddress="rhs_address";
            public const string RHSCity = "rhs_city";
            public const string RHSCityPhoneCode = "rhs_codecityphone";

        }

        
    }
}
