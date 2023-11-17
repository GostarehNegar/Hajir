using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Linq;

namespace GN.Library.Xrm.Query.Internal
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/ef/core/modeling/dynamic-model
    /// </summary>
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {

        public object Create(DbContext context)
        {
           
            var result= context is IHaveTypes types
                ? (context.GetType(), string.Join(",",types.Types.OrderBy(x=>x.Name).Select(x=>x.Name)))
                : (object)context.GetType();
            return result;
        }
    }

}
