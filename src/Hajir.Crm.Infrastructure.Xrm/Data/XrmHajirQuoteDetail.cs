using GN.Library.Xrm.StdSolution;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xrm.Sdk;
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

		[AttributeLogicalName(Schema.AggregateProductId)]
		public EntityReference AggregateProduct
		{
			get => this.GetAttributeValue<EntityReference>(Schema.AggregateProductId);
			set => this.SetAttribiuteValue(Schema.AggregateProductId, value);
		}
		[AttributeLogicalName(Schema.AggregateProductId)]
		public Guid? AggregateProductId
		{
			get => this.AggregateProduct?.Id;
			set => this.AggregateProduct = value.HasValue ? new EntityReference(XrmHajirAggregateProduct.Schema.LogicalName, value.Value) : null;
		}
	}

	public static partial class XrmHajirExtensions
	{

		public static IEnumerable<XrmHajirQuoteDetail> GetDetails(this IQueryable<XrmHajirQuoteDetail> query, Guid quoteId)
		{
			return query.Where(x => (Guid)x.QuoteId == quoteId)
				.ToArray();
		}
		public static IEnumerable<XrmHajirQuoteDetail> GetAggregateDetails(this IQueryable<XrmHajirQuoteDetail> query, Guid aggregateId)
		{
			return query.Where(x => (Guid)x.AggregateProductId == aggregateId)
				.ToArray();
		}
	}
}
