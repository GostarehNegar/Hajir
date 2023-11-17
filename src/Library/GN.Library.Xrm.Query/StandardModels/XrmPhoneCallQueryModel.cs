using GN.Library.Shared.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmPhoneCallQueryModel
    {
        public Guid ActivityId { get; set; }
        public string UniqueID { get; set; }
    }
    public class XrmPhoneCallQueryModelConfiguration : IEntityTypeConfiguration<XrmPhoneCallQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmPhoneCallQueryModel> builder)
        {
            builder.ToTable("PhoneCall");
            builder.HasKey(x => x.ActivityId);

            builder.Property(x => x.UniqueID)
                .HasColumnName("bmsd_uniqueid");
        }
    }
    public static class XrmPhoneCallQueryModelExtentions
    {
        public static ChatPhoneCallEntity ToChatPhoneCallEntity(this XrmPhoneCallQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmPhoneCallQueryModel, ChatPhoneCallEntity>(THIS);
        }
    }
}
