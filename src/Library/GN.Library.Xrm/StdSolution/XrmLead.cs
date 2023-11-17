using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    /// <summary>
    /// Ref: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/lead?view=dynamics-ce-odata-9
    /// </summary>
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmLead : XrmEntity<XrmLead, DefaultStateCodes, DefaultStatusCodes>
    {
        public new class Schema : XrmEntity.Schema
        {
            public const string LogicalName = "lead";
            public const string LeadId = "leadid";
            public const string Subject = "subject";
            public const string FirstName = "firstname";
            public const string FullName = "fullname";
            public const string LastName = "lastname";
            public const string EmailAddress1 = "emailaddress1";
            public const string MobilePhone = "mobilephone";
            public const string BusinessPhone = "telephone1";

            /// <summary>
            /// 
            /// Ref: https://docs.microsoft.com/en-us/previous-versions/dynamicscrm-2016/administering-dynamics-365/dn531157(v=crm.8)
            /// </summary>
            public enum StateCodes
            {
                Open = 0,
                Qualified = 1,
                Disqualified = 2
            }
            public enum StatusCodes
            {
                /// <summary>
                /// New=1 (State = Open)
                /// </summary>
                New = 1,
                /// <summary>
                /// Contacted=2 (State = Open)
                /// </summary>
                Contacted = 2,
                /// <summary>
                /// Qualified=3 (State = Qualified)
                /// </summary>
                Qualified = 3,
                /// <summary>
                /// Lost=4 (State = Disqualified)
                /// </summary>
                Lost = 4,
                /// <summary>
                /// CanNotContact=5 (State = Disqualified)
                /// </summary>

                CannotContact = 5,
                /// <summary>
                /// NoLongerInterested=6 (State = Disqualified)
                /// </summary>
                NoLongerInterested = 6,
                /// <summary>
                /// Canceled=7 (State = Disqualified)
                /// </summary>

                Canceled = 7
            }
        }

        public XrmLead() : base(Schema.LogicalName)
        {

        }

        [AttributeLogicalNameAttribute(Schema.LeadId)]
        public System.Nullable<System.Guid> LeadId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>(Schema.LeadId);
            }
            set
            {
                this.SetAttributeValue(Schema.LeadId, value);
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

        [AttributeLogicalNameAttribute(Schema.LeadId)]
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

        [AttributeLogicalNameAttribute(Schema.Subject)]
        public string Subject
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.Subject);
            }
            set
            {
                this.SetAttributeValue(Schema.Subject, value);
            }
        }
        [AttributeLogicalNameAttribute(Schema.FullName)]
        public string FullName
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.FullName);
            }
        }
        [AttributeLogicalNameAttribute(Schema.BusinessPhone)]
        public string BusinessPhone
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.BusinessPhone);
            }
            set
            {
                this.SetAttributeValue(Schema.BusinessPhone, value);
            }
        }
        [AttributeLogicalNameAttribute(Schema.MobilePhone)]
        public string MobilePhone
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.MobilePhone);
            }
            set
            {
                this.SetAttributeValue(Schema.MobilePhone, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.FirstName)]
        public string FirstName
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.FirstName);
            }
            set
            {
                this.SetAttributeValue(Schema.FirstName, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.LastName)]
        public string LastName
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.LastName);
            }
            set
            {
                this.SetAttributeValue(Schema.LastName, value);
            }
        }

        [AttributeLogicalNameAttribute(Schema.EmailAddress1)]
        public string EmailAddress1
        {
            get
            {
                return this.GetAttributeValue<string>(Schema.EmailAddress1);
            }
            set
            {
                this.SetAttributeValue(Schema.EmailAddress1, value);
            }
        }



    }
}
