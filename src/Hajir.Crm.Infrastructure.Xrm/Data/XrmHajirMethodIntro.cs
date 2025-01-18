using GN.Library.Xrm;
using Hajir.Crm.Entities;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmHajirMethodIntroduction: XrmEntity<XrmHajirMethodIntroduction, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
            public const string LogicalName = SolutionPerfix + "methodintroduction";
            public const string MethodIntroductionId = LogicalName + "id";
            public const string Name = SolutionPerfix + "name";
        }

        public XrmHajirMethodIntroduction() : base(Schema.LogicalName) { }

        [AttributeLogicalName(Schema.MethodIntroductionId)]
        public System.Nullable<System.Guid> MethodIntroductionId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.MethodIntroductionId);
            }
            set
            {
                this.SetAttributeValue(Schema.MethodIntroductionId, value);
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



        [AttributeLogicalNameAttribute(Schema.MethodIntroductionId)]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.MethodIntroductionId = value;
            }
        }

        [AttributeLogicalNameAttribute(Schema.Name)]
        public string Name { get { return this.GetAttributeValue<string>(Schema.Name); } set { this.SetAttribiuteValue(Schema.Name, value); } }

    }

}
