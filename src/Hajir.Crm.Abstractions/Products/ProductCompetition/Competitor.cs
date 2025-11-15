using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hajir.Crm.Products.ProductCompetition
{
    public class Competitor
    {
        public class PriceItem
        {
            public string Name { get; set; }
            public string Price { get; set; }
            public string Description { get; set; }
            public string Prodcut_Url { get; set; }
            public string Manufacturer { get; set; }
            public decimal GetKVA()
            {
                return HajirUtils.Instance.GetKVA(this.Name) ?? 0;// + (HajirUtils.Instance.GetKVA(this.Name, "VA") ?? 0) / 1000;
            }
            public bool IsUps()
            {
                return this.Name != null && (this.Name.ToUpper().Contains("UPS") || this.Name.Contains("یوپی") || this.Name.Contains("یو پی"));
            }
            public decimal GetPrice()
            {
                if (this.Price == null) return 0;
                var isToman = this.Price.Contains("توم") ? 10 : 1;
                var p = "";
                foreach (var c in this.Price)
                {
                    if (char.IsDigit(c) || c == '.')
                        p = p + c;
                    else if (c == ',')
                    {

                    }
                    else
                    {
                        break;
                    }
                }
                p = p.Replace(" ", "");
                var res = !string.IsNullOrWhiteSpace(p) && decimal.TryParse(p, out var __p) ? __p * isToman : 0;
                return !string.IsNullOrWhiteSpace(p) && decimal.TryParse(p, out var _p) ? _p * isToman : 0;
            }
        }
        public string Name { get; set; }
        public PriceItem[] Items { get; set; }

        public Competitor()
        {

        }
        public Competitor(string name, PriceItem[] items)
        {
            this.Name = name;
            this.Items = items;
        }
    }
}
