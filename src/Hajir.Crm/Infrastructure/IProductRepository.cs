using GN.Library.Xrm;
using Hajir.Crm.Abstractions.Entities;
using Hajir.Crm.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hajir.Crm.Infrastructure
{
    public interface IProductRepository
    {
        HajirProductEntity GetProcuct(string id);
    }
    class XrmProductRepository : IProductRepository
    {
        private readonly IXrmDataServices dataServices;
        private readonly ILogger<XrmProductRepository> logger;

        public XrmProductRepository(IXrmDataServices dataServices, ILogger<XrmProductRepository> logger)
        {
            this.dataServices = dataServices;
            this.logger = logger;
        }
        public HajirProductEntity GetProcuct(string id)
        {
            return !Guid.TryParse(id, out var _id)
                ? null
                : this.dataServices.GetRepository<XrmHajirProduct>()
                    .Queryable
                    .Where(x => x.ProductId == _id)
                    .FirstOrDefault()?
                    .ToDynamic()
                    .To<HajirProductEntity>();
        }
    }
}
