using GN;
using GN.Library.Shared.Entities;
using Hajir.Crm.Common;
using Hajir.Crm.Internals;
using Hajir.Crm.Products;
using Hajir.Crm.Sales;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static PriceList GetPriceList(this ICacheService cacheService, int no=1)
        {
            
            return cacheService.PriceLists.FirstOrDefault(x => x.Name == HajirCrmConstants.GetPriceListName(no));
        }
        public static Product GetProductById(this ICacheService cacheService, string productId)
        {
            return cacheService.Products.FirstOrDefault(x => x.Id == productId);
        }
    }
}
