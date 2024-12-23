using Hajir.Crm.Integration.SanadPardaz.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
        class TokenModel
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }

        }
        private readonly SanadPardazIntegrationOptions options;
        private readonly ILogger<SanadPardazApiClient> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IMemoryCache cache;
        private HttpClient httpClient;

        private T Deserialize<T>(string content) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
        private string Serialize(object obj)=> Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        public SanadPardazApiClient(SanadPardazIntegrationOptions options, ILogger<SanadPardazApiClient> logger,
            IServiceProvider serviceProvider, IMemoryCache cache)
        {
            
            this.options = options;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.cache = cache;
        }
        private HttpClient DoGetHttpClient()
        {
            if (this.httpClient == null)
            {
                this.httpClient = new HttpClient();
                var str = this.options.ApiUrl + "api/v3/";
                this.httpClient.BaseAddress = new Uri(this.options.GetBaseApiAddress());
            }
            return this.httpClient;
        }
        private async Task<TokenModel> _GetToken()
        {
            TokenModel result = null;
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri(this.options.GetBaseApiAddress()) })
                {

                    using (var multipartContent = new MultipartFormDataContent())
                    {

                        // Add string fields
                        multipartContent.Add(new StringContent(this.options.UserName), "UserName");
                        multipartContent.Add(new StringContent(this.options.Password), "Password");

                        HttpResponseMessage response = await client.PostAsync("Users/Token", multipartContent);
                        response.EnsureSuccessStatusCode();

                        string responseContent = await response.Content.ReadAsStringAsync();
                        result = Deserialize<TokenModel>(responseContent);

                    }


                }
            }
            catch (Exception ex)
            {

                this.logger.LogError(
                    $"An error occured while trying to GetToken. Err:{ex.GetBaseException().Message}");
            }
            return result;
        }
        private Task<HttpClient> GetHttpClient(bool refresh=false)
        {
            if (refresh)
            {
                this.cache.Remove("SANADPARDAZ_HTTPCLIENT");
            }
            return this.cache.GetOrCreateAsync<HttpClient>("SANADPARDAZ_HTTPCLIENT", async entry =>
            {

                entry.RegisterPostEvictionCallback((a, b, c, d) =>
                {

                    if (b is HttpClient _c)
                    {
                        _c.Dispose();
                    }

                });
                var token = await this._GetToken();
                var result = new HttpClient()
                {
                    BaseAddress = new Uri(this.options.GetBaseApiAddress()),
                };
                if (token == null)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                    return result;
                }
                else
                {
                    result.DefaultRequestHeaders.Authorization =
                         new AuthenticationHeaderValue("Bearer", token.access_token);
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.expires_in / 2);
                    return result;
                }

            });
        }
        public void Dispose()
        {
            this.httpClient?.Dispose();

        }

        public async Task<GoodGetModel> GetGoods(int pageNumber = 1, int pageSize = 10)
        {
            await this.GetHttpClient(false);
            var response = await this.DoGetHttpClient().GetAsync($"good?OrderBy=actiondate&Direction=desc&PageNumber={pageNumber}&PageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GoodGetModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task Test()
        {
            var client = this.DoGetHttpClient();
            var response = await client.GetAsync("good");
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<GoodGetModel>(res);


            //throw new NotImplementedException();
        }

        private async Task<Res> PostJsonAsync<Res>(HttpClient client,  string api, object req)
        {
            var response = await client.PostAsync(api, new StringContent(Serialize(req), Encoding.UTF8, "application/json"));
            var fff = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return this.Deserialize<Res>(await response.Content.ReadAsStringAsync());

        }
        public async Task<DetailGetModel> GetDetails(int pageNumber = 1, int pageSize = 10)
        {

            var client = await this.GetHttpClient();

            var request = new SanadRequestModel<GetDetailRequestModel>
            {
                configId = 1,
                input = new GetDetailRequestModel { pageNumber = 1, pageSize = 20 }
            };
            var rr = await PostJsonAsync<GetDetialResponseModel>(client, "Request", request);



            var response = await this.DoGetHttpClient()
                .GetAsync($"Detail?OrderBy=actiondate&Direction=desc&PageNumber={pageNumber}&PageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DetailGetModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<DetailGetByCodeModel> GetDetailByCode(int code)
        {
            var response = await this.DoGetHttpClient()
                .GetAsync($"Detail/{code}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DetailGetByCodeModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
