﻿using GN;
using GN.Library;
using Hajir.Crm.Entities;
using Hajir.Crm.Features.Products;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AutoMapper.Configuration;
using GN.Library.Xrm;
using Hajir.Crm.Features.Sales;
using Hajir.Crm.Infrastructure.Xrm.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Hajir.Crm.Features.Integration;
using GN.Library.Xrm.StdSolution;

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
            result.ProductType = product.ProductType ?? HajirProductEntity.Schema.ProductTypes.Other;
            result.ProductSeries = product.ProductSerie ?? HajirProductEntity.Schema.ProductSeries.UNKOWN;
            result.UOMId = product.GetAttributeValue<EntityReference>("defaultuomid")?.Id.ToString();
            result.SupportedBattries = product.SupportedBatteries;
            result.NumberOfRows = product.GetNumberIfFloors();
            result.Vendor = CabinetVendors.Hajir;
            result.BatteryPower = int.TryParse(product.BatteryCurrent, out var p) ? p : 0;
            return result;
        }

        public static ProductSeries ToProductSeries(this XrmHajirProductSeries series)
        {
            return new ProductSeries
            {
                Id = series.Id.ToString(),
                Name = series.Name
            };
        }

        public static SaleQuote ToQuote(this XrmHajirQuote q)
        {
            return null;
        }
        public static XrmHajirQuote ToXrmQuote(this SaleQuote q)
        {
            return new XrmHajirQuote
            {
                Id = Guid.TryParse(q.QuoteId, out var _id) ? _id : Guid.Empty,
            };
        }
        public static XrmHajirQuoteDetail ToXrmQuoteDetail(this SaleQuoteLine line)
        {
            var res =
            new XrmHajirQuoteDetail
            {
                ///QuoteDetailId = Guid.TryParse(line.Id, out var _id) ? _id : (Guid?)null,
                ProductId = Guid.TryParse(line.ProductId, out var _pid) ? _pid : (Guid?)null,
                Quantity = Convert.ToDouble(line.Quantity)
            };
            if (Guid.TryParse(line.Id, out var _id))
            {
                res.QuoteDetailId = _id;

            }
            return res;

        }
        public static IntegrationContact ToIntegrationContact(this XrmContact c)
        {
            return c == null ? null : new IntegrationContact
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Id = c.Id.ToString()
            };
        }
        public static IntegrationQuote ToIntegrationQuote(this XrmHajirQuote quote)
        {
            var result = quote.ToDynamic()
                .To<IntegrationQuote>();
            return result;
        }
        public static IntegrationQuoteProduct ToIntegrationQuoteProduct(this XrmHajirQuoteDetail quote)
        {
            var result = quote == null ? null : quote.ToDynamic()
                .To<IntegrationQuoteProduct>();
            return result;
        }
        public static IntegrationAccount ToIntegrationAccount(this XrmHajirAccount c)
        {
            return c == null ? null : new IntegrationAccount
            {
                Name = c.Name,

                Id = c.Id.ToString(),
                ModifiedOn = c.ModifiedOn.Value,
                CreatedOn = c.CreatedOn.Value

            };
        }
        public static XrmHajirAggregateProduct ToXrmAggergateProduct(this SaleAggergateProduct p)
        {
            var res = new XrmHajirAggregateProduct
            {
                PricePerUnit = p.PricePerUint,
                Quantity = p.Quantity,
                ManualDiscount = p.ManualDiscount,
                Name = p.Name,
                //AggregateProductId = Guid.TryParse(p.Id,out var _id)?_id: Guid.Empty
                //Name = p.Id
            };
            if (Guid.TryParse(p.Id, out var _id))
            {
                res.AggregateProductId = _id;
            }
            return res;
        }
    }
}
