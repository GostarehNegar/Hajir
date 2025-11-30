using GN;
using GN.Library.Shared.Entities;
using Hajir.Crm.Common;
using Hajir.Crm.Internals;
using Hajir.Crm.Products;
using Hajir.Crm.Products.ProductCompetition;
using Hajir.Crm.Sales;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm
{
    public static partial class HajirCrmExtensions
    {
        public static IHajirCrmServiceContext CreateHajirServiceContext(this IServiceProvider serviceProvider)
        {

            return new HajirCrmServiceContext(serviceProvider);
        }

        internal static int CalcLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }

            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;

                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[lengthA, lengthB];
        }

        public static string NumberToString(decimal number)
        {
            return PersianNumberToTextConverter.ConvertToText(number);
        }
        public static Guid? GetOwnerId(this DynamicEntity entity)
        {
            if (entity == null)
                return null;
            return entity.GetAttributeValue<DynamicEntityReference>("ownerid")?.Id != null &&
                Guid.TryParse(entity.GetAttributeValue<DynamicEntityReference>("ownerid")?.Id, out var _res)
                ? _res
                : (Guid?)null;

        }
        public static DynamicEntityReference GetOwnerReference(this DynamicEntity entity)
        {
            if (entity == null)
                return null;
            return entity.GetAttributeValue<DynamicEntityReference>("ownerid");

        }
        public static DynamicEntityReference GetCreatedByReference(this DynamicEntity entity)
        {
            if (entity == null)
                return null;
            return entity.GetAttributeValue<DynamicEntityReference>("createdby");

        }
        public static Guid? GetCreatedBy(this DynamicEntity entity)
        {
            if (entity == null)
                return null;
            return entity.GetAttributeValue<DynamicEntityReference>("createdby")?.Id != null &&
                Guid.TryParse(entity.GetAttributeValue<DynamicEntityReference>("createdby")?.Id, out var _res)
                ? _res
                : (Guid?)null;

        }
        public static DynamicEntityReference GetModifiedByReference(this DynamicEntity entity)
        {
            if (entity == null)
                return null;
            return entity.GetAttributeValue<DynamicEntityReference>("modifiedby");

        }
        public static Guid? GetModifiedBy(this DynamicEntity entity)
        {
            if (entity == null)
                return null;
            return entity.GetAttributeValue<DynamicEntityReference>("modifiedby")?.Id != null &&
                Guid.TryParse(entity.GetAttributeValue<DynamicEntityReference>("modifiedby")?.Id, out var _res)
                ? _res
                : (Guid?)null;

        }
        public static PriceList GetPriceList(this ICacheService cacheService, int no = 1)
        {

            return cacheService.PriceLists.FirstOrDefault(x => x.Name == HajirCrmConstants.GetPriceListName(no));
        }
        public static Product GetProductById(this ICacheService cacheService, string productId)
        {
            return cacheService.Products.FirstOrDefault(x => x.Id == productId);
        }
        public static Product GetProductByProductNumber(this ICacheService cacheService, string productNumber)
        {
            return cacheService.Products.FirstOrDefault(x => x.ProductNumber == productNumber);
        }
        public static Competitor[] GetCompetitors()
        {

            var result = new List<Competitor>();
            string[] _competitors = new string[] { "hirad", "faratel", "alja", "faran" };
            foreach (var item in _competitors)
            {
                try
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(typeof(HajirCrmExtensions).Assembly.Location), $"Products\\ProductCompetition\\Data\\{item}.json");
                    var d = Directory.Exists(Path.GetDirectoryName(fileName));
                    var data = File.ReadAllText(fileName);
                    var pl = Newtonsoft.Json.JsonConvert.DeserializeObject<Competitor.PriceItem[]>(data);
                    pl.ToList().ForEach(x => x.Manufacturer = item);
                    result.Add(new Competitor(item, pl));
                }
                catch (Exception ex)
                {

                }
            }


            return result.ToArray();


        }
        public static Competitor[] GetCompetitors(this ICacheService cache)
        {
            return cache.Cache.GetOrCreate<Competitor[]>("_COMPETETORS_", ent =>
            {
                ent.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                return GetCompetitors();
            });

        }
        

    }
}
