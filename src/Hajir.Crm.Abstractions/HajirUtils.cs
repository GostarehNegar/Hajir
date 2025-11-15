using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hajir.Crm
{
    public class HajirUtils
    {
        public static HajirUtils Instance = new HajirUtils();

        public decimal? GetBatteryPowerFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var parts = name.Split(' ');
            for (var i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "آمپر")
                {
                    if (i > 0 && decimal.TryParse(parts[i + -1], out var _res))
                        return _res;
                }
            }
            return null;

        }
        public string FormatAmount(decimal? amount) => string.Format("{0:###,###}", amount);

        public int? GetCabinetFloorsFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var parts = name.Split(' ');
            for (var i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "طبقه" && i > 0)
                {
                    switch (parts[i - 1])
                    {
                        case "دو": return 2;
                        case "سه": return 3;
                        case "یک": return 1;
                        case "چهار": return 4;
                        default:
                            break;

                    }
                    if (i > 0 && int.TryParse(parts[i + -1], out var _res))
                        return _res;
                }
            }
            return null;
        }

        public int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }

            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];

            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;

                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[lengthA, lengthB];
        }

        public string RemoveArabic(string str)
        {
            return str == null ? str : str.Replace("ك", "ک").Replace("ي", "ی");

        }
        public decimal? GetKVA(string kva, string tag = "KVA")
        {
            if (string.IsNullOrWhiteSpace(kva)) return null;
            kva = kva.Replace("کاوا", "KVA").ToUpperInvariant();
            var idx = kva.IndexOf(tag) - 1;
            while (idx > 0 && kva[idx] == ' ')
                idx--;
            var digits = "";
            while (idx > 0 && (char.IsDigit(kva[idx]) || kva[idx] == '.'))
            {
                digits = kva[idx] + digits;
                idx--;
            }
            var result = !string.IsNullOrWhiteSpace(digits) && decimal.TryParse(digits, out var _r)
                ? _r
                : (decimal?)null;
            if (result==null && tag == "KVA")
            {
                var va = GetKVA(kva, "VA");
                if (va.HasValue)
                {
                    return va / 1000;
                }
            }
            return result;
        }
    }
}
