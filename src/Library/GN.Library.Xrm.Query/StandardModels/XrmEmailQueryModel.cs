
using GN.Library.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmEmailQueryModel
    {
        public Guid ActivityId { get; set; }
        public string Subject { get; set; }
    }
    public class XrmEmailQueryModelConfiguration : IEntityTypeConfiguration<XrmEmailQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmEmailQueryModel> builder)
        {
            builder.ToTable("Email");
            builder.HasKey(x => x.ActivityId);
        }
    }
    public static class XrmEmailQueryModelExtentions
    {
        public static EmailEntity ToChatEmailEntity(this XrmEmailQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmEmailQueryModel, EmailEntity>(THIS);
        }
    }
}
