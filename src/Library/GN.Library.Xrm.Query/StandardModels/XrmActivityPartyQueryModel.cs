
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmActivityPartyQueryModel
    {
        public Guid ActivityPartyId { get; set; }
        public Guid ActivityId { get; set; }
        public Guid PartyId { get; set; }
        public string PartyIdName { get; set; }
        public int PartyObjectTypeCode { get; set; }
        public int ParticipationTypeMask { get; set; }
        public virtual XrmSMSQueryModel SMS { get; set; }

    }
    public class XrmActivityPartyQueryModelConfiguration : IEntityTypeConfiguration<XrmActivityPartyQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmActivityPartyQueryModel> builder)
        {
            builder.ToTable("ActivityParty");
            builder.HasKey(x => x.ActivityPartyId);

            builder.HasOne(d => d.SMS)
                .WithMany(p => p.RecipientContacts)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("bmsd_sms_activity_parties");
        }
    }

}
