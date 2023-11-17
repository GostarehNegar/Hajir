using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmSubject : XrmEntity<XrmSubject, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "subject";
            public const string SubjectId = LogicalName + "id";
            public const string Title = "title";
        }
        public XrmSubject() : base(Schema.LogicalName) { }

        public override Guid Id
        {
            get { return this.SolutionId ?? Guid.Empty; }
            set { this.SolutionId = value; }
        }
        [AttributeLogicalName(Schema.SubjectId)]
        public System.Nullable<Guid> SolutionId
        {
            get { return this.GetAttributeValue<Guid?>(Schema.SubjectId); }
            set { this.SetAttribiuteValue(Schema.SubjectId, value); }
        }
        [AttributeLogicalName(Schema.Title)]
        public string UniqueName
        {
            get { return this.GetAttributeValue<string>(Schema.Title); }
            set { this.SetAttribiuteValue(Schema.Title, value); }
        }

    }
}
