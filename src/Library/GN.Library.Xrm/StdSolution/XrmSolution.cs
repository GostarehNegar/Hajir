using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalName(Schema.LogicalName)]
    public class XrmSolution : XrmEntity<XrmSolution, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "solution";
            public const string SolutionId = LogicalName + "id";
            public const string UniqueName = "uniquename";
            public const string FriendlyName = "friendlyname";
            public const string SolutionType = "solutiontype";
            public const string IsNamaged = "ismanaged";
            public const string PublisherId = "publisherid";
            public const string IsVisible = "isvisible";
            public const string Version = "version";
            public const string InstalledOn = "installedon";
        }
        public XrmSolution() : base(Schema.LogicalName) { }

        public override Guid Id
        {
            get { return this.SolutionId ?? Guid.Empty; }
            set { this.SolutionId = value; }
        }
        [AttributeLogicalName(Schema.SolutionId)]
        public System.Nullable<Guid> SolutionId
        {
            get { return this.GetAttributeValue<Guid?>(Schema.SolutionId); }
            set { this.SetAttribiuteValue(Schema.SolutionId, value); }
        }

        [AttributeLogicalName(Schema.UniqueName)]
        public string UniqueName
        {
            get { return this.GetAttributeValue<string>(Schema.UniqueName); }
            set { this.SetAttribiuteValue(Schema.UniqueName, value); }
        }

        [AttributeLogicalName(Schema.FriendlyName)]
        public string FriendlyName
        {
            get { return this.GetAttributeValue<string>(Schema.FriendlyName); }
            set { this.SetAttribiuteValue(Schema.FriendlyName, value); }
        }

        [AttributeLogicalName(Schema.Version)]
        public string Version
        {
            get { return this.GetAttributeValue<string>(Schema.Version); }
            set { this.SetAttribiuteValue(Schema.Version, value); }
        }
        [AttributeLogicalName(Schema.IsNamaged)]
        public bool? IsManaged
        {
            get { return this.GetAttributeValue<bool?>(Schema.IsNamaged); }
            set { this.SetAttribiuteValue(Schema.IsNamaged, value); }
        }
        [AttributeLogicalName(Schema.SolutionType)]
        public int? SolutionTypeCode
        {
            get
            {
                return this.GetAttributeValue<OptionSetValue>(Schema.SolutionType) == null ? (int?)null : this.GetAttributeValue<OptionSetValue>(Schema.SolutionType).Value;
            }
            set
            {
                this.SetAttribiuteValue(Schema.SolutionType, value.HasValue ? new OptionSetValue(value.Value) : null);
            }
        }
        [AttributeLogicalName(Schema.PublisherId)]
        public Guid? PublisherId
        {
            get { var r = this.GetAttributeValue<EntityReference>(Schema.PublisherId); return r != null ? r.Id : (Guid?)null; }
            set { this.SetAttribiuteValue(Schema.PublisherId, value.HasValue ? new EntityReference("Publisher", value.Value) : null); }
        }

		public Version GetVersion()
		{
			return System.Version.TryParse(this.Version, out var _v) ? _v : null;

		}
        public override string ToString()
        {
            return string.Format("{0} {1}", this.FriendlyName, this.Version);
        }
    }
}
