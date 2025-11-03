using Hajir.Crm.Integration;
using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Integration.SanadPardaz.Entities;
using Hajir.Crm.Integration.SanadPardaz.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public class SanadPardazDbContext : DbContext, ISanadPardazDbContext
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
        public DbSet<GoodGroupType> GoodGroupsType { get; set; }
        public DbSet<DetailCode> DetailCodes { get; set; }
        public DbSet<DetailType> DetailTypes { get; set; }
        public DbSet<DetailClass> DetailClasses { get; set; }

        public IntegrationAccount GetAccount(string id)
        {
            return int.TryParse(id, out var _id)
                ? this.DetailCodes.FirstOrDefault(x => x.DetailAccCode == _id)?.ToAccount()
                : null;
        }

        public IEnumerable<IntegrationAccount> GetAccounts(int skip, int take)
        {
            return this.DetailCodes
                .Where(x => x.Typee != 0 && x.Typee != null)
                .Skip(skip)
                .Take(take)
                .ToArray()
                .Select(x => x.ToAccount())
                .ToArray();
        }

        public IntegrationContact GetContact(string id)
        {
            return int.TryParse(id, out var _id)
                ? this.DetailCodes.FirstOrDefault(x => x.DetailAccCode == _id)?.ToContact()
                : null;
        }

        public IEnumerable<IntegrationContact> GetContacts(int skip, int take)
        {
            return this.DetailCodes
                .Where(x => (x.Typee == 0 || x.Typee == null) && (x.DetailClass == 99 || x.DetailClass == 22))
                .Skip(skip)
                .Take(take)
                .ToArray()
                .Select(x => x.ToContact())
                .ToArray();
        }

        public IEnumerable<IntegrationProduct> GetProducts(int skip, int take)
        {
            var q = from p in this.Goods
                    join c in this.GoodCategories on p.CatCode equals c.CatCode into cats
                    from cat in cats.DefaultIfEmpty()
                    select new IntegrationProduct
                    {
                        Id = p.GoodCode,
                        Name = p.GoodName,
                        CatName = cat == null ? string.Empty : cat.CatName,
                        CatCode = p.CatCode,
                        GroupId = p.Gid,
                        //GroupName = grp == null ? string.Empty : grp.GName
                    };
                    


            return q.Skip(skip).Take(take).ToArray();


            return this.Goods
                .Skip(skip)
                .Take(take)
                .Select(x => x.ToProduct())
                .ToArray();

        }

        public IEnumerable<IntegrationProduct> GetProductsByCategory(short catCode)
        {
            var q = from p in this.Goods
                    join c in this.GoodCategories on p.CatCode equals c.CatCode into cats
                    from cat in cats.DefaultIfEmpty()
                    where p.CatCode==catCode
                    select new IntegrationProduct
                    {
                        Id = p.GoodCode,
                        Name = p.GoodName,
                        CatName = cat == null ? string.Empty : cat.CatName,
                        CatCode = p.CatCode,
                        GroupId = p.Gid,
                        ProductNumber = p.GoodCode,
                        UnitOfMeasurement =p.CountUnit
                        
                        //GroupName = grp == null ? string.Empty : grp.GName
                    };
            return q.ToArray();
        }
    }
}
