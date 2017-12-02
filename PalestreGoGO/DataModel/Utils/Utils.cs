using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public static class Utils
    {
        public static DateTime DateTimeFromDateAndTime(DateTime date, TimeSpan time) => new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
    }
}
