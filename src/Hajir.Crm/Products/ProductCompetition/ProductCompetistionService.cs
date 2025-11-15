using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Products.ProductCompetition
{

    public interface IProductCompetitionService
    {
        Task<Competitor[]> GetCompetitors();
    }
    internal class ProductCompetistionService : IProductCompetitionService
    {
        private string[] _competitors = new string[] { "hirad", "faratel", "alja", "faran" };
        public async Task<Competitor[]> GetCompetitors()
        {
            await Task.CompletedTask;
            var result = new List<Competitor>();
            foreach(var item in _competitors)
            {
                try
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), $"Products\\ProductCompetition\\Data\\{item}.json");
                    var d = Directory.Exists(Path.GetDirectoryName(fileName));
                    var data = File.ReadAllText(fileName);
                    var pl = Newtonsoft.Json.JsonConvert.DeserializeObject<Competitor.PriceItem[]>(data);
                    result.Add(new Competitor(item, pl));
                }
                catch (Exception ex)
                {

                }
            }


            return result.ToArray();


        }
    }
}
