using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.Crm.Excel.AddIn
{
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
    }

}
