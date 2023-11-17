using GN.Library.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmSerialQueryModel
    {
        public XrmSerialQueryModel()
        {
            IncidentBases = new HashSet<XrmIncidentQueryModel>();
        }

        public Guid GnSerialId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedOnBehalfBy { get; set; }
        public Guid? ModifiedOnBehalfBy { get; set; }
        public Guid OwnerId { get; set; }
        public int OwnerIdType { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        public int Statecode { get; set; }
        public int? Statuscode { get; set; }
        public byte[] VersionNumber { get; set; }
        public int? ImportSequenceNumber { get; set; }
        public DateTime? OverriddenCreatedOn { get; set; }
        public int? TimeZoneRuleVersionNumber { get; set; }
        public int? UtcconversionTimeZoneCode { get; set; }
        public int? GnLock { get; set; }
        public int? GnSupportduration { get; set; }
        public int? GnDep { get; set; }
        public string GnName { get; set; }
        public string GnShopperName { get; set; }
        public string GnFactorNumber { get; set; }
        public int? GnProductname { get; set; }
        public DateTime? GnShoppingDate { get; set; }
        public string GnCompany { get; set; }
        public Guid? GnFactor { get; set; }
        public bool? GnHardlock { get; set; }
        public Guid? GnAccount { get; set; }
        public DateTime? GnExpiredDate { get; set; }
        public string GnOldguid { get; set; }
        public int? GnLockType { get; set; }
        public string GnPackage { get; set; }
        public int? GnProductcode { get; set; }
        public string NewGnHid { get; set; }

        public virtual XrmAccountQueryModel GnAccountNavigation { get; set; }
        public virtual XrmInvoiceQueryModel GnFactorNavigation { get; set; }
        public virtual ICollection<XrmIncidentQueryModel> IncidentBases { get; set; }
    }
    public class XrmSerialQueryModelConfiguration : IEntityTypeConfiguration<XrmSerialQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmSerialQueryModel> builder)
        {
            builder.HasKey(e => e.GnSerialId);
            builder.ToTable("gn_serial");

            builder.Property(e => e.GnSerialId)
                   .HasColumnName("gn_serialId");

            builder.Property(e => e.CreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.GnAccount)
                .HasColumnName("gn_account");

            builder.Property(e => e.GnCompany)
                .HasColumnName("gn_company");

            builder.Property(e => e.GnDep)
                .HasColumnName("gn_dep");

            builder.Property(e => e.GnExpiredDate)
                .HasColumnType("datetime")
                .HasColumnName("gn_expired_date");

            builder.Property(e => e.GnFactor)
                .HasColumnName("gn_factor");

            builder.Property(e => e.GnFactorNumber)
                .HasColumnName("gn_factor_number");

            builder.Property(e => e.GnHardlock)
                .HasColumnName("gn_hardlock");

            builder.Property(e => e.GnLock)
                .HasColumnName("gn_lock");

            builder.Property(e => e.GnLockType)
                .HasColumnName("gn_lock_type");

            builder.Property(e => e.GnName)
                .HasColumnName("gn_name");

            builder.Property(e => e.GnOldguid)
                .HasColumnName("gn_oldguid");

            builder.Property(e => e.GnPackage)
                .HasColumnName("gn_Package");

            builder.Property(e => e.GnProductcode)
                .HasColumnName("gn_productcode");

            builder.Property(e => e.GnProductname)
                .HasColumnName("gn_productname");

            builder.Property(e => e.GnShopperName)
                .HasColumnName("gn_shopper_name");

            builder.Property(e => e.GnShoppingDate)
                .HasColumnType("datetime")
                .HasColumnName("gn_shopping_date");

            builder.Property(e => e.GnSupportduration)
                .HasColumnName("gn_supportduration");

            builder.Property(e => e.ModifiedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.NewGnHid)
                .HasColumnName("new_gn_hid");

            builder.Property(e => e.OverriddenCreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.Statecode)
                .HasColumnName("statecode");

            builder.Property(e => e.Statuscode)
                .HasColumnName("statuscode");

            builder.Property(e => e.UtcconversionTimeZoneCode)
                .HasColumnName("UTCConversionTimeZoneCode");

            builder.Property(e => e.VersionNumber)
                .IsRowVersion()
                .IsConcurrencyToken();

            //builder.HasOne(d => d.GnAccountNavigation)
            //    .WithMany(p => p.GnSerialBases)
            //    .HasForeignKey(d => d.GnAccount)
            //    .HasConstraintName("gn_account_account");

            builder.HasOne(d => d.GnFactorNavigation)
                .WithMany(p => p.GnSerialBases)
                .HasForeignKey(d => d.GnFactor)
                .HasConstraintName("gn_invoice_factor");
        }
    }
    public static class XrmSerialQueryModelExtentions
    {
        public static SerialEntity ToChatSerialEntity(this XrmSerialQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmSerialQueryModel, SerialEntity>(THIS);
        }
    }
}
