using System.Linq;

namespace Hajir.Crm.Products
{
    public class DatasheetPropDefinition
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }

    }
    public class DatasheetProperty
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
    }
    public class Datasheet
    {
        public DatasheetProperty[] Properties { get; set; }

        public string ProductCode => Properties.FirstOrDefault(x => x.Description == "کد کالا")?.Value.ToString();

    }
}
