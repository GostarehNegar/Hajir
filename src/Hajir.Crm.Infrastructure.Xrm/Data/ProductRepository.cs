using GN.Library.Xrm;
using Hajir.Crm.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hajir.Crm.Features.Products;


namespace Hajir.Crm.Infrastructure.Xrm.Data
{

	class XrmProductRepository : IProductRepository
	{
		private readonly IXrmDataServices dataServices;
		private readonly ILogger<XrmProductRepository> logger;

		public XrmProductRepository(IXrmDataServices dataServices, ILogger<XrmProductRepository> logger)
		{
			this.dataServices = dataServices;
			this.logger = logger;
		}
		public XrmHajirProduct GetXrmProcuct(string id)
		{
			return !Guid.TryParse(id, out var _id)
				? null
				: dataServices.GetRepository<XrmHajirProduct>()
					.Queryable
					.Where(x => x.ProductId == _id)
					.FirstOrDefault();
		}
		public HajirProductEntity GetProcuct(string id)
		{
			return !Guid.TryParse(id, out var _id)
				? null
				: dataServices.GetRepository<XrmHajirProduct>()
					.Queryable
					.Where(x => x.ProductId == _id)
					.FirstOrDefault()?
					.ToDynamic()
					.To<HajirProductEntity>();
		}

		public Product GetProductById(string id)
		{
			return GetXrmProcuct(id)?.ToProduct();

		}

        public IEnumerable<Product> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
