using GN.Library.AI.Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.AI.Agents.ProductScaper
{
    internal class ProductScaperAgent : BaseAgent
    {
        public ProductScaperAgent(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override AgentOptions Options => new AgentOptions
        {
            Name = "scraper",
            PythonPath = "ProductScraper\\py"
        };
    }
}
