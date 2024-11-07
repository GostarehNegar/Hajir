using System.Text;

namespace Hajir.Crm.Internals
{
    public class PersianNumberToTextConverter
    {
        private static readonly string[] units = { "", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه", "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static readonly string[] tens = { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static readonly string[] hundreds = { "", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static readonly string[] scales = { "", "هزار", "میلیون", "میلیارد" };

        public static string ConvertToText(decimal number)
        {
            if (number == 0)
                return "صفر";

            StringBuilder result = new StringBuilder();
            int scaleIndex = 0;

            while (number > 0)
            {
                int part = (int)(number % 1000);
                if (part > 0)
                {
                    string partText = ConvertThreeDigits(part);
                    if (scaleIndex > 0)
                    {
                        partText += " " + scales[scaleIndex];
                    }
                    if (result.Length > 0)
                    {
                        result.Insert(0, " و ");
                    }
                    result.Insert(0, partText);
                }
                number /= 1000;
                scaleIndex++;
            }

            // Remove trailing "و" if present
            string finalResult = result.ToString().TrimEnd();
            if (finalResult.EndsWith(" و"))
            {
                finalResult = finalResult.Substring(0, finalResult.Length - 2);
            }

            return finalResult;
        }

        private static string ConvertThreeDigits(int number)
        {
            StringBuilder result = new StringBuilder();

            int hundredsDigit = number / 100;
            int remainder = number % 100;

            if (hundredsDigit > 0)
            {
                result.Append(hundreds[hundredsDigit]);
                if (remainder > 0)
                    result.Append(" و ");
            }

            if (remainder > 0)
            {
                if (remainder < 20)
                {
                    result.Append(units[remainder]);
                }
                else
                {
                    int tensDigit = remainder / 10;
                    int unitsDigit = remainder % 10;

                    result.Append(tens[tensDigit]);
                    if (unitsDigit > 0)
                    {
                        result.Append(" و " + units[unitsDigit]);
                    }
                }
            }

            return result.ToString();
        }
    }
}
