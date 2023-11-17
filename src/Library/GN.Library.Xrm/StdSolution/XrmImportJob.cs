using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmImportJob : XrmEntity<XrmImportJob, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "importjob";
            public const string ImportJobId = LogicalName + "id";
        }
        public XrmImportJob() : base(Schema.LogicalName) { }

        public override Guid Id
        {
            get { return this.ImportJobId ?? Guid.Empty; }
            set { this.ImportJobId = value; }
        }
        [AttributeLogicalName(Schema.ImportJobId)]
        public System.Nullable<Guid> ImportJobId
        {
            get { return this.GetAttributeValue<Guid?>(Schema.ImportJobId); }
            set { this.SetAttribiuteValue(Schema.ImportJobId, value); }
        }

    }
}
