
using GN.Library.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GN.Library.Xrm.Query.StandardModels
{
    public class XrmContactQueryModel
    {
        public XrmContactQueryModel()
        {
            //AccountBases = new HashSet<XrmAccountQueryModel>();
            //IncidentBaseGnContactPersonNavigations = new HashSet<XrmIncidentQueryModel>();
            //IncidentBasePrimaryContacts = new HashSet<XrmIncidentQueryModel>();
            //IncidentBaseResponsibleContacts = new HashSet<XrmIncidentQueryModel>();
            //InverseMaster = new HashSet<XrmContactQueryModel>();
            //QuoteBases = new HashSet<XrmQuoteQueryModel>();
            //SMSs = new HashSet<XrmSMSQueryModel>();
        }
        public Guid ContactId { get; set; }
        //public Guid? OriginatingLeadId { get; set; }
        //public Guid? OwningBusinessUnit { get; set; }
        //public string Salutation { get; set; }
        //public string EmailAddress1 { get; set; }
        //public string EmailAddress2 { get; set; }
        //public string EmailAddress3 { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public Guid? CreatedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        //public Guid? ModifiedBy { get; set; }
        //public int StateCode { get; set; }
        //public int? StatusCode { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string FullName { get; set; }
        public string MobilePhone { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telephone3 { get; set; }
        //public Guid? MasterId { get; set; }
        //public Guid OwnerId { get; set; }
        //public Guid? ParentCustomerId { get; set; }
        //public string ParentCustomerIdName { get; set; }
        //public virtual XrmContactQueryModel Master { get; set; }
        //public virtual ICollection<XrmAccountQueryModel> AccountBases { get; set; }
        //public virtual ICollection<XrmIncidentQueryModel> IncidentBaseGnContactPersonNavigations { get; set; }
        //public virtual ICollection<XrmIncidentQueryModel> IncidentBasePrimaryContacts { get; set; }
        //public virtual ICollection<XrmIncidentQueryModel> IncidentBaseResponsibleContacts { get; set; }
        //public virtual ICollection<XrmContactQueryModel> InverseMaster { get; set; }
        //public virtual ICollection<XrmQuoteQueryModel> QuoteBases { get; set; }
    }
    public class XrmContactQueryModelConfiguration : IEntityTypeConfiguration<XrmContactQueryModel>
    {
        public void Configure(EntityTypeBuilder<XrmContactQueryModel> builder)
        {
            builder.ToTable("Contact");
            builder.HasKey(x => x.ContactId);



            //builder.Property(e => e.CreatedOn).HasColumnType("datetime");
            //builder.Property(e => e.ModifiedOn).HasColumnType("datetime");

            //builder.Property(e => e.EmailAddress1)
            //    .HasMaxLength(100)
            //    .HasColumnName("EMailAddress1");

            //builder.Property(e => e.EmailAddress2)
            //    .HasMaxLength(100)
            //    .HasColumnName("EMailAddress2");

            //builder.Property(e => e.EmailAddress3)
            //    .HasMaxLength(100)
            //    .HasColumnName("EMailAddress3");

            //builder.HasOne(d => d.Master)
            //    .WithMany(p => p.InverseMaster)
            //    .HasForeignKey(d => d.MasterId)
            //    .HasConstraintName("contact_master_contact");

        }
    }
    public static class XrmContactQueryModelExtentions
    {
        public static ContactEntity ToChatContactEntity(this XrmContactQueryModel THIS)
        {
            return AppHost.Services.Mapper.Map<XrmContactQueryModel, ContactEntity>(THIS);
        }
        public static IEnumerable<XrmContactQueryModel> FindByPhoneNumber(this IQueryable<XrmContactQueryModel> THIS , string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return (new XrmContactQueryModel[] { }).AsQueryable();
            //if (phoneNumber.Length > 8)
            //{
            //    phoneNumber = phoneNumber.Substring(phoneNumber.Length - 8);
            //}
            //var res= THIS
            //    .Where(x => x.MobilePhone.EndsWith(phoneNumber)
            //    || x.Telephone1.EndsWith(phoneNumber)
            //    || x.Telephone2.EndsWith(phoneNumber))
            //    .Take(1)
            //    .ToArray();


            return THIS
                .Where(x => x.MobilePhone == phoneNumber
                || x.Telephone1 == phoneNumber)
                //|| x.Telephone2 == phoneNumber)
                .Take(1)
                .ToArray();
                //|| x.Telephone3 == phoneNumber);
        }
    }
}
