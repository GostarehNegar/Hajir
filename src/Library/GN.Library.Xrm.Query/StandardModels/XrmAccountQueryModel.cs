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
    public class XrmAccountQueryModel
    {
        public XrmAccountQueryModel()
        {
            //GnSerialBases = new HashSet<XrmSerialQueryModel>();
            //InvoiceBases = new HashSet<XrmInvoiceQueryModel>();
        }
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telephone3 { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        public Guid? OriginatingLeadId { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public string AccountNumber { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid OwnerId { get; set; }
        public int StateCode { get; set; }
        public int? StatusCode { get; set; }
        public Guid? ParentAccountId { get; set; }
        public Guid? MasterId { get; set; }

        //public virtual XrmContactQueryModel PrimaryContact { get; set; }
        //public virtual ICollection<XrmSerialQueryModel> GnSerialBases { get; set; }
        //public virtual ICollection<XrmInvoiceQueryModel> InvoiceBases { get; set; }
        //public virtual XrmAccountQueryModel ParentAccount { get; set; }
        //public virtual XrmAccountQueryModel Master { get; set; }
        //public virtual ICollection<XrmAccountQueryModel> InverseMaster { get; set; }
        //public virtual ICollection<XrmAccountQueryModel> InverseParentAccount { get; set; }

    }
    public class XrmAccountQueryModelConfiguration : IEntityTypeConfiguration<XrmAccountQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmAccountQueryModel> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(x => x.AccountId);


            builder.Property(e => e.CreatedOn).HasColumnType("datetime");

            builder.Property(e => e.ModifiedOn).HasColumnType("datetime");
            builder.Property(e => e.EmailAddress1)
                   .HasMaxLength(100)
                   .HasColumnName("EMailAddress1");

            builder.Property(e => e.EmailAddress2)
                .HasMaxLength(100)
                .HasColumnName("EMailAddress2");

            builder.Property(e => e.EmailAddress3)
                .HasMaxLength(100)
                .HasColumnName("EMailAddress3");

            //builder.HasOne(d => d.Master)
            //    .WithMany(p => p.InverseMaster)
            //    .HasForeignKey(d => d.MasterId)
            //    .HasConstraintName("account_master_account");

            //builder.HasOne(d => d.ParentAccount)
            //    .WithMany(p => p.InverseParentAccount)
            //    .HasForeignKey(d => d.ParentAccountId)
            //    .HasConstraintName("account_parent_account");

            //builder.HasOne(d => d.PrimaryContact)
            //    .WithMany(p => p.AccountBases)
            //    .HasForeignKey(d => d.PrimaryContactId)
            //    .HasConstraintName("account_primary_contact");
        }

    }
    public static class XrmAccountQueryModelExtentions
    {
        public static ChatAccountEntity ToChatAccountEntity(this XrmAccountQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmAccountQueryModel, ChatAccountEntity>(THIS);
        }
    }
}
