using GN.Library.Shared.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmTaskQueryModel
    {
        public Guid ActivityId { get; set; }
        public string Subject { get; set; }
    }
    public class XrmTaskQueryModelConfiguration : IEntityTypeConfiguration<XrmTaskQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmTaskQueryModel> builder)
        {
            builder.ToTable("Task");
            builder.HasKey(x => x.ActivityId);
        }
    }
    public static class XrmTaskQueryModelExtentions
    {
        public static ChatTaskEntity ToChatTaskEntity(this XrmTaskQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmTaskQueryModel, ChatTaskEntity>(THIS);
        }
    }
}
