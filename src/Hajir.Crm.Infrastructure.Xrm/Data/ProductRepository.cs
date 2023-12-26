using GN.Library.Xrm;
using Hajir.Crm.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hajir.Crm.Features.Products;
using Microsoft.AspNetCore.Mvc;


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
			XrmHajirProduct Fix(XrmHajirProduct item)
			{
				if (!item.ProductType.HasValue &&  item.Name.StartsWith("UPS"))
				{
					item.ProductType = HajirProductEntity.Schema.ProductTypes.UPS;
				}
				if (!item.ProductType.HasValue && item.Name.StartsWith("کابينت"))
				{
					item.ProductType = HajirProductEntity.Schema.ProductTypes.Cabinet;
				}
				///
				///
				if (!item.ProductType.HasValue && item.Name.StartsWith("باتري"))
				{
					item.ProductType = HajirProductEntity.Schema.ProductTypes.Battery;
				}

				return item;
			}
			var result = new List<Product>();
			var repo = this.dataServices
					.GetRepository<XrmHajirProduct>();

			var types = this.dataServices
				.GetRepository<XrmHajirTypeProduct>()
				.Queryable
				.ToArray();


			var fin = false;
			var take = 300;
			var skip = 0;
			while (!fin)
			{
				var items = repo.Queryable.SKIP(skip).Take(take).ToArray();
				result.AddRange(items.Select(x => Fix(x).ToProduct()));
				fin = items.Length < take;
				skip += take;
			}
			return result;
		}
	}
}

