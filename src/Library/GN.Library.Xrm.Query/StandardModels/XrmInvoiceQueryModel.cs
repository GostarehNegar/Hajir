
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
    public class XrmInvoiceQueryModel
    {
        public XrmInvoiceQueryModel()
        {
            GnSerialBases = new HashSet<XrmSerialQueryModel>();
        }
        public Guid InvoiceId { get; set; }
        public string Name { get; set; }
        public decimal? TotalAmount { get; set; }
        public Guid? OpportunityId { get; set; }
        public int? PriorityCode { get; set; }
        public Guid? SalesOrderId { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        public Guid? PriceLevelId { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FreightAmount { get; set; }
        public decimal? TotalLineItemAmount { get; set; }
        public decimal? TotalLineItemDiscountAmount { get; set; }
        public decimal? TotalAmountLessFreight { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public Guid? CreatedBy { get; set; }
        public decimal? TotalTax { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int StateCode { get; set; }
        public int? StatusCode { get; set; }
        public string ShipToName { get; set; }
        public byte[] VersionNumber { get; set; }
        public int? PricingErrorCode { get; set; }

        public decimal? DiscountPercentage { get; set; }
        public bool? IsPriceLocked { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? OverriddenCreatedOn { get; set; }
        public Guid? TransactionCurrencyId { get; set; }
        public decimal? TotalLineItemAmountBase { get; set; }
        public decimal? TotalLineItemDiscountAmountBase { get; set; }
        public decimal? TotalTaxBase { get; set; }
        public decimal? TotalAmountLessFreightBase { get; set; }
        public decimal? DiscountAmountBase { get; set; }
        public decimal? TotalAmountBase { get; set; }
        public decimal? FreightAmountBase { get; set; }
        public decimal? TotalDiscountAmountBase { get; set; }
        public Guid? ModifiedOnBehalfBy { get; set; }
        public Guid? CreatedOnBehalfBy { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? CustomerId { get; set; }
        public int? CustomerIdType { get; set; }
        public string CustomerIdName { get; set; }
        public int OwnerIdType { get; set; }
        public string CustomerIdYomiName { get; set; }
        public int? GnOldofficialnumber { get; set; }
        public int? GnNewnumber { get; set; }
        public string GnCustomeridname { get; set; }
        public decimal? NewPaymentTotal { get; set; }
        public decimal? NewMande { get; set; }
        public string NewPaymentDesc { get; set; }
        public string GnOldguid { get; set; }
        public bool? GnOldone { get; set; }
        public int? GnOrganization { get; set; }
        public decimal? GnTaxpercent { get; set; }
        public DateTime? GnInvoiceDate { get; set; }
        public string GnDescription { get; set; }
        public int? GnPayType { get; set; }
        public int? GnOfficialInv { get; set; }
        public decimal? GnPaid { get; set; }
        public decimal? GnRemained { get; set; }
        public string NewDomainname { get; set; }
        public string NewOwn { get; set; }
        public DateTime? GnFulfilldate { get; set; }
        public decimal? GnOldtotal { get; set; }
        public decimal? GnOldtax { get; set; }
        public decimal? GnOlddiscount { get; set; }
        public int? NewState { get; set; }
        public int? NewStatus { get; set; }
        public decimal? NewPaymentTotalBase { get; set; }
        public Guid? GnContractor { get; set; }
        public decimal? GnPaidBase { get; set; }
        public decimal? GnDiscountPercent { get; set; }
        public decimal? GnRemainedBase { get; set; }
        public decimal? NewMandeBase { get; set; }
        public decimal? GnOldtaxBase { get; set; }
        public decimal? GnOldtotalBase { get; set; }
        public decimal? GnOlddiscountBase { get; set; }
        public string TraversedPath { get; set; }
        public int? OnHoldTime { get; set; }
        public DateTime? NewDueDate { get; set; }
        public string NewPaymentSerial { get; set; }
        public Guid? GnCostCenterId { get; set; }
        public DateTime? NewBalancePayment { get; set; }
        public int? NewBalanceSerial { get; set; }
        public virtual XrmAccountQueryModel GnContractorNavigation { get; set; }
        public virtual ICollection<XrmSerialQueryModel> GnSerialBases { get; set; }

    }
    public class XrmInvoiceQueryModelConfiguration : IEntityTypeConfiguration<XrmInvoiceQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmInvoiceQueryModel> builder)
        {
            builder.ToTable("Invoice");
            builder.HasKey(x => x.InvoiceId);

            builder.Property(e => e.TotalLineItemDiscountAmount)
                .HasColumnType("money");
            builder.Property(e => e.TotalTax)
                .HasColumnType("money");

            builder.Property(e => e.TotalAmount)
                .HasColumnType("money");

            builder.Property(e => e.CreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.OverriddenCreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.TotalAmountLessFreight)
             .HasColumnType("money");

            builder.Property(e => e.TotalLineItemAmount)
                .HasColumnType("money");

            builder.Property(e => e.TotalDiscountAmount)
                .HasColumnType("money");

            builder.Property(e => e.ModifiedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.DiscountAmount)
                .HasColumnType("money");
            builder.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(23, 10)");

            builder.Property(e => e.DiscountAmountBase)
                .HasColumnType("money")
                .HasColumnName("DiscountAmount_Base");

            builder.Property(e => e.DueDate)
                .HasColumnType("datetime");

            builder.Property(e => e.FreightAmount)
                            .HasColumnType("money");

            builder.Property(e => e.FreightAmountBase)
                .HasColumnType("money")
                .HasColumnName("FreightAmount_Base");

            builder.Property(e => e.GnContractor)
                .HasColumnName("gn_contractor");

            builder.Property(e => e.GnCostCenterId)
                .HasColumnName("gn_CostCenterId");

            builder.Property(e => e.GnCustomeridname)
                .HasColumnName("gn_customeridname");

            builder.Property(e => e.GnDescription)
                .HasColumnName("gn_description");

            builder.Property(e => e.GnDiscountPercent)
                .HasColumnType("decimal(23, 10)")
                .HasColumnName("gn_DiscountPercent");

            builder.Property(e => e.GnFulfilldate)
                .HasColumnType("datetime")
                .HasColumnName("gn_fulfilldate");

            builder.Property(e => e.GnInvoiceDate)
                .HasColumnType("datetime")
                .HasColumnName("gn_InvoiceDate");

            builder.Property(e => e.GnNewnumber)
                .HasColumnName("gn_newnumber");

            builder.Property(e => e.GnOfficialInv)
                .HasColumnName("gn_official_inv");

            builder.Property(e => e.GnOlddiscount)
                .HasColumnType("money")
                .HasColumnName("gn_olddiscount");

            builder.Property(e => e.GnOlddiscountBase)
                .HasColumnType("money")
                .HasColumnName("gn_olddiscount_Base");

            builder.Property(e => e.GnOldguid)
                .HasColumnName("gn_oldguid");

            builder.Property(e => e.GnOldofficialnumber)
                .HasColumnName("gn_oldofficialnumber");

            builder.Property(e => e.GnOldone)
                .HasColumnName("gn_oldone");

            builder.Property(e => e.GnOldtax)
                .HasColumnType("money")
                .HasColumnName("gn_oldtax");

            builder.Property(e => e.GnOldtaxBase)
                .HasColumnType("money")
                .HasColumnName("gn_oldtax_Base");

            builder.Property(e => e.GnOldtotal)
                .HasColumnType("money")
                .HasColumnName("gn_oldtotal");

            builder.Property(e => e.GnOldtotalBase)
                .HasColumnType("money")
                .HasColumnName("gn_oldtotal_Base");

            builder.Property(e => e.GnOrganization)
                .HasColumnName("gn_organization");

            builder.Property(e => e.GnPaid)
                .HasColumnType("money")
                .HasColumnName("gn_Paid");

            builder.Property(e => e.GnPaidBase)
                .HasColumnType("money")
                .HasColumnName("gn_paid_Base");

            builder.Property(e => e.GnPayType)
                .HasColumnName("gn_pay_type");

            builder.Property(e => e.GnRemained)
                .HasColumnType("money")
                .HasColumnName("gn_Remained");

            builder.Property(e => e.GnRemainedBase)
                .HasColumnType("money")
                .HasColumnName("gn_remained_Base");

            builder.Property(e => e.GnTaxpercent)
                .HasColumnType("decimal(23, 10)")
                .HasColumnName("gn_taxpercent");



            builder.Property(e => e.NewBalancePayment)
                .HasColumnType("datetime")
                .HasColumnName("new_balance_payment");

            builder.Property(e => e.NewBalanceSerial)
                .HasColumnName("new_balance_serial");

            builder.Property(e => e.NewDomainname)
                .HasColumnName("new_domainname");

            builder.Property(e => e.NewDueDate)
                .HasColumnType("datetime")
                .HasColumnName("new_DueDate");

            builder.Property(e => e.NewMande)
                .HasColumnType("money")
                .HasColumnName("new_mande");

            builder.Property(e => e.NewMandeBase)
                .HasColumnType("money")
                .HasColumnName("new_mande_Base");

            builder.Property(e => e.NewOwn)
                .HasColumnName("new_own");

            builder.Property(e => e.NewPaymentDesc)
                .HasColumnName("new_payment_desc");

            builder.Property(e => e.NewPaymentSerial)
                .HasColumnName("new_PaymentSerial");

            builder.Property(e => e.NewPaymentTotal)
                .HasColumnType("money")
                .HasColumnName("new_payment_total");

            builder.Property(e => e.NewPaymentTotalBase)
                .HasColumnType("money")
                .HasColumnName("new_payment_total_Base");

            builder.Property(e => e.NewState)
                .HasColumnName("new_state");

            builder.Property(e => e.NewStatus)
                .HasColumnName("new_status");


            builder.Property(e => e.ShipToName)
                .HasColumnName("ShipTo_Name");


            builder.Property(e => e.TotalAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalAmount_Base");


            builder.Property(e => e.TotalAmountLessFreightBase)
                .HasColumnType("money")
                .HasColumnName("TotalAmountLessFreight_Base");

            builder.Property(e => e.TotalDiscountAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalDiscountAmount_Base");


            builder.Property(e => e.TotalLineItemAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalLineItemAmount_Base");


            builder.Property(e => e.TotalLineItemDiscountAmountBase)
                .HasColumnType("money")
                .HasColumnName("TotalLineItemDiscountAmount_Base");

            builder.Property(e => e.TotalTaxBase)
                .HasColumnType("money")
                .HasColumnName("TotalTax_Base");

            builder.Property(e => e.VersionNumber)
                .IsRowVersion()
                .IsConcurrencyToken();

            //builder.HasOne(d => d.GnContractorNavigation)
            //    .WithMany(p => p.InvoiceBases)
            //    .HasForeignKey(d => d.GnContractor)
            //    .HasConstraintName("gn_Contractor");
        }
    }
    public static class XrmInvoiceQueryModelExtentions
    {
        public static InvoiceEntity ToChatInvoiceEntity(this XrmInvoiceQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmInvoiceQueryModel, InvoiceEntity>(THIS);
        }
    }
}
