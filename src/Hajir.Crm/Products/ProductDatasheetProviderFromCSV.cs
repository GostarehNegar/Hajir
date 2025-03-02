using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Products
{
    public interface IProductDatasheetProviderFromCSV : IDatasheetProvider
    {
        DatasheetPropDefinition[] GetProps();

    }
    public class ProductDatasheetProviderFromCSV : IProductDatasheetProviderFromCSV
    {
        private readonly IMemoryCache cache;

        public ProductDatasheetProviderFromCSV(IMemoryCache cache)
        {
            this.cache = cache;
        }
        private Datasheet ToDatasheet(DatasheetPropDefinition[] props, string[] values)
        {
            DatasheetPropDefinition getspec(int i)
            {
                return i < props.Length ? props[i] : null;
            }
            object get_value(string v)
            {
                return string.IsNullOrWhiteSpace(v)
                    ? v
                    : decimal.TryParse(v, out var _res) ? (object)_res : v;

            }
            return new Datasheet
            {
                Properties = values.Select((a, i) => new DatasheetProperty
                {
                    Name = getspec(i)?.Name,
                    Description = getspec(i)?.Description,
                    Type = getspec(i)?.Type,
                    Value = get_value(a)
                }).ToArray()
            };
        }

        private string RemoveQuotes(string s)
        {
            var result = "";
            bool inquote = false;
            foreach (var ch in s)
            {
                switch (ch)
                {
                    case '"':
                        inquote = !inquote;
                        break;
                    case '\n':
                    case '\r':
                        if (!inquote)
                        {
                            result += ch;
                        }
                        break;
                    default: 
                        result += ch; 
                        break;

                }
            }
            return result;
        }
        public Datasheet[] DoGetDatasheets()
        {

            return this.cache.GetOrCreate<Datasheet[]>("_CSVDATASHEETS__", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                var prps = this.GetProps();

                var file_name = HajirCrmConstants.GetDatasheetFullPathFileName();
                if (!File.Exists(file_name))
                {
                    throw new Exception($"File Not Found:{file_name}");
                }
                var text = RemoveQuotes(File.ReadAllText(file_name));
                return text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => ToDatasheet(prps, x.Split('\t').ToArray()))
                .ToArray();
            });
        }

        public DatasheetPropDefinition[] GetProps()
        {
            return this.cache.GetOrCreate<DatasheetPropDefinition[]>("_DATASHEET_PROPSPEC_", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                if (File.Exists(HajirCrmConstants.GetDatasheetPropSpecFullPath()))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DatasheetPropDefinition[]>(
                        File.ReadAllText(HajirCrmConstants.GetDatasheetPropSpecFullPath()));
                }
                throw new Exception($"File Not Found:{HajirCrmConstants.GetDatasheetPropSpecFullPath()}");
            });
        }

        static string ConvertToExcelColumn(int columnNumber)
        {
            string columnName = "";

            while (columnNumber >= 0)
            {
                int remainder = columnNumber % 26;
                columnName = (char)('A' + remainder) + columnName;
                columnNumber = (columnNumber / 26) - 1;
            }

            return columnName;
        }
        static int ConvertFromExcelColumn(string columnName)
        {
            int columnNumber = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                char currentChar = columnName[i];
                int value = currentChar - 'A' + 1; // Convert character to its corresponding value (A=1, B=2, ..., Z=26)
                columnNumber = columnNumber * 26 + value; // Accumulate the result
            }

            return columnNumber - 1; // Convert to zero-based index
        }

        public void CreatePropSpecFile()
        {
            var props = File.ReadAllText(HajirCrmConstants.GetDatasheetFullPathFileName())
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)[0]
                .Split('\t');
            var propList = new List<DatasheetPropDefinition>();
            var i = 0;
            var batteryIdx = 0;
            foreach (var prop in props)
            {
                var spec = new DatasheetPropDefinition
                {
                    Name = prop.Trim(),
                    Description = prop.Trim(),
                    Label = ConvertToExcelColumn(i),
                    Type = "string"
                };
                propList.Add(spec);
                if (prop.Contains("تعداد باتری"))
                {
                    batteryIdx++;
                    spec.Description = $"{spec.Description} {batteryIdx}";
                    spec.Type = "decimal";
                }
                if (spec.Description.Contains("Power Factor"))
                {
                    spec.Description = $"{spec.Description} {batteryIdx}";
                    spec.Type = "decimal";
                }
                if (string.IsNullOrWhiteSpace(spec.Description) && batteryIdx > 0)
                {
                    spec.Description = $"Power Factor {batteryIdx}";
                    spec.Type = "decimal";
                }
                spec.Name = spec.Description;

                i++;
            }
            File.WriteAllText("DatasheetProps.json", Newtonsoft.Json.JsonConvert.SerializeObject(propList));






        }

        public Task<Datasheet[]> GetDatasheets()
        {
            return Task.FromResult(this.DoGetDatasheets());

        }
    }
}
