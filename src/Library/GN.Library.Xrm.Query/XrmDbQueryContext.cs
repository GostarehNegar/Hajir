using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using GN.Library.Xrm.StdSolution;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Xrm.Sdk;
using GN.Library.Xrm.Query.StandardModels;

namespace GN.Library.Xrm.Query
{
    public interface IXrmDbQueryContext : IDisposable
    {
        IQueryable<XrmAccountQueryModel> Accounts { get; }
        //IQueryable<XrmAppointmentQueryModel> Appointments { get; }
        IQueryable<XrmContactQueryModel> Contacts { get; }
        IQueryable<XrmSMSQueryModel> SMSs { get; }
        //    IQueryable<XrmIncidentQueryModel> Incidents { get; }
        //    IQueryable<XrmInvoiceQueryModel> Invoices { get; }
        IQueryable<XrmPhoneCallQueryModel> PhoneCalls { get; }
        //    IQueryable<XrmQuoteQueryModel> Quotes { get; }
        //    IQueryable<XrmSerialQueryModel> Serials { get; }
        IQueryable<XrmActivityPartyQueryModel> ActivityParties { get; }
        IQueryable<XrmSystemUserQueryModel> SystemUsers { get; }
        //    IQueryable<XrmTaskQueryModel> Tasks { get; }
    }
    public class XrmDbQueryContext : DbContext, IXrmDbQueryContext
    {
        public Guid? CurrentUserId { get; set; } = null;

        private XrmDbQueryContextSettings Settings { get; set; }

        public IQueryable<XrmAccountQueryModel> Accounts => this.Set<XrmAccountQueryModel>();
        public IQueryable<XrmSMSQueryModel> SMSs => this.Set<XrmSMSQueryModel>();
        public IQueryable<XrmContactQueryModel> Contacts => this.Set<XrmContactQueryModel>();
        //public IQueryable<XrmEmailQueryModel> Emails => this.Set<XrmEmailQueryModel>();
        //public IQueryable<XrmIncidentQueryModel> Incidents => this.Set<XrmIncidentQueryModel>();
        //public IQueryable<XrmInvoiceQueryModel> Invoices => this.Set<XrmInvoiceQueryModel>();
        public IQueryable<XrmPhoneCallQueryModel> PhoneCalls => this.Set<XrmPhoneCallQueryModel>();
        //public IQueryable<XrmQuoteQueryModel> Quotes => this.Set<XrmQuoteQueryModel>();
        //public IQueryable<XrmSerialQueryModel> Serials => this.Set<XrmSerialQueryModel>();
        public IQueryable<XrmSystemUserQueryModel> SystemUsers => this.Set<XrmSystemUserQueryModel>();
        public IQueryable<XrmActivityPartyQueryModel> ActivityParties => this.Set<XrmActivityPartyQueryModel>();
        //public IQueryable<XrmTaskQueryModel> Tasks => this.Set<XrmTaskQueryModel>();

        public XrmDbQueryContext(XrmDbQueryContextSettings settings)
        {
            this.Settings = settings;
            this.Database.SetCommandTimeout(120);

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new XrmSMSQueryModelConfiguration());
            modelBuilder.ApplyConfiguration(new XrmAccountQueryModelConfiguration());
            modelBuilder.ApplyConfiguration(new StandardModels.XrmContactQueryModelConfiguration());
            //modelBuilder.ApplyConfiguration(new XrmEmailQueryModelConfiguration());
            //modelBuilder.ApplyConfiguration(new XrmIncidentQueryModelConfiguration());
            //modelBuilder.ApplyConfiguration(new XrmInvoiceQueryModelConfiguration());
            modelBuilder.ApplyConfiguration(new XrmPhoneCallQueryModelConfiguration());
            //modelBuilder.ApplyConfiguration(new XrmQuoteQueryModelConfiguration());
            //modelBuilder.ApplyConfiguration(new XrmSerialQueryModelConfiguration());
            modelBuilder.ApplyConfiguration(new XrmSystemUserQueryModelConfiguration());
            modelBuilder.ApplyConfiguration(new XrmActivityPartyQueryModelConfiguration());
            //modelBuilder.ApplyConfiguration(new XrmTaskQueryModelConfiguration());
            //this.Database.GetDbConnection().StateChange += XrmDbQueryContext_StateChange;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this.Settings.ConnectionString);
            }
        }

        private void XrmDbQueryContext_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            bool IsContextInfoRequired = false;
            if (e.CurrentState == System.Data.ConnectionState.Open && IsContextInfoRequired)
            {
                bool success = false;
                if (this.CurrentUserId.HasValue)
                {
                    this.Database.ExecuteSqlRaw(string
                        .Format(@"DECLARE  @uid uniqueidentifier
                        SET @uid = convert(uniqueidentifier, '{0}')
                        SET CONTEXT_INFO @uid", this.CurrentUserId.Value));
                    success = true;
                }
                if (!success)
                    Console.WriteLine(
                        "Failed to set 'CONTEXT_INFO' on DBContext connection. This DBContext requires 'CONTEXT_INFO' otherwise it may have unexpected results." +
                        " Type:'{0}'", this.GetType().Name
                        );
            }
        }
    }
}
