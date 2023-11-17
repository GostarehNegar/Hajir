using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace GN.Library.Xrm.Query.StandardModels
{
    public enum SMSStatusReasons
    {
        Draft = 1,
        SentToSmSys = 276160007,
        RetrySendToSmSys = 276160008,
        Sending = 276160009,
        Sent = 276160010,
        Failed = 276160011,
        Unknown = 276160013,
        Received = 276160012,
        Delivered = 2,
        Canceled = 3,
        Scheduled = 4
    }
    public class XrmSMSQueryModel
    {
        public XrmSMSQueryModel()
        {
            RecipientContacts = new HashSet<XrmActivityPartyQueryModel>();

        }
        public Guid ActivityId { get; set; }
        public string Recipient { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        
        public string Description { get; set; }
        public bool TriggerSend { get; set; }
        public string SMSLineIdName { get; set; }
        public string SMSID { get; set; }
        public int StatusReason { get; set; }
        [ForeignKey("ActivityPartyId")]
        public virtual ICollection<XrmActivityPartyQueryModel> RecipientContacts { get; set; }


    }
    public class XrmSMSQueryModelConfiguration : IEntityTypeConfiguration<XrmSMSQueryModel>
    {

        public void Configure(EntityTypeBuilder<XrmSMSQueryModel> builder)
        {
            builder.ToTable("bmsd_sms");

            builder.HasKey(x => x.ActivityId);

            builder.Property(x => x.SMSLineIdName)
                .HasColumnName("bmsd_SMSLineIdName");

            builder.Property(p => p.Recipient)
                .HasColumnName("bmsd_recipientphonenumber");
            builder.Property(x => x.CreatedOn)
                .HasColumnName("CreatedOn")
                .HasColumnType("datetime2");

            builder.Property(x => x.ModifiedOn)
                .HasColumnName("ModifiedOn")
                .HasColumnType("datetime2");

            builder.Property(x => x.Description)
                .HasColumnName("description");

            builder.Property(p => p.Title)
                   .HasColumnName("subject");
            builder.Property(x => x.SMSID)
                .HasColumnName("bmsd_smsid");
            builder.Property(x => x.TriggerSend)
                .HasColumnName("bmsd_triggersend");

            builder.Property(x => x.StatusReason)
                .HasColumnName("statuscode");



        }
    }
    public static class XrmSMSQueryModelExtentions
    {
        //public static SMSModel ToSMS(this XrmSMSQueryModel THIS)
        //{
        //    return new SMSModel()
        //    {
        //        CreatedOn = THIS.CreatedOn,
        //        ActivityId = THIS.ActivityId,
        //        Recipient = THIS.Recipient,
        //        Title = THIS.Title,
        //        Description = THIS.Description,
        //        LineIdName = THIS.SMSLineIdName
        //    };
        //}

        public static IQueryable<XrmSMSQueryModel> GetLatestDrafts(this IQueryable<XrmSMSQueryModel> THIS, int count = 100)
        {
            return THIS
                .Where(x => x.CreatedOn > DateTime.Now.AddDays(-1))
                .Where(x => (x.TriggerSend && x.StatusReason == (int)SMSStatusReasons.Draft))
                .Include(x => x.RecipientContacts)
                .OrderBy(x => x.CreatedOn)
                .Take(count);
        }
        public static long CountDrafts(this IQueryable<XrmSMSQueryModel> THIS)
        {
            return THIS
                //.Where(x => x.CreatedOn > DateTime.Now.AddDays(-1))
                .Where(x => (x.TriggerSend && x.StatusReason == (int)SMSStatusReasons.Draft))
                .Count();
                //.Include(x => x.RecipientContacts)
                //.OrderBy(x => x.CreatedOn)
                //.Take(count);
        }

        //public static List<SMSModel> dg(this IXrmDbQueryContext THIS, int count, int minusDays)
        //{
        //    var q = from s in THIS.SMSs
        //            join x in THIS.ActivityParties on s.ActivityId equals x.ActivityId
        //            orderby s.CreatedOn
        //            where (s.CreatedOn > DateTime.Now.AddDays(minusDays))
        //            where (s.TriggerSend == true)
        //            where (s.StatusReason == (int)SMSStatusReasons.Draft)
        //            where (x.ParticipationTypeMask == 2)
        //            join c in THIS.Contacts on x.PartyId equals c.ContactId into _contacts
        //            from c1 in _contacts.DefaultIfEmpty()

        //            select new SMSModel()
        //            {
        //                ActivityId = s.ActivityId,
        //                Title = s.Title,
        //                Description = s.Description,
        //                Recipient = c1.MobilePhone,
        //                CreatedOn = s.CreatedOn
        //            };
        //    return q.Take(count).ToList();
        //}
    }
}
