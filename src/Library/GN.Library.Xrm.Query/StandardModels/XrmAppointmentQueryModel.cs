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
    public class XrmAppointmentQueryModel
    {
        public Guid ActivityId { get; set; }
        public string Subject { get; set; }
    }
    public class XrmAppointmentQueryModelConfiguration : IEntityTypeConfiguration<XrmAppointmentQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmAppointmentQueryModel> builder)
        {
            builder.ToTable("Appointment");
            builder.HasKey(x => x.ActivityId);
        }
    }
    public static class XrmAppointmentQueryModelExtentions
    {
        public static ChatAppointmentEntity ToChatAppointmentEntity(this XrmAppointmentQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmAppointmentQueryModel, ChatAppointmentEntity>(THIS);
        }
    }
}
