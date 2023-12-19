using GN.Library.Xrm.StdSolution;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hajir.Crm.Infrastructure.Xrm.Data
{
	[EntityLogicalName(Schema.LogicalName)]
	public class XrmHajirQuoteDetail : XrmQuoteDetail
	{
		public new class Schema : XrmQuoteDetail.Schema
		{
			public const string SolutionPerfix = HajirCrmConstants.RahsamSolutionPerfix;
			public const string AggregateProductId = SolutionPerfix + "aggregateproducts";
		}

	}

	public static partial class XrmHajirExtensions
	{

		public static IEnumerable<XrmHajirQuoteDetail> GetDetails(this IQueryable<XrmHajirQuoteDetail> query, Guid quoteId)
		{
			return query.Where(x => (Guid)x.QuoteId == quoteId)
				.ToArray();
		}
	}
}
