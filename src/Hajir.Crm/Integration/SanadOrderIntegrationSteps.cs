using GN.Library.Functional.Pipelines;
using Hajir.Crm.Integration.Infrastructure;
using MassTransit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration
{
    internal static class SanadOrderIntegrationSteps
    {
        public static async Task<SanadOrderIntegrationContext> LoadQuote(SanadOrderIntegrationContext ctx)
        {
            var quote = ctx.ServiceProvider.GetService<IIntegrationStore>()
               .LoadQuoteById(ctx.QuoteId);
            if (quote == null)
            {
                throw new Exception($"Quote Not Found. Id:{ctx.QuoteId}");
            }
            ctx.Quote(quote);
            return ctx;
            
        }
        public static async Task<SanadOrderIntegrationContext> LoadAccout(SanadOrderIntegrationContext ctx)
        {
            var account = ctx.Store
               .LoadAccountById(ctx.Quote().AccountId);
            if (account == null)
            {
                throw new Exception($"Account Not Found. Id:{ctx.Quote().AccountId}");
            }
            ctx.Account(account);
            return ctx;
        }
        public static async Task<SanadOrderIntegrationContext> LoadDetail(SanadOrderIntegrationContext ctx)
        {
            var detail = await ctx.SanadApi.FindDetailByNationalId(ctx.Account().NationalId);
            if (detail == null)
            {
                throw new Exception(
                    $"SandPardaz Detail Not Found. NationalId:{ctx.Account().NationalId} ");
            }
            ctx.Detail(detail);
            ctx.Request.WithCustomerDetailCode(detail.DetailAccCode);
            return ctx;
        }
        public static async Task<SanadOrderIntegrationContext> LoadProcucts(SanadOrderIntegrationContext ctx)
        {
            foreach(var p in ctx.Quote().Products)
            {
                var id = p.ProcuctId;
                var _p = ctx.ServiceProvider.GetService<IProductIntegrationStore>().LoadProductById(p.ProcuctId);
                //p.Product(_p);
                var no = _p.ProductNumber;
                var goods = await ctx.SanadApi.GetCachedGoods();
                var g = await ctx.SanadApi.GetCachedGoodByCode(_p.ProductNumber);
                p.SetAttributeValue("sand_procuct_code", g.GoodCode);
                p.ProductNumber = g.GoodCode;
            }
            return ctx;
        }
        public static async Task<SanadOrderIntegrationContext> InsertOrder(SanadOrderIntegrationContext ctx)
        {
            
            var order = await ctx.SanadApi.InsertOrder(req => {


                //req.detailCode1 = ctx.Detail().DetailAccCode;
            
            });
            
            return ctx;
        }
    }
}
