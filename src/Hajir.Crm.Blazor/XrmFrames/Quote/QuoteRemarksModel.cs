using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.XrmFrames.Quote
{
    public class QuoteRemarkLine
    {
        public string Left { get; set; }
        public bool Selected { get; set; }
        public bool HasInput { get; set; }
        public string Input { get; set; }

        public string Right { get; set; }
    }
    public class QuoteRemarksModel
    {
        public string Text { get; set; }
        public QuoteRemarkLine[] Lines { get; set; }
        public static QuoteRemarksModel Default => new QuoteRemarksModel
        {
            Lines = new QuoteRemarkLine[]
            {
                new QuoteRemarkLine{Left="دارای ", HasInput=true, Right="ماه گارانتی است"},
                new QuoteRemarkLine{Left="Text2"},
            }
        };
    }
}
