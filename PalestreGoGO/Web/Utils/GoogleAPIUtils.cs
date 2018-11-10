using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.Utils
{
    public static class GoogleAPIUtils
    {
        const string GOOGLE_MAPS_API_URL = "https://maps.googleapis.com/maps/api/js?libraries=places&key={0}&language=it&callback=initMap";

        public static string GetGoogleMapsAPIUrl(string key)
        {
            return string.Format(GOOGLE_MAPS_API_URL, key);
        }

        public static string GetStaticMapUrl(string nomeMarker, float latitudine, float longitudine, string key)
        {
            return $"https://maps.googleapis.com/maps/api/staticmap?markers=size:mid%7Clabel:{WebUtility.UrlEncode(nomeMarker)}%7C{latitudine},{longitudine}&size=640x250&scale=2&maptype=roadmap&zoom=15&key={key}";
        }

        public static string GetExternalMapUrl(float latitudine, float longitudine)
        {
            return $"https://www.google.com/maps/search/?api=1&query={latitudine.ToString(CultureInfo.InvariantCulture)},{longitudine.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
