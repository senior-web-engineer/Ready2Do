using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
