using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{

	[EntityLogicalNameAttribute(Schema.LogicalName)]
	public class XrmPhoneCall : XrmActivity<XrmPhoneCall, DefaultStateCodes, DefaultStatusCodes>
	{
		public new class Schema : XrmActivity<XrmPhoneCall, DefaultStateCodes, DefaultStatusCodes>.Schema
		{
			public const string LogicalName = "phonecall";
			public const string To = "to";
			public const string From = "from";
            public const string PhoneNumber = "phonenumber";
            public const string Directioncode = "directioncode";
            public enum StateCodes
            {
                Open = 0,
                Completed = 1,
                Canceled = 2

            }
            public enum StatusCodes
            {
                Open = 1,
                Made = 2,
                Canceled = 3,
                Recieved = 4,
            }
        }

		public XrmPhoneCall() : base(Schema.LogicalName)
		{

		}
        [AttributeLogicalNameAttribute(Schema.PhoneNumber)]
        public string PhoneNumber
        {
            get { return this.GetAttributeValue<string>(Schema.PhoneNumber); }
            set { this.SetAttributeValue(Schema.PhoneNumber, value); }
        }

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute(Schema.To)]
        public IEnumerable<XrmActivityParty> To
        {
            get
            {
                EntityCollection collection = this.GetAttributeValue<EntityCollection>(Schema.To);
                if (collection != null && collection.Entities != null && (collection.Entities.Count > 0))
                {
                    return collection.Entities.Select(x => x.ToEntity<XrmActivityParty>()).AsEnumerable();

                }
                else
                {
                    return new List<XrmActivityParty>();
                    //return null;
                }
            }
            set
            {
                EntityCollection collection = new EntityCollection(value.Select(x => x.ToEntity<Entity>()).ToList());
                this.SetAttributeValue(Schema.To, collection);
            }
        }

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute(Schema.From)]
        public virtual IEnumerable<XrmActivityParty> From
        {
            get
            {
                EntityCollection collection = this.GetAttributeValue<EntityCollection>(Schema.From);
                if (collection != null && collection.Entities != null && (collection.Entities.Count > 0))
                {
                    return collection.Entities.Select(x => x.ToEntity<XrmActivityParty>()).AsEnumerable();

                }
                else
                {
                    return new List<XrmActivityParty>();
                    //return null;
                }
            }
            set
            {
                EntityCollection collection = new EntityCollection(value.Select(x => x.ToEntity<Entity>()).ToList());
                this.SetAttributeValue(Schema.From, collection);
            }
        }
    }
    public static class XrmPhoneCallExtentions
    {

        public static XrmPhoneCall LoadFrom(this XrmPhoneCall THIS)
        {
            var items = THIS.GetDataServices().GetRepository<XrmActivityParty>()
                .Queryable.Where(x => x.ActivityId.Id == THIS.ActivityId && x.ParticipationTypeMask.Value== (int) ParticipationTypeMask.ToRecipient)
                .ToArray();
            THIS.From = items;
            return THIS;

        }
        public static XrmPhoneCall LoadTo(this XrmPhoneCall THIS)
        {
            var items = THIS.GetDataServices().GetRepository<XrmActivityParty>()
                .Queryable.Where(x => x.ActivityId.Id == THIS.ActivityId && x.ParticipationTypeMask.Value == (int)ParticipationTypeMask.Sender)
                .ToArray();
            THIS.To = items;
            return THIS;

        }
        public static XrmPhoneCall AddFromEntity(this XrmPhoneCall THIS , XrmEntity entity)
        {
            THIS.From = new List<XrmActivityParty>(THIS.From ?? Array.Empty<XrmActivityParty>())
            {
                new XrmActivityParty() { PartyId = new EntityReference(entity.LogicalName, entity.Id) }
            };
            return THIS;
        }
        public static XrmPhoneCall AddToEntity(this XrmPhoneCall THIS, XrmEntity entity)
        {
            THIS.To = new List<XrmActivityParty>(THIS.To ?? Array.Empty<XrmActivityParty>())
            {
                new XrmActivityParty() { PartyId = new EntityReference(entity.LogicalName, entity.Id) }
            };
            return THIS;
        }
    }
}
