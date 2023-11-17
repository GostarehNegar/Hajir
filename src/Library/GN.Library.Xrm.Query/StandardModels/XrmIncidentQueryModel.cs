
using GN.Library.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmIncidentQueryModel
    {
        public XrmIncidentQueryModel()
        {
            InverseGnParentCase = new HashSet<XrmIncidentQueryModel>();
        }

        public Guid IncidentId { get; set; }
        public Guid? OwningBusinessUnit { get; set; }
        public Guid? ContractDetailId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? ContractId { get; set; }
        public int? ActualServiceUnits { get; set; }
        public int? CaseOriginCode { get; set; }
        public int? BilledServiceUnits { get; set; }
        public int? CaseTypeCode { get; set; }
        public string ProductSerialNumber { get; set; }
        public string Title { get; set; }
        public Guid? ProductId { get; set; }
        public int? ContractServiceLevelCode { get; set; }
        public string Description { get; set; }
        public bool? IsDecrementing { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string TicketNumber { get; set; }
        public int? PriorityCode { get; set; }
        public int? CustomerSatisfactionCode { get; set; }
        public int? IncidentStageCode { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? FollowupBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public byte[] VersionNumber { get; set; }
        public int StateCode { get; set; }
        public int? SeverityCode { get; set; }
        public int? StatusCode { get; set; }
        public Guid? ResponsibleContactId { get; set; }
        public Guid? KbArticleId { get; set; }
        public int? TimeZoneRuleVersionNumber { get; set; }
        public int? ImportSequenceNumber { get; set; }
        public int? UtcconversionTimeZoneCode { get; set; }
        public DateTime? OverriddenCreatedOn { get; set; }
        public decimal? ExchangeRate { get; set; }
        public Guid? ModifiedOnBehalfBy { get; set; }
        public Guid? TransactionCurrencyId { get; set; }
        public Guid? CreatedOnBehalfBy { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid OwnerId { get; set; }
        public int OwnerIdType { get; set; }
        public int? CustomerIdType { get; set; }
        public string CustomerIdName { get; set; }
        public string CustomerIdYomiName { get; set; }
        public Guid? ExistingCase { get; set; }
        public int? ServiceStage { get; set; }
        public bool? CheckEmail { get; set; }
        public bool? ActivitiesComplete { get; set; }
        public Guid? StageId { get; set; }
        public bool? FollowUpTaskCreated { get; set; }
        public Guid? EntityImageId { get; set; }
        public Guid? ProcessId { get; set; }
        public int? GnWbslevel { get; set; }
        public int? GnPreviousState { get; set; }
        public Guid? GnUser { get; set; }
        public Guid? GnCaseType { get; set; }
        public Guid? GnClassId { get; set; }
        public Guid? GnParentCaseId { get; set; }
        public Guid? GnCaseWorker { get; set; }
        public Guid? GnContactPerson { get; set; }
        public string GnOlduser { get; set; }
        public string GnIsimported { get; set; }
        public string GnContactInfo { get; set; }
        public int? GnAge { get; set; }
        public int? GnStateAge { get; set; }
        public Guid? GnRequestedById { get; set; }
        public Guid? GnCostCenter { get; set; }
        public Guid? GnSerial { get; set; }
        public Guid? GnQcby { get; set; }
        public Guid? GnRequirementId { get; set; }
        public Guid? GnBusinessType { get; set; }
        public Guid? GnCaseManagerId { get; set; }
        public Guid? SlainvokedId { get; set; }
        public bool? RouteCase { get; set; }
        public bool? IsEscalated { get; set; }
        public bool? CustomerContacted { get; set; }
        public Guid? MasterId { get; set; }
        public bool? BlockedProfile { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public double? SentimentValue { get; set; }
        public bool? Merged { get; set; }
        public DateTime? ResponseBy { get; set; }
        public DateTime? EscalatedOn { get; set; }
        public Guid? ParentCaseId { get; set; }
        public Guid? SocialProfileId { get; set; }
        public int? MessageTypeCode { get; set; }
        public int? ResolveBySlastatus { get; set; }
        public DateTime? ResolveBy { get; set; }
        public double? InfluenceScore { get; set; }
        public Guid? EntitlementId { get; set; }
        public int? FirstResponseSlastatus { get; set; }
        public bool? FirstResponseSent { get; set; }
        public string TraversedPath { get; set; }
        public int? OnHoldTime { get; set; }
        public DateTime? LastOnHoldTime { get; set; }
        public Guid? ResolveByKpiid { get; set; }
        public Guid? FirstResponseByKpiid { get; set; }
        public Guid? Slaid { get; set; }
        public Guid? CreatedByExternalParty { get; set; }
        public Guid? ModifiedByExternalParty { get; set; }
        public bool? DecrementEntitlementTerm { get; set; }
        public virtual XrmContactQueryModel GnContactPersonNavigation { get; set; }
        public virtual XrmIncidentQueryModel GnParentCase { get; set; }
        public virtual XrmSerialQueryModel GnSerialNavigation { get; set; }
        public virtual XrmContactQueryModel PrimaryContact { get; set; }
        public virtual XrmContactQueryModel ResponsibleContact { get; set; }
        public virtual ICollection<XrmIncidentQueryModel> InverseGnParentCase { get; set; }
    }
    public class XrmIncidentQueryModelConfiguration : IEntityTypeConfiguration<XrmIncidentQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmIncidentQueryModel> builder)
        {
            builder.ToTable("Incident");

            builder.HasKey(e => e.IncidentId)
                     .HasName("cndx_PrimaryKey_Incident");

            builder.Property(e => e.LastOnHoldTime)
                .HasColumnType("datetime");

            builder.Property(e => e.ModifiedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.OverriddenCreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.CreatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.FollowupBy)
                .HasColumnType("datetime");

            builder.Property(e => e.EscalatedOn)
                .HasColumnType("datetime");

            builder.Property(e => e.ResolveBy)
                .HasColumnType("datetime");
            builder.Property(e => e.ResponseBy)
                .HasColumnType("datetime");

            builder.Property(e => e.ExchangeRate)
                .HasColumnType("decimal(23, 10)");

            builder.Property(e => e.FirstResponseByKpiid)
                .HasColumnName("FirstResponseByKPIId");

            builder.Property(e => e.FirstResponseSlastatus)
                .HasColumnName("FirstResponseSLAStatus");

            builder.Property(e => e.GnAge)
                .HasColumnName("gn_Age");

            builder.Property(e => e.GnBusinessType)
                .HasColumnName("gn_BusinessType");

            builder.Property(e => e.GnCaseManagerId)
                .HasColumnName("gn_CaseManagerId");

            builder.Property(e => e.GnCaseType)
                .HasColumnName("gn_CaseType");

            builder.Property(e => e.GnCaseWorker)
                .HasColumnName("gn_CaseWorker");

            builder.Property(e => e.GnClassId)
                .HasColumnName("gn_ClassId");

            builder.Property(e => e.GnContactInfo)
                .HasColumnName("gn_ContactInfo");

            builder.Property(e => e.GnContactPerson)
                .HasColumnName("gn_ContactPerson");

            builder.Property(e => e.GnCostCenter)
                .HasColumnName("gn_CostCenter");

            builder.Property(e => e.GnIsimported)
                .HasColumnName("gn_isimported");

            builder.Property(e => e.GnOlduser)
                .HasColumnName("gn_olduser");

            builder.Property(e => e.GnParentCaseId)
                .HasColumnName("gn_ParentCaseId");

            builder.Property(e => e.GnPreviousState)
                .HasColumnName("gn_PreviousState");

            builder.Property(e => e.GnQcby)
                .HasColumnName("gn_QCBy");

            builder.Property(e => e.GnRequestedById)
                .HasColumnName("gn_RequestedById");

            builder.Property(e => e.GnRequirementId)
                .HasColumnName("gn_RequirementId");

            builder.Property(e => e.GnSerial)
                .HasColumnName("gn_Serial");

            builder.Property(e => e.GnStateAge)
                .HasColumnName("gn_StateAge");

            builder.Property(e => e.GnUser)
                .HasColumnName("gn_User");

            builder.Property(e => e.GnWbslevel)
                .HasColumnName("gn_WBSLevel");



            builder.Property(e => e.ResolveByKpiid)
                .HasColumnName("ResolveByKPIId");

            builder.Property(e => e.ResolveBySlastatus)
                .HasColumnName("ResolveBySLAStatus");

            builder.Property(e => e.Slaid)
                .HasColumnName("SLAId");

            builder.Property(e => e.SlainvokedId)
                .HasColumnName("SLAInvokedId");

            builder.Property(e => e.UtcconversionTimeZoneCode).HasColumnName("UTCConversionTimeZoneCode");

            builder.Property(e => e.VersionNumber)
                .IsRowVersion()
                .IsConcurrencyToken();

            //builder.HasOne(d => d.GnContactPersonNavigation)
            //    .WithMany(p => p.IncidentBaseGnContactPersonNavigations)
            //    .HasForeignKey(d => d.GnContactPerson)
            //    .HasConstraintName("gn_contact_incident_ContactPerson");

            builder.HasOne(d => d.GnParentCase)
                .WithMany(p => p.InverseGnParentCase)
                .HasForeignKey(d => d.GnParentCaseId)
                .HasConstraintName("gn_incident_incident");

            builder.HasOne(d => d.GnSerialNavigation)
                .WithMany(p => p.IncidentBases)
                .HasForeignKey(d => d.GnSerial)
                .HasConstraintName("gn_gn_serial_incident_Serial");

            //builder.HasOne(d => d.PrimaryContact)
            //    .WithMany(p => p.IncidentBasePrimaryContacts)
            //    .HasForeignKey(d => d.PrimaryContactId)
            //    .HasConstraintName("contact_as_primary_contact");

            //builder.HasOne(d => d.ResponsibleContact)
            //    .WithMany(p => p.IncidentBaseResponsibleContacts)
            //    .HasForeignKey(d => d.ResponsibleContactId)
                //.HasConstraintName("contact_as_responsible_contact");
        }

    }
    public static class XrmIncidentQueryModelExtentions
    {
        public static IncidentEntity ToChatIncidentEntity(this XrmIncidentQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmIncidentQueryModel, IncidentEntity>(THIS);
        }
    }
}
