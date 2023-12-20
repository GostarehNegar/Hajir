using GN.Library.Xrm;
using GN.Library.Xrm.StdSolution;
using Hajir.Crm.Features.Sales;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Tests.Specs.Infrastructure
{
	[TestClass]
	public class QuoteRepository_Specs:TestFixture
	{
		[TestMethod]
		public async Task can_load_quote()
		{
			var host = this.GetHost();
			var quote_id = host.Services
				.GetService<IXrmDataServices>()
				.GetRepository<XrmQuote>()
				.Queryable
				.Take(5)
				.ToArray()
				.FirstOrDefault()
				.QuoteId;

			var target = host.Services
				.GetService<IQuoteRepository>();

			var quote = target.LoadQuote(quote_id.ToString());

		}
	}
}
