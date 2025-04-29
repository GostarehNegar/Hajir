using System;
using System.Collections.Generic;
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

        public int? GetCabinetFloorsFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var parts = name.Split(' ');
            for (var i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "طبقه" && i>0)
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
    }
}
