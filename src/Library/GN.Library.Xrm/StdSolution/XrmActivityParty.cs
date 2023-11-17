using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    public enum ParticipationTypeMask
    {
        Sender = 1,
        ToRecipient = 2,
        CCRecipient = 3,
        BccRecipient = 4,
        RequiredAttendee = 5,
        OptionalAttendee = 6,
        Organizer = 7,
        Regarding = 8,
        Owner = 9,
        Resource = 10,
        Customer = 11
    }

    [System.Runtime.Serialization.DataContractAttribute()]
    [Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("activityparty")]

    public partial class XrmActivityParty : XrmEntity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
    {



        public XrmActivityParty() :
                base(EntityLogicalName)
        {
        }

        public const string EntityLogicalName = "activityparty";

        public const int EntityTypeCode = 135;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

        private void OnPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activityid")]
        public Microsoft.Xrm.Sdk.EntityReference ActivityId
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("activityid");
            }
            set
            {
                this.OnPropertyChanging("ActivityId");
                this.SetAttributeValue("activityid", value);
                this.OnPropertyChanged("ActivityId");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activitypartyid")]
        public System.Nullable<System.Guid> ActivityPartyId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>("activitypartyid");
            }
            set
            {
                this.OnPropertyChanging("ActivityPartyId");
                this.SetAttributeValue("activitypartyid", value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
                this.OnPropertyChanged("ActivityPartyId");
            }
        }

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activitypartyid")]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.ActivityPartyId = value;
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("addressused")]
        public string AddressUsed
        {
            get
            {
                return this.GetAttributeValue<string>("addressused");
            }
            set
            {
                this.OnPropertyChanging("AddressUsed");
                this.SetAttributeValue("addressused", value);
                this.OnPropertyChanged("AddressUsed");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("addressusedemailcolumnnumber")]
        public System.Nullable<int> AddressUsedEmailColumnNumber
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<int>>("addressusedemailcolumnnumber");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("donotemail")]
        public System.Nullable<bool> DoNotEmail
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<bool>>("donotemail");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("donotfax")]
        public System.Nullable<bool> DoNotFax
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<bool>>("donotfax");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("donotphone")]
        public System.Nullable<bool> DoNotPhone
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<bool>>("donotphone");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("donotpostalmail")]
        public System.Nullable<bool> DoNotPostalMail
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<bool>>("donotpostalmail");
            }
        }



        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("effort")]
        public System.Nullable<double> Effort
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<double>>("effort");
            }
            set
            {
                this.OnPropertyChanging("Effort");
                this.SetAttributeValue("effort", value);
                this.OnPropertyChanged("Effort");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("exchangeentryid")]
        public string ExchangeEntryId
        {
            get
            {
                return this.GetAttributeValue<string>("exchangeentryid");
            }
            set
            {
                this.OnPropertyChanging("ExchangeEntryId");
                this.SetAttributeValue("exchangeentryid", value);
                this.OnPropertyChanged("ExchangeEntryId");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("instancetypecode")]
        public Microsoft.Xrm.Sdk.OptionSetValue InstanceTypeCode
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("instancetypecode");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ispartydeleted")]
        public System.Nullable<bool> IsPartyDeleted
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<bool>>("ispartydeleted");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ownerid")]
        public Microsoft.Xrm.Sdk.EntityReference OwnerId
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("ownerid");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("participationtypemask")]
        public Microsoft.Xrm.Sdk.OptionSetValue ParticipationTypeMask
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("participationtypemask");
            }
            set
            {
                this.OnPropertyChanging("ParticipationTypeMask");
                this.SetAttributeValue("participationtypemask", value);
                this.OnPropertyChanged("ParticipationTypeMask");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("partyid")]
        public Microsoft.Xrm.Sdk.EntityReference PartyId
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("partyid");
            }
            set
            {
                this.OnPropertyChanging("PartyId");
                this.SetAttributeValue("partyid", value);
                this.OnPropertyChanged("PartyId");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("partyid")]
        [Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("account_activity_parties")]
        public GN.Library.Xrm.StdSolution.XrmAccount account_activity_parties
        {
            get
            {
                return this.GetRelatedEntity<GN.Library.Xrm.StdSolution.XrmAccount>("account_activity_parties", null);
            }
            set
            {
                this.OnPropertyChanging("account_activity_parties");
                this.SetRelatedEntity<GN.Library.Xrm.StdSolution.XrmAccount>("account_activity_parties", null, value);
                this.OnPropertyChanged("account_activity_parties");
            }
        }


        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activityid")]
        [Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("activitypointer_activity_parties")]
        //public GN.Library.Xrm.StdSolution.ActivityPointer activitypointer_activity_parties
        //{
        //    get
        //    {
        //        return this.GetRelatedEntity<GN.Library.Xrm.StdSolution.ActivityPointer>("activitypointer_activity_parties", null);
        //    }
        //    set
        //    {
        //        this.OnPropertyChanging("activitypointer_activity_parties");
        //        this.SetRelatedEntity<GN.Library.Xrm.StdSolution.ActivityPointer>("activitypointer_activity_parties", null, value);
        //        this.OnPropertyChanged("activitypointer_activity_parties");
        //    }
        //}



        public GN.Library.Xrm.StdSolution.XrmContact contact_activity_parties
        {
            get
            {
                return this.GetRelatedEntity<GN.Library.Xrm.StdSolution.XrmContact>("contact_activity_parties", null);
            }
            set
            {
                this.OnPropertyChanging("contact_activity_parties");
                this.SetRelatedEntity<GN.Library.Xrm.StdSolution.XrmContact>("contact_activity_parties", null, value);
                this.OnPropertyChanged("contact_activity_parties");
            }
        }

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("partyid")]
        [Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lead_activity_parties")]
        public XrmLead Lead_activity_parties
        {
            get
            {
                return this.GetRelatedEntity<GN.Library.Xrm.StdSolution.XrmLead>("lead_activity_parties", null);
            }
            set
            {
                this.OnPropertyChanging("lead_activity_parties");
                this.SetRelatedEntity<GN.Library.Xrm.StdSolution.XrmLead>("lead_activity_parties", null, value);
                this.OnPropertyChanged("lead_activity_parties");
            }
        }

    }
}