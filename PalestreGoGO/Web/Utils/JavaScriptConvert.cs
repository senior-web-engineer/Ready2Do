using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Utils
{
    /// <summary>
    /// JavaScriptConvert almost works like Json.NET's native JsonConvert serializer, except that it removes quotes around object names and enforces camelCased property names. 
    /// The cool thing is that the CamelCasePropertyNamesContractResolver is being smart about abbreviations like "ID", which will not be turned into "iD", but into the all-lower "id".
    /// </summary>
    /// <see cref="https://blog.mariusschulz.com/2014/02/05/passing-net-server-side-data-to-javascript"/>
    public static class JavaScriptConvert
    {
        public static HtmlString SerializeObject(object value)
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = new JsonSerializer
                {
                    // Let's use camelCasing as is common practice in JavaScript
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                // We don't want quotes around object names
                jsonWriter.QuoteName = false;
                serializer.Serialize(jsonWriter, value);

                return new HtmlString(stringWriter.ToString());
            }
        }
    }
}
