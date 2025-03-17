using System.Collections.Generic;
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
        public string Value { get; set; }
        //public string Type { get; set; }
    }
    public class Datasheet
    {
        public DatasheetProperty[] Properties { get; set; }

        public string ProductCode =>
            (Properties.FirstOrDefault(x => x.Description == "كد كالا") ?? Properties.FirstOrDefault())?
            .Value;
        public string ProductName =>
           (Properties.FirstOrDefault(x => x.Description == "نام كالا") ?? Properties.Skip(1).FirstOrDefault())?
           .Value;

        public string GetBatterySpec()
        {
            var specs = new List<string>();
            var result = "";

            for (var i = 1; i < 12; i++)
            {
                var n = Properties.FirstOrDefault(x => x.Description == $"تعداد باتری {i}")?.Value;
                var p = Properties.FirstOrDefault(x => x.Description == $"Power Factor {i}")?.Value;
                if (!string.IsNullOrWhiteSpace(n))
                {
                    specs.Add($"{n}:{p}");
                }
            }
            return string.Join(",", specs);

        }
    }
}
