using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hajir.Crm.Sales
{

    public class QuoteEditModel
    {
        public class QuoteLine
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal? Price { get; set; }
            public decimal? Discount { get; set; }
            public decimal? PercentDiscount { get; set; }
            public decimal? Amount { get; set; }
            public int RowNumber { get; set; }

            public QuoteLine Recalculate()
            {
                
                this.Amount = this.Price * this.Quantity;
                var discount= (this.Discount ?? 0) + (this.PercentDiscount ?? 0) * this.Amount / 100;
                this.Amount = this.Amount - discount;


                return this;
            }

        }
        public Guid Id { get; set; }
        public string Remarks { get; set; }
        public QuoteLine[] Lines { get; set; }

        public QuoteEditModel Recalculate()
        {
            var rowNumber = 1;
            foreach (var l in this.Lines.OrderBy(x => x.RowNumber))
            {
                l.RowNumber = rowNumber;
                rowNumber++;
            }
            return this;
        }
        public QuoteEditModel AddLine(int rowNumber = int.MaxValue, QuoteLine line = null)
        {
            var lst = this.Lines.ToList();
            line = line ?? new QuoteLine();
            if (rowNumber > lst.Count)
            {
                lst.Add(line);
            }
            else
            {
                if (rowNumber < 0)
                    rowNumber = 0;
                lst.Insert(rowNumber, line);
            }
            this.Lines = lst.ToArray();
            this.Recalculate();

            return this;
        }
    }
}
