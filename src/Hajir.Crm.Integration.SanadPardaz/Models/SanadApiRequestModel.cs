namespace Hajir.Crm.Integration.SanadPardaz.Models
{
    public class SanadApiRequestModel<T>
    {
        public int configId { get; set; }
        public T input { get; set; }
    }

}
