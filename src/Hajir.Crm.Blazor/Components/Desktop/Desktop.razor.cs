using Hajir.Crm.Sales;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Components.Desktop
{
    public class SearchResult
    {
        public string Name { get; set; }

    }
    public partial class Desktop
    {
        public string Search { get; set; }

        public List<SearchResult> Results = new List<SearchResult>();
        public SearchResult[,] Matrix = new SearchResult[0, 0];

        public async Task HandleIntervalElapsed(string b)
        {
            var accounts = await this.ServiceProvider.GetService<IQuoteRepository>()
                 .SearchAccount(b);
            Results.Clear();
            Results.AddRange(accounts.Select(x => new SearchResult { Name = x.Name }));

            this.Matrix = FillMatrix(Results, 3);

        }
        static SearchResult[,] FillMatrix(List<SearchResult> items, int columns)
        {
            int rows = (int)Math.Ceiling(items.Count / (double)columns);
            SearchResult[,] matrix = new SearchResult[rows, columns];

            for (int i = 0; i < items.Count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                matrix[row, col] = items[i];
            }

            // Optional: Fill empty cells with null or empty string
            for (int i = items.Count; i < rows * columns; i++)
            {
                int row = i / columns;
                int col = i % columns;
                matrix[row, col] = null;
            }
            return matrix;
        }
    }

}


