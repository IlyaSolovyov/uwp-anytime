using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyTimeT10.Services
{
    public class CalendarService
    {
        private static DateTime GetFirstDayOfWeek(DateTime date)
        {
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }
            return date;
        }
        private static DayOfWeek GetDayFromString(string day)
        {
            return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), day, true);
        }

        public static List<DateTime> GetWeekDatesList(int offset)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime date = GetFirstDayOfWeek(DateTime.Now).AddDays(7 * offset);
            for (int i = 0; i < 7; i++)
            {
                dates.Add(date);
                date = date.AddDays(1);
            }
            return dates;
        }
        public static DateTime GetDateByDayName(string dayName, int offset)
        {
            DayOfWeek day = GetDayFromString(dayName);
            DateTime date = GetFirstDayOfWeek(DateTime.Now.AddDays(7 * offset));
            while (date.DayOfWeek != day)
            {
                date = date.AddDays(1);
            }
            return date;
        }
       
    }
}
