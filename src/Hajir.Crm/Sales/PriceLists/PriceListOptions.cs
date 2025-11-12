namespace Hajir.Crm.Sales.PriceLists
{
    public class PriceListOptions
    {
        public bool Disabled { get; set; } = false;
        public PriceListOptions Validate()
        {
            return this;
        }
    }
}