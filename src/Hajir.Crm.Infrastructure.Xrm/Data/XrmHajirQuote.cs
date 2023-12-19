using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirQuote : XrmQuote
    {
        public new class Schema : HajirQuoteEntity.Schema
        {

        }
    }
}
