using Hajir.Crm.SanadPardaz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Integration
{
    public interface ISanadApiClientService
    {
        Task<IEnumerable<SanadPardazGoodModel>> GetGoods(int page = 1, int pageLength = 100);
        Task<IEnumerable<SanadPardazDetialModel>> GetDetials(int page = 1, int pageLength = 500);
        Task<InsertOrderResponse> InsertOrder(InsertOrderRequest request);
        Task<InsertOrderResponse> InsertOrder(Action<InsertOrderRequest> configure);
        Task<IEnumerable<SanadPardazDetialModel>> GetCachedDetails();
        Task<IEnumerable<SanadPardazGoodModel>> GetCachedGoods();
        Task<SanadPardazGoodModel> GetCachedGoodByCode(string code);
        Task<SanadPardazDetialModel> FindDetailByNationalId(string nationalId);
        Task<SanadPardazDetialModel> FindDetailByEconomicCode (string economiCode);
        Task<SanadPardazDetialModel> FindDetailByRegistrationNo(string regNo);

    }
}
