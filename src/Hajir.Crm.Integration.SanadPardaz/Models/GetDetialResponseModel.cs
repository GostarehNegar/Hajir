using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class GetDetailRequestModel
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
    public class SanadRequestModel<T>
    {
        public int configId { get; set; }
        public T input { get; set; }
    }
    public class SanadResponseModel<T>
    {
        public class DataModel
        {
            public T[] result { get; set; }
        }
        public DataModel data { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }

    }

    public class GetDetialResponseModel : SanadResponseModel<DetailModel>
    {



    }
}
