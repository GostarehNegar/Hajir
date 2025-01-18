using GN.Library.Xrm;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirJobTitle : XrmEntity<XrmHajirJobTitle, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.HajirSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "jobtitle";
            public const string JobTitleId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
        }

        public XrmHajirJobTitle() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.JobTitleId)]
        public System.Nullable<System.Guid> JobTitleId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.JobTitleId);
            }
            set
            {
                this.SetAttributeValue(Schema.JobTitleId, value);
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



        [AttributeLogicalNameAttribute(Schema.JobTitleId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.JobTitleId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }
}
