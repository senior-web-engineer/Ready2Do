using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Web.Utils
{
    public static class DateTimeUtils
    {
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
    }
}
