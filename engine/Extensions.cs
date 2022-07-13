namespace Beforevents
{
    using System.Globalization;
    using System.Linq;
    using System.Net;

    public static class Extensions
    {
        public static string Normal(this string str)
        {
            str = str.TrimStart().TrimEnd().Normalize();
            return WebUtility.HtmlDecode(str);
        }

        public static List<CultureInfo> cultures = new List<CultureInfo> { CultureInfo.CurrentCulture, CultureInfo.GetCultureInfo("it-IT"), CultureInfo.GetCultureInfo("en-EN") };

        public static DateTime ToDateTime(this string str, List<string>? dt = null, int cultureInfoIndex = 0)
        {
            try
            {
                str = str.ToLower();
                for (int i = 0; i < str.Length - 1; i++)
                    if ((char.IsDigit(str[i]) && char.IsLetter(str[i + 1])) || (char.IsLetter(str[i]) && char.IsDigit(str[i + 1])))
                        str = str.Insert(i + 1, " ");
                if (dt == null)
                    dt = str.Split(new string[] { " ", "-", "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                List<string> strs = dt.Where(x => !int.TryParse(x, out int tmp)).ToList();
                int month = 0, day = 0;
                do
                {
                    List<string> abbrMonths = Enumerable.Range(1, 12).Select(x => cultures[cultureInfoIndex].DateTimeFormat.GetAbbreviatedMonthName(x).ToLower()).ToList();
                    List<string> months = Enumerable.Range(1, 12).Select(x => cultures[cultureInfoIndex].DateTimeFormat.GetMonthName(x).ToLower()).ToList();
                    List<string> abbrDays = Enumerable.Range(0, 7).Select(x => cultures[cultureInfoIndex].DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)x).ToLower()).ToList();
                    List<string> days = Enumerable.Range(0, 7).Select(x => cultures[cultureInfoIndex].DateTimeFormat.GetDayName((DayOfWeek)x).ToLower()).ToList();

                    List<int> d = days.Where(x => strs.Contains(x)).Select(x => days.IndexOf(x)).ToList();
                    if (d.Count == 1)
                        strs.Remove(days[d[0]]);
                    if (d.Count == 0)
                    {
                        d = abbrDays.Where(x => strs.Contains(x)).Select(x => abbrDays.IndexOf(x)).ToList();
                        if (d.Count == 1)
                            strs.Remove(abbrDays[d[0]]);
                    }
                    if (d.Count == 1)
                        day = d[0];

                    List<int> m = months.Where(x => strs.Contains(x)).Select(x => months.IndexOf(x)).ToList();
                    if (m.Count == 1)
                        strs.Remove(months[m[0]]);
                    if (m.Count == 0)
                    {
                        m = abbrMonths.Where(x => strs.Contains(x)).Select(x => abbrMonths.IndexOf(x)).ToList();
                        if (m.Count == 1)
                            strs.Remove(abbrMonths[m[0]]);
                    }
                    if (m.Count == 1)
                        month = m[0] + 1;
                }
                while (strs.Count > 0 && cultureInfoIndex++ < cultures.Count - 1);

                List<int> numeric = dt.Where(x => int.TryParse(x, out int tmp)).Select(x => int.Parse(x)).OrderBy(x => x).ToList();
                List<int> possibleMonth = numeric.Where(x => x <= 12).ToList();
                if (possibleMonth.Count > 1)
                    throw new Exception("Ambiguity in month");
                if (possibleMonth.Count == 1 && month == 0)
                    month = possibleMonth[0];
                List<int> possibleDay = numeric.Where(x => x > 12 && x <= 31).ToList();
                if (possibleDay.Count == 1 && day == 0)
                    day = possibleDay[0];
                if (day == 0 && month != 0)
                    day = numeric.Min();
                int year = 0;
                List<int> possibleYear = numeric.Where(x => x > 31).ToList();
                if (possibleYear.Count == 1 && year == 0)
                    year = possibleYear[0];
                else
                    year = DateTime.Now.Year;

                return new DateTime(year, month, day);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception();
            }
        }

        public static bool ToDateTimeTests()
        {
            if ("22 Ago".ToDateTime() == new DateTime(2022, 8, 22))
                if ("22 Aug".ToDateTime() == new DateTime(2022, 8, 22))
                    if ("22-8-2024".ToDateTime() == new DateTime(2024, 8, 22))
                        if ("22/8/2028".ToDateTime() == new DateTime(2028, 8, 22))
                            if ("22/08/2021".ToDateTime() == new DateTime(2021, 8, 22))
                                if ("08/22/2025".ToDateTime() == new DateTime(2025, 8, 22))
                                    if ("2026/08/22".ToDateTime() == new DateTime(2026, 8, 22))
                                        if ("2026/22/08".ToDateTime() == new DateTime(2026, 8, 22))
                                            if ("2027-22-08".ToDateTime() == new DateTime(2027, 8, 22))
                                                if ("2020-22 Aug".ToDateTime() == new DateTime(2020, 8, 22))
                                                    if ("2025/Aug 22".ToDateTime() == new DateTime(2025, 8, 22))
                                                        if ("22 Aug 2021".ToDateTime() == new DateTime(2021, 8, 22))
                                                            if ("5 Aug 2023".ToDateTime() == new DateTime(2023, 8, 5))
                                                                if ("5 Ago 2024".ToDateTime() == new DateTime(2024, 8, 5))
                                                                    if ("Ago-5/2025".ToDateTime() == new DateTime(2025, 8, 5))
                                                                        if ("6Settembre2025".ToDateTime() == new DateTime(2025, 9, 6))
                                                                            if ("Settembre6/2025".ToDateTime() == new DateTime(2025, 9, 6))
                                                                                //if ("5 5 2021".ToDateTime() == new DateTime(2021, 5, 5)) //ambiguity
                                                                                //if ("5 8 2026".ToDateTime() == new DateTime(2026, 8, 5))
                                                                                return true;

            return false;
        }
    }
}