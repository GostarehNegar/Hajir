using Hajir.Crm.Integration.SanadPardaz.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.SanadPardaz
{
    public interface ISanadPardazApiClient : IDisposable
    {
        Task Test();
        Task<GoodGetModel> GetGoods(int pageNumber = 1, int pageSize = 10);
        Task<DetailGetModel> GetDetails(int pageNumber = 1, int pageSize = 10);
        Task<DetailGetByCodeModel> GetDetailByCode(int code);
    }
    class SanadPardazApiClient : ISanadPardazApiClient
    {
        private readonly SanadPardazIntegrationOptions options;
        private readonly ILogger<SanadPardazApiClient> logger;
        private HttpClient httpClient;

        public SanadPardazApiClient(SanadPardazIntegrationOptions options, ILogger<SanadPardazApiClient> logger)
        {
            this.options = options;
            this.logger = logger;

        }
        private HttpClient GetHttpClient()
        {
            if (this.httpClient == null)
            {
                this.httpClient = new HttpClient();
                var str = this.options.ApiUrl + "api/v2/";
                this.httpClient.BaseAddress = new Uri(this.options.GetBaseApiAddress());
            }
            return this.httpClient;
        }

        public void Dispose()
        {
            this.httpClient?.Dispose();

        }

        public async Task<GoodGetModel> GetGoods(int pageNumber = 1, int pageSize = 10)
        {
            var response = await this.GetHttpClient().GetAsync($"good?OrderBy=actiondate&Direction=desc&PageNumber={pageNumber}&PageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GoodGetModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task Test()
        {
            var client = this.GetHttpClient();
            var response = await client.GetAsync("good");
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<GoodGetModel>(res);


            //throw new NotImplementedException();
        }

        public async Task<DetailGetModel> GetDetails(int pageNumber = 1, int pageSize = 10)
        {
            var response = await this.GetHttpClient()
                .GetAsync($"Detail?OrderBy=actiondate&Direction=desc&PageNumber={pageNumber}&PageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DetailGetModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<DetailGetByCodeModel> GetDetailByCode(int code)
        {
            var response = await this.GetHttpClient()
                .GetAsync($"Detail/{code}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DetailGetByCodeModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
