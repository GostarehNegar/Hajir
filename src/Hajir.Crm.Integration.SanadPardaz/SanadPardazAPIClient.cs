using Hajir.Crm.Integration.Infrastructure;
using Hajir.Crm.Integration.SanadPardaz.Models;
using Hajir.Crm.SanadPardaz;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration.SanadPardaz
{

    class SanadPardazApiClient : ISanadApiClientService// ISanadPardazApiClient
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
        private T Deserialize<T>(string content) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
        private string Serialize(object obj) => Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        public SanadPardazApiClient(SanadPardazIntegrationOptions options, ILogger<SanadPardazApiClient> logger,
            IServiceProvider serviceProvider, IMemoryCache cache)
        {

            this.options = options;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.cache = cache;
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
        private Task<HttpClient> GetHttpClient(bool refresh = false)
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
            //this.httpClient?.Dispose();

        }




        private async Task<TRep[]> RequestEx<TReq, TRep>(int requestId, TReq req) where TReq : class, new()
        {
            try
            {
                var client = await this.GetHttpClient();
                var response = await PostJsonAsync<SanadApiResponseModel<TRep>>(client,
                    "Request",
                    new SanadApiRequestModel<TReq>
                    {
                        configId = requestId,
                        input = req
                    });
                if (!response.isSuccess)
                {
                    throw new Exception(
                        $"Unsuccessfull request. Message:{response.message}");
                }
                return response.data.result;
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to execute a SanadPardaz request. configId:{requestId} " +
                    $"Err:{err.GetBaseException().Message}");
                throw;


            }

        }
        private async Task<TRep[]> Request<TReq, TRep>(int requestId, Action<TReq> configure) where TReq : class, new()
        {
            var req = new TReq();
            configure?.Invoke(req);
            try
            {
                var client = await this.GetHttpClient();
                var response = await PostJsonAsync<SanadApiResponseModel<TRep>>(client,
                    "Request",
                    new SanadApiRequestModel<TReq>
                    {
                        configId = requestId,
                        input = req
                    });
                if (!response.isSuccess)
                {
                    throw new Exception(
                        $"Unsuccessfull request. Message:{response.message}");
                }
                return response.data.result;
            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to execute a SanadPardaz request. configId:{requestId} " +
                    $"Err:{err.GetBaseException().Message}");
                throw;


            }

        }
        private async Task<Res> PostJsonAsync<Res>(HttpClient client, string api, object req)
        {
            var response = await client.PostAsync(api, new StringContent(Serialize(req), Encoding.UTF8, "application/json"));
            var fff = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return this.Deserialize<Res>(await response.Content.ReadAsStringAsync());
        }
        public async Task<DetailGetModel> GetDetails(int pageNumber = 1, int pageSize = 10)
        {

            var client = await this.GetHttpClient();

            var request = new SanadApiRequestModel<GetDetailRequestModel>
            {
                configId = 1,
                input = new GetDetailRequestModel { pageNumber = 1, pageSize = 20 }
            };
            var rr = await PostJsonAsync<GetDetialResponseModel>(client, "Request", request);



            var response = await (await this.GetHttpClient())
                .GetAsync($"Detail?OrderBy=actiondate&Direction=desc&PageNumber={pageNumber}&PageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DetailGetModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<DetailGetByCodeModel> GetDetailByCode(int code)
        {
            var response = await (await this.GetHttpClient())
                .GetAsync($"Detail/{code}");
            response.EnsureSuccessStatusCode();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DetailGetByCodeModel>(await response.Content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<SanadPardazGoodModel>> GetGoods(int page = 1, int pageLength = 100)
        {
            SanadPardazGoodModel fix(SanadPardazGoodModel goodModel)
            {
                if (goodModel != null)
                {
                    goodModel.GoodName = goodModel.GoodName?.Replace("ك", "ک").Replace("ي", "ی");
                }
                return goodModel;
            }
            var res = await Request<GetGoodRequestModel, SanadPardazGoodModel>(6, cfg =>
            {

                cfg.pageNumber = page < 1 ? 1 : page;
                cfg.pageSize = pageLength;
                cfg.orderBy = "ActionDate";
                cfg.orderDirection = "desc";
            });
            return res.Select(x => fix(x)); ;
        }

        public async Task<IEnumerable<SanadPardazDetialModel>> GetDetials(int page = 1, int pageLength = 500)
        {
            var res = await Request<GetDetailRequestModel, SanadPardazDetialModel>(1, cfg =>
            {

                cfg.pageNumber = page < 1 ? 1 : page;
                cfg.pageSize = pageLength;
                cfg.orderBy = "ActionDate";
                cfg.orderDirection = "desc";
            });
            return res; ;
        }

        public async Task<InsertOrderResponse> InsertOrder(InsertOrderRequest request)
        {
            var req = request ?? new InsertOrderRequest
            {
                typeNo = 100,

                financYear = "1403",
                rdBaseNo = 0,
                companyId = 1,
                strDate = "1403/09/07",
                detailCode1 = 1008931,
                detailCode2 = 994000,
                detailCode3 = 0,
                detailCode4 = 0,
                detailCode5 = 2,
                userCode = 34,
                sysCode = 1,
                description = "184000000",
                strCreditDate = "1403/09/10",
                desc1 = "اسلامشهر-شهرک امام حسین",
                desc2 = "30",
                desc3 = "02156558093",
                desc4 = "اسلامشهر",
                items = new InsertOrderRequest.OrderRow[]
                {
                    new InsertOrderRequest.OrderRow
                    {
                        itemRow= 1,
                        goodCode= "40107200002",
                        amount= 2,
                        itemNumber1= 12,
                        fee= 82800000,
                        rowDesc= "دستگاه باتری داخلی است",
                        itemDesc4= "دستگاه no name ارسال گردد",
                        batchNo= 0,
                        batchNo2= 0,
                        batchNo3= 0,
                        batchNo4= 0,
                        batchNo5= 0,
                        batchNo6= 0,
                        vat1Price= 16560000,
                        itemDetailCode1= 0,
                        itemDetailCode2= 0,
                        discountPer= 0,
                        discountPrice= 0,
                    }
                },
                addReds = new InsertOrderRequest.AddReds[]
                {
                    new InsertOrderRequest.AddReds
                    {
                        addRedTypeId= 256,
                        rowId= 1,
                        per= 0,
                        price= 20560000,
                        addOrRed= 1

                    }
                }

            };
            var res = await this.RequestEx<InsertOrderRequest, InsertOrderResponse>(8, req);
            return res.FirstOrDefault();


        }

        public Task<IEnumerable<SanadPardazDetialModel>> GetCachedDetails()
        {
            return this.cache.GetOrCreateAsync<IEnumerable<SanadPardazDetialModel>>("_SANAD_DETAILS_CACHE", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.options.DetailCacheLifetime);
                await Task.CompletedTask;
                var result = new List<SanadPardazDetialModel>();
                try
                {
                    var page = 1;
                    using (var scope = this.serviceProvider.CreateScope())
                    {
                        var client = scope.ServiceProvider.GetService<ISanadApiClientService>();

                        while (true)
                        {
                            var items = await client.GetDetials(page, 1000);
                            result.AddRange(items);
                            page++;
                            if (items.Count() < 1000)
                                break;
                        }
                    }
                    this.logger.LogInformation(
                        $"{result.Count} Accounts (Details) Successfuly Cached from SanadPardaz.");
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                        $"An error occured while trying to cache Sanad Detail Codes. Err:{ex.GetBaseException().Message}");

                }
                return result;

            });
        }
        public Task<IEnumerable<SanadPardazGoodModel>> GetCachedGoods()
        {
            return this.cache.GetOrCreateAsync<IEnumerable<SanadPardazGoodModel>>("_SANAD_GOODS_CACHE", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.options.DetailCacheLifetime);
                await Task.CompletedTask;
                var result = new List<SanadPardazGoodModel>();
                try
                {
                    var page = 1;

                    while (true)
                    {
                        var items = await this.GetGoods(page, 1000);
                        result.AddRange(items);
                        page++;
                        if (items.Count() < 1000)
                            break;
                    }

                    this.logger.LogInformation(
                        $"{result.Count} Goods Successfuly Cached from SanadPardaz.");
                }
                catch (Exception ex)
                {
                    this.logger.LogError(
                        $"An error occured while trying to cache Sanad Goods. Err:{ex.GetBaseException().Message}");

                }
                return result;

            });
        }
        public async Task<SanadPardazDetialModel> FindDetailByNationalId(string nationalId)
        {
            return (await this.GetCachedDetails())
                .FirstOrDefault(x => x.NationalId == nationalId);

        }

        public async Task<SanadPardazDetialModel> FindDetailByEconomicCode(string economiCode)
        {
            return (await this.GetCachedDetails())
               .FirstOrDefault(x => x.EconomicCode == economiCode);
        }

        public async Task<SanadPardazDetialModel> FindDetailByRegistrationNo(string regNo)
        {
            return (await this.GetCachedDetails())
               .FirstOrDefault(x => x.RegNo == regNo);
        }

        public async Task<SanadPardazGoodModel> GetCachedGoodByCode(string code)
        {
            return (await this.GetCachedGoods())
                .FirstOrDefault(x => x.GoodCode == code);

        }

        public Task<InsertOrderResponse> InsertOrder(Action<InsertOrderRequest> configure)
        {
            //var request = new InsertOrderRequest();
            var request =  new InsertOrderRequest
            {
                typeNo = 100,

                financYear = "1403",
                rdBaseNo = 0,
                companyId = 1,
                strDate = "1403/09/07",
                detailCode1 = 1008931,
                detailCode2 = 994000,
                detailCode3 = 0,
                detailCode4 = 0,
                detailCode5 = 2,
                userCode = 34,
                sysCode = 1,
                description = "184000000",
                strCreditDate = "1403/09/10",
                desc1 = "اسلامشهر-شهرک امام حسین",
                desc2 = "30",
                desc3 = "02156558093",
                desc4 = "اسلامشهر",
                items = new InsertOrderRequest.OrderRow[]
    {
                    new InsertOrderRequest.OrderRow
                    {
                        itemRow= 1,
                        goodCode= "40107200002",
                        amount= 2,
                        itemNumber1= 12,
                        fee= 82800000,
                        rowDesc= "دستگاه باتری داخلی است",
                        itemDesc4= "دستگاه no name ارسال گردد",
                        batchNo= 0,
                        batchNo2= 0,
                        batchNo3= 0,
                        batchNo4= 0,
                        batchNo5= 0,
                        batchNo6= 0,
                        vat1Price= 16560000,
                        itemDetailCode1= 0,
                        itemDetailCode2= 0,
                        discountPer= 0,
                        discountPrice= 0,
                    }
    },
                addReds = new InsertOrderRequest.AddReds[]
    {
                    new InsertOrderRequest.AddReds
                    {
                        addRedTypeId= 256,
                        rowId= 1,
                        per= 0,
                        price= 20560000,
                        addOrRed= 1

                    }
    }

            };

            configure?.Invoke(request);
            return this.InsertOrder(request);
        }
    }
}
