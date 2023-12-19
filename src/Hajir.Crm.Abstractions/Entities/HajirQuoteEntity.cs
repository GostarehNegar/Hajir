using GN.Library.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Entities
{
    public class HajirQuoteEntity : DynamicEntity
    {
		public new class Schema
		{
			public const string LogicalName = "quote";
			public const string QuoteId = "quoteid";
			public const string Name = "name";
			public const string QuoteNumber = "quotenumber";
			public const string CustomerId = "customerid";
			public const string PriceLevelId = "pricelevelid";
			public const string TotalAmount = "totalamount";
			public const string TotalTax = "totaltax";


			// https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/quote#BKMK_StateCode
			public enum QuoteStateCodes
			{
				Draft = 0,
				Active = 1,
				Won = 2,
				Closed = 3

			}
			public enum QuoteStatusCodes
			{
				/// <summary>
				/// InProgress when state = Draft
				/// </summary>
				InProgressDraft = 1,
				/// <summary>
				/// In progress when state = Active
				/// </summary>
				InProgressActive = 2,
				/// <summary>
				/// Open when sate = active
				/// </summary>
				Open = 3,
				/// <summary>
				/// State = Won
				/// </summary>
				Won = 4,
				/// <summary>
				/// State = Closed
				/// </summary>
				Lost = 5,
				/// <summary>
				/// State = Closed
				/// </summary>
				Canceled = 6,
				/// <summary>
				/// State = Closed
				/// </summary>
				Reised = 7


			}

			public class ColumnSelectors : XrmColumnSelector<XrmQuote>
			{
				public static Expression<Func<XrmQuote, XrmQuote>> Default => x => new XrmQuote
				{
					Name = x.Name,
					Id = x.Id,
					StatusCode = x.StatusCode,
					StateCode = x.StateCode
				};
				public static Expression<Func<XrmQuote, XrmQuote>> All => x => x;
				public static Expression<Func<XrmQuote, XrmQuote>> Min => x => new XrmQuote
				{
					Id = x.Id
				};



			}



		}
	}
}
