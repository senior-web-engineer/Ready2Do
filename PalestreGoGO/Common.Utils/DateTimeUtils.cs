using System;
using System.Globalization;
using System.Web;

namespace Common.Utils
{
    public static class DateTimeUtils
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string ToISO8601(this DateTime dt, bool urlEncoded)
        {
            if (urlEncoded)
                return HttpUtility.UrlEncode(dt.ToString("o", CultureInfo.InvariantCulture));
            else
                return dt.ToString("o", CultureInfo.InvariantCulture);
        }

        public static DateTime? FromIS8601(this string isoDate)
        {
            DateTime? result = null;
            DateTime tmp;
            if (DateTime.TryParse(isoDate, null, DateTimeStyles.RoundtripKind, out tmp))
            {
                result = tmp;
            }
            return result;
        }

        public static DateTime FromUnixTimeSeconds(this long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        public static DateTime FromUnixTimeMilliSeconds(this long unixTime)
        {
            return epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Ritorna il numero di MILLISECONDI (attenzione NON secondi) dalla Epoch di Unix (1970/1/1)
        /// </summary>
        /// <param name="winTime">DateTime da convertire</param>
        /// <returns></returns>
        public static long ToUnixTimeMimlliSeconds(this DateTime winTime)
        {
            return (long)winTime.Subtract(epoch).TotalMilliseconds;
        }

        public static long ToUnixTimeSeconds(this DateTime winTime)
        {
            return (long)winTime.Subtract(epoch).TotalSeconds;
        }
    }
}
