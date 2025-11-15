using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Agents.Tools
{
    public static class ToolsExtensions
    {
        public static IServiceCollection AddSerarchContactTool(this IServiceCollection services, SearchContactToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<SearchContactToolService>();

        }
        public static IServiceCollection AddRegisterPhoneCallTool(this IServiceCollection services, RegisterPhoneCallToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<RegisterPhoneCallTool>();

        }
        public static IServiceCollection AddCreateOpportunityCallTool(this IServiceCollection services, CreateOpportunityToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<CreateOpportunityTool>();

        }
        public static IServiceCollection AddListOpportunityTool(this IServiceCollection services, ListOpportunityToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<ListOpportunityTool>();

        }
        public static IServiceCollection AddSearchProductTool(this IServiceCollection services, SearchProdcutsToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<SearchProdcutsTool>();

        }
        public static IServiceCollection AddSearchAccountTool(this IServiceCollection services, AccountSearchToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<AccountSearchTool>();

        }
        public static IServiceCollection AddAccountInformationTool(this IServiceCollection services, AccountInformationToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<AccountInformationTool>();

        }
        public static IServiceCollection AddPriceCaomparisonTool(this IServiceCollection services, ProductPriceComparisonToolOptions options)
        {
            return services.AddSingleton(options)
                .AddHostedService<ProductPriceComparisonTool>();

        }
    }
}
