using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirProduct : XrmProduct
    {
        public new class Schema : HajirProductEntity.Schema
        {

        }
    }
}
