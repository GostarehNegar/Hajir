using Hajir.Crm.Integration.SanadPardaz.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public class SanadPardazDbContext : DbContext
    {
        private readonly DbContextOptions<SanadPardazDbContext> options;
        private readonly ILogger<SanadPardazDbContext> logger;
        private readonly IServiceProvider serviceProvider;

        public SanadPardazDbContext(DbContextOptions<SanadPardazDbContext> options, ILogger<SanadPardazDbContext> logger, IServiceProvider serviceProvider) : base(options)
        {
            this.options = options;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public DbSet<Good> Goods { get; set; }
        public DbSet<GoodCategory> GoodCategories { get; set; }
        public DbSet<GoodGroup> GoodGroups { get; set; }
        public DbSet<GoodGroupType> GoodGroupsType { get; set;}
        public DbSet<DetailCode> DetailCodes { get; set; }
        public DbSet<DetailType> DetailTypes { get; set; }
        public DbSet<DetailClass> DetailClasses { get; set; }


             
    }
}
