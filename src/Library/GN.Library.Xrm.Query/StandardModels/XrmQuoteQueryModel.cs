using GN.Library.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmQuoteQueryModel
    {
        public Guid QuoteId { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        public Guid? PriceLevelId { get; set; }
        public Guid? OpportunityId { get; set; }
        public string QuoteNumber { get; set; }
        public int RevisionNumber { get; set; }
        public string Name { get; set; }
        public int? PricingErrorCode { get; set; }
        public string Description { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FreightAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalLineItemAmount { get; set; }
        public decimal? TotalLineItemDiscountAmount { get; set; }
        public decimal? TotalAmountLessFreight { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public decimal? TotalTax { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public DateTime? ClosedOn { get; set; }
        public DateTime? RequestDeliveryBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int StateCode { get; set; }
        public int? StatusCode { get; set; }
        public byte[] VersionNumber { get; set; }
        public bool? WillCall { get; set; }
        public string BillToName { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public Guid? UniqueDscId { get; set; }
        public int? ImportSequenceNumber { get; set; }
        public decimal? ExchangeRate { get; set; }
        public DateTime? OverriddenCreatedOn { get; set; }
        public int? UtcconversionTimeZoneCode { get; set; }
        public decimal? TotalLineItemDiscountAmountBase { get; set; }
        public decimal? TotalAmountLessFreightBase { get; set; }
        public decimal? DiscountAmountBase { get; set; }
        public decimal? FreightAmountBase { get; set; }
        public decimal? TotalAmountBase { get; set; }
        public decimal? TotalDiscountAmountBase { get; set; }
        public decimal? TotalTaxBase { get; set; }
        public decimal? TotalLineItemAmountBase { get; set; }
        public Guid? CreatedOnBehalfBy { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? ModifiedOnBehalfBy { get; set; }
        public Guid? CustomerId { get; set; }
        public int OwnerIdType { get; set; }
        public string CustomerIdName { get; set; }
        public int? CustomerIdType { get; set; }
        public string CustomerIdYomiName { get; set; }
        public Guid? ProcessId { get; set; }
        public Guid? StageId { get; set; }
        public decimal? GnDiscountPercent { get; set; }
        public Guid? GnRecipient { get; set; }
        public decimal? GnTaxpercent { get; set; }
        public string GnQuotenote { get; set; }
        public DateTime? GnDate { get; set; }
        public int? GnOrganization { get; set; }
        public int? GnQuotenumber { get; set; }
        public string TraversedPath { get; set; }
        public DateTime? LastOnHoldTime { get; set; }
        public int? OnHoldTime { get; set; }
        public decimal? GnPrepay { get; set; }
        public decimal? GnPrepayBase { get; set; }
        public Guid? GnCostCenterId { get; set; }

        public virtual XrmContactQueryModel GnRecipientNavigation { get; set; }

    }
    public class XrmQuoteQueryModelConfiguration : IEntityTypeConfiguration<XrmQuoteQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmQuoteQueryModel> builder)
        {
            builder.ToTable("Quote");
            builder.HasKey(e => e.QuoteId);
            builder.Property(e => e.BillToName)
                .HasColumnName("BillTo_Name");

            
            builder.Property(e => e.ClosedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.CreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.DiscountAmount)
                .HasColumnType("money");

            builder.Property(e => e.DiscountAmountBase)
                .HasColumnType("money")
                .HasColumnName("DiscountAmount_Base");

            builder.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(23, 10)");

            builder.Property(e => e.EffectiveFrom)
                .HasColumnType("datetime");

            builder.Property(e => e.EffectiveTo)
                .HasColumnType("datetime");

            builder.Property(e => e.ExchangeRate)
                .HasColumnType("decimal(23, 10)");

            builder.Property(e => e.ExpiresOn)
                .HasColumnType("datetime");

            builder.Property(e => e.FreightAmount)
                .HasColumnType("money");

            builder.Property(e => e.FreightAmountBase)
                .HasColumnType("money")
                .HasColumnName("FreightAmount_Base");

            builder.Property(e => e.GnCostCenterId)
                .HasColumnName("gn_CostCenterId");

            builder.Property(e => e.GnDate)
                .HasColumnType("datetime")
                .HasColumnName("gn_date");

            builder.Property(e => e.GnDiscountPercent)
                .HasColumnType("decimal(23, 10)")
                .HasColumnName("gn_DiscountPercent");

            builder.Property(e => e.GnOrganization)
                .HasColumnName("gn_organization");

            builder.Property(e => e.GnPrepay)
                .HasColumnType("money")
                .HasColumnName("gn_prepay");

            builder.Property(e => e.GnPrepayBase)
                .HasColumnType("money")
                .HasColumnName("gn_prepay_Base");

            builder.Property(e => e.GnQuotenote)
                .HasColumnName("gn_quotenote");

            builder.Property(e => e.GnQuotenumber)
                .HasColumnName("gn_quotenumber");

            builder.Property(e => e.GnRecipient)
                .HasColumnName("gn_recipient");

            builder.Property(e => e.GnTaxpercent)
                .HasColumnType("decimal(23, 10)")
                .HasColumnName("gn_taxpercent");

            builder.Property(e => e.LastOnHoldTime)
                .HasColumnType("datetime");

            builder.Property(e => e.ModifiedOn)
                .HasColumnType("datetime");


            builder.Property(e => e.OverriddenCreatedOn)
                .HasColumnType("datetime");


            builder.Property(e => e.RequestDeliveryBy)
                .HasColumnType("datetime");


            builder.Property(e => e.TotalAmount)
                .HasColumnType("money");

            builder.Property(e => e.TotalAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalAmount_Base");

            builder.Property(e => e.TotalAmountLessFreight)
                .HasColumnType("money");

            builder.Property(e => e.TotalAmountLessFreightBase)
                .HasColumnType("money")
                .HasColumnName("TotalAmountLessFreight_Base");

            builder.Property(e => e.TotalDiscountAmount)
                .HasColumnType("money");

            builder.Property(e => e.TotalDiscountAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalDiscountAmount_Base");

            builder.Property(e => e.TotalLineItemAmount)
                .HasColumnType("money");

            builder.Property(e => e.TotalLineItemAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalLineItemAmount_Base");

            builder.Property(e => e.TotalLineItemDiscountAmount)
                .HasColumnType("money");

            builder.Property(e => e.TotalLineItemDiscountAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalLineItemDiscountAmount_Base");

            builder.Property(e => e.TotalTax).HasColumnType("money");

            builder.Property(e => e.TotalTaxBase)
                .HasColumnType("money")
                .HasColumnName("TotalTax_Base");


            builder.Property(e => e.UtcconversionTimeZoneCode)
                .HasColumnName("UTCConversionTimeZoneCode");

            builder.Property(e => e.VersionNumber)
                .IsRowVersion()
                .IsConcurrencyToken();

            //builder.HasOne(d => d.GnRecipientNavigation)
            //    .WithMany(p => p.QuoteBases)
            //    .HasForeignKey(d => d.GnRecipient)
            //    .HasConstraintName("gn_contact_quote_recipient");

        }
    }
    public static class XrmQuoteQueryModelExtentions
    {
        public static QuoteEntity ToChatQuoteEntity(this XrmQuoteQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmQuoteQueryModel, QuoteEntity>(THIS);
        }
    }
}
