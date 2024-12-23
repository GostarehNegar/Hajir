using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hajir.Crm.Products
{
    public enum BatteryTypes
    {
        Seven,
        Nine,
        Tewelve,
        Eigtheen,
        TwentyEight,
        Fourty,
        SixtyFive,
        Hundered
    }
    public class BatterySpec
    {
        public int Number { get; private set; }
        public decimal PowerFactor { get; private set; }
        public BatterySpec(int number, decimal powerFactor)
        {
            Number = number;
            PowerFactor = powerFactor;
        }
        public static BatterySpec Parse(string b)
        {
            if (int.TryParse(b.Split(':')[0], out int value))
            {
                if (b.Split(':').Length > 1 && decimal.TryParse(b.Split(':')[1], out var factor))
                {
                    return new BatterySpec(value, factor);
                }
                else
                {
                    return new BatterySpec(value, 0);
                }
            }
            return null;
        }
        public static IEnumerable<BatterySpec> ParseCollection(string text)
        {
            return (text ?? "").Split(new char[] { ',', ';' })
                .Select(x => Parse(x))
                .Where(x => x != null)
                .ToArray();
        }

    }
}
