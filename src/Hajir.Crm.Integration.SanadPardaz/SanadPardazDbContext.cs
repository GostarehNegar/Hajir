using Hajir.Crm.Integration;
using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Integration.PriceList;
using Hajir.Crm.Integration.SanadPardaz.Entities;
using Hajir.Crm.Integration.SanadPardaz.Internals;
using Hajir.Crm.Sales;
using Hajir.Crm.Sales.PriceLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public class SanadPardazDbContext : DbContext, ISanadPardazDbConext
    {
        private readonly DbContextOptions<SanadPardazDbContext> options;
        private readonly ILogger<SanadPardazDbContext> logger;
        private readonly IServiceProvider serviceProvider;

        public SanadPardazDbContext(DbContextOptions<SanadPardazDbContext> options, ILogger<SanadPardazDbContext> logger, IServiceProvider serviceProvider) : base(options)
        {
            this.options = options;
            //options.GetExtension<SqlServerDbContextOptionsExtensions>()
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

        public Task<PriceListItem> GetPriceAsync(string productNumber)
        {
            throw new NotImplementedException();
        }

        private IntegrationPriceListItem ReadPriceListItem(DbDataReader reader)
        {
            return new IntegrationPriceListItem
            {
                Price1 = reader.IsDBNull(reader.GetOrdinal("Fee1"))
                    ? (decimal?)null
                    : Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("Fee1"))),
                Price2 = reader.IsDBNull(reader.GetOrdinal("Fee2")) || !decimal.TryParse(reader.GetString(reader.GetOrdinal("Fee2")), out var _p)
                    ? (decimal?)null
                    : _p,
                ProductNumber = reader.IsDBNull(reader.GetOrdinal("GoodCode"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("GoodCode"))

            };
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
                    where p.CatCode == catCode
                    select new IntegrationProduct
                    {
                        Id = p.GoodCode,
                        Name = p.GoodName,
                        CatName = cat == null ? string.Empty : cat.CatName,
                        CatCode = p.CatCode,
                        GroupId = p.Gid,
                        ProductNumber = p.GoodCode,
                        UnitOfMeasurement = p.CountUnit

                        //GroupName = grp == null ? string.Empty : grp.GName
                    };
            return q.ToArray();
        }

        public async Task<IEnumerable<IntegrationPriceListItem>> GetPriceListItems()
        {
            var result = new List<IntegrationPriceListItem>();
            var cats = this.GoodCategories.ToArray();
            using (var db = this.Database.GetDbConnection())
            {
                await db.OpenAsync();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM Hajir_VWFee ";
                    var reader = await cmd.ExecuteReaderAsync();
                    while(await reader.ReadAsync())
                    {
                        try
                        {
                            result.Add(ReadPriceListItem(reader));
                        }
                        catch (Exception err)
                        {
                            this.logger.LogWarning(
                                $"An error occured wile trying to read this PriceListItem. Err:{err.Message}");
                        }
                    }

                }
            }

            return result;
        }
    }
}
