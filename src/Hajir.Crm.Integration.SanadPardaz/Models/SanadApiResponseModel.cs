namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class SanadApiResponseModel<T>
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

}
