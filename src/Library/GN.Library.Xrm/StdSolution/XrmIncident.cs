using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmIncident : XrmEntity<XrmIncident, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "incident";
            public const string IncidentId = LogicalName + "id";
        }
            public XrmIncident() : base(Schema.LogicalName)
        {

        }
        [AttributeLogicalNameAttribute(Schema.IncidentId)]
        public System.Nullable<System.Guid> LeadId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.IncidentId);
            }
            set
            {
                this.SetAttributeValue(Schema.IncidentId, value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
            }
        }
        [AttributeLogicalNameAttribute(Schema.IncidentId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.LeadId = value;
            }
        }
    }
}
