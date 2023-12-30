using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hajir.Crm;
using Hajir.Crm.Features.Sales;

namespace Hajir.Crm.Blazor.Components
{
	public partial class QuoteComponent
	{
		[Parameter]
		public string Id { get; set; }

		[Inject]
		public IServiceProvider ServiceProvider { get; set; }

		protected override Task OnParametersSetAsync()
		{
			using (var ctx = this.ServiceProvider.CreateHajirServiceContext())
			{
				var quote = ctx.LoadQuoteByQuoteNumber(this.Id);
			}
			return base.OnParametersSetAsync();
		}
	}
}
