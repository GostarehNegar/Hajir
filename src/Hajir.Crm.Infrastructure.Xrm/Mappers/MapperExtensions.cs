using GN;
using GN.Library;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AutoMapper.Configuration;
using GN.Library.Xrm;

namespace Hajir.Crm.Infrastructure.Xrm
{
	public static class MapperExtensions
	{

		private static MapperConfigurationExpression mappings = new MapperConfigurationExpression() { CreateMissingTypeMaps = true, };
		private static MapperConfiguration configuration;
		private static IMapper mapper;
		static MapperExtensions()
		{
			var config =
			mapper = new Mapper(new MapperConfiguration(mappings)) as IMapper;
		}
		public static T2 Mapper<T1, T2>(T1 input) => mapper.Map<T1, T2>(input);

		public static Product ToProduct(this XrmHajirProduct product)
		{

			var result = product.ToDynamic().To<Product>();
			result.ProductType = product.ProductType;
			return result;

			return mapper.Map<XrmHajirProduct, Product>(product, opt =>
			{
				opt.ConfigureMap().ForAllMembers(o => o.Ignore());
			});
		}
	}
}
