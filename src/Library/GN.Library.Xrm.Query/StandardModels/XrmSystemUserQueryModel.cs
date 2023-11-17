using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GN;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Xrm;
using System.Linq;
using GN.Library.Xrm.Query.StandardModels;
using GN.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GN.Library.Shared.Chats;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmSystemUserQueryModel
    {
        public Guid SystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string DomainName { get; set; }
        public string ExtensionNumber { get; set; }
        public string MobilePhone { get; set; }
    }
    public class XrmSystemUserQueryModelConfiguration : IEntityTypeConfiguration<XrmSystemUserQueryModel>
    {
       
        public void Configure(EntityTypeBuilder<XrmSystemUserQueryModel> builder)
        {
            builder.ToTable("SystemUser");
            builder.HasKey(x => x.SystemUserId);
           

            builder.Property(x => x.ExtensionNumber)
                .HasColumnName("bmsd_callSysDialingExtensionName");
        }
    }
    public static class XrmSystemUserQueryModelExtentions
    {
        public static ChatUserEntity ToChatUserEntity(this XrmSystemUserQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmSystemUserQueryModel, ChatUserEntity>(THIS);
        }

        public static IQueryable<XrmSystemUserQueryModel> FindByExtension(this IQueryable<XrmSystemUserQueryModel> THIS , string extension)
        {
            return THIS.Where(x => x.ExtensionNumber == extension);
        }
    }
}
