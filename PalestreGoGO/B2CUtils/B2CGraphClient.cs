using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace B2CUtils
{
    public class B2CGraphClient
    {
        const string GraphTenant = "ready2do.onmicrosoft.com";
        const string GraphClientId = "827f6bab-a885-43a9-952b-028ac0e17bf9";
        const string GraphClientSecret = "hzS7X97GRCMMLqP/vbRvqnPW6JYtnRHjcy7KLQl0wTY=";

        #region CONSTANTS
        const string aadInstance = "https://login.microsoftonline.com/";
        const string aadGraphResourceId = "https://graph.windows.net/";
        const string aadGraphEndpoint = "https://graph.windows.net/";
        const string aadGraphSuffix = "";
        const string aadGraphVersion = "api-version=1.6";


        const string API_USERS = "/users";

        const string StruttureOwned = "extension_827f6baba88543a9952b028ac0e17bf9_struttureOwned";
        const string StruttureGestite = "extension_827f6baba88543a9952b028ac0e17bf9_struttureGestite";
        const string AccountConfirmedOn = "extension_827f6baba88543a9952b028ac0e17bf9_accountConfirmedOn";
        const string Refereer = "extension_827f6baba88543a9952b028ac0e17bf9_refereer";
        #endregion

        AuthenticationContext _authContext;
        ClientCredential _credential;
        Serializer _msGraphSerializer;

        public B2CGraphClient()
        {

            // The AuthenticationContext is ADAL's primary class, in which you indicate the tenant to use.
            this._authContext = new AuthenticationContext("https://login.microsoftonline.com/" + GraphTenant);

            // The ClientCredential is where you pass in your client_id and client_secret, which are
            // provided to Azure AD in order to receive an access_token by using the app's identity.
            this._credential = new ClientCredential(GraphClientId, GraphClientSecret);

            //usiamo lo stesso serializare usato nella librearira Microsoft.Graph (che internamente usa cmq il NewtonSoft)
            this._msGraphSerializer = new Serializer();
        }


        public async Task<string> GetUserById(string userId)
        {
            var response = await SendGraphGetRequestAsync($"{API_USERS}/{userId}", null);
            return response;
        }

        //public async Task<AzureUser> GetUserByMailAsync(string email)
        //{
        //    string query = $"$filter=(signInNames/any(x:x/value eq '{email}')) or (userPrincipalName eq '{email}')";
        //    var response = await SendGraphGetRequestAsync(API_USERS, query);
        //    var resp = JsonConvert.DeserializeObject<GraphResponse>(response);
        //    var result = _msGraphSerializer.DeserializeObject<List<AzureUser>>(resp.Value).FirstOrDefault();
        //    return result;
        //}

        ///// <summary>
        ///// Prende in input un AzureUser con le sole proprietà da aggiornare
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public async Task UpdateUserProfile(string userId, AzureUser user)
        //{
        //    if (user == null) { throw new ArgumentNullException(nameof(user)); }
        //    string api = $"{API_USERS}/{HttpUtility.UrlEncode(userId)}";
        //    await SendGraphPatchRequestAsync(api, JsonConvert.SerializeObject(user));
        //}

        //private async Task<T> SendGraphPostRequestAsync<T>(string api, string json) where T : class
        //{
        //    // NOTE: This client uses ADAL v2, not ADAL v4
        //    AuthenticationResult result = await _authContext.AcquireTokenAsync(aadGraphResourceId, _credential);
        //    HttpClient http = new HttpClient();
        //    string url = aadGraphEndpoint + _tenant + api + "?" + aadGraphVersion;

        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        //    HttpResponseMessage response = await http.SendAsync(request);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        string error = await response.Content.ReadAsStringAsync();
        //        object formatted = JsonConvert.DeserializeObject(error);
        //        throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
        //    }
        //    var reply = await response.Content.ReadAsStringAsync();
        //    if (typeof(T) == typeof(string)) { return reply as T; }
        //    return JsonConvert.DeserializeObject<T>(reply);
        //}

        public async Task<string> SendGraphGetRequestAsync(string api, string query)
        {
            // First, use ADAL to acquire a token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for; in this case, the Graph API.
            AuthenticationResult result = await _authContext.AcquireTokenAsync("https://graph.windows.net", _credential);

            // For B2C user managment, be sure to use the 1.6 Graph API version.
            HttpClient http = new HttpClient();
            string url = string.Format("{0}{1}{2}?{3}", "https://graph.windows.net/", GraphTenant, api, aadGraphVersion);
            if (!string.IsNullOrEmpty(query))
            {
                url += "&" + query;
            }

            // Append the access token for the Graph API to the Authorization header of the request, using the Bearer scheme.
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }
            return await response.Content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<GraphResponse>(await response.Content.ReadAsStringAsync());
        }

        //private async Task<string> SendGraphPatchRequestAsync(string api, string json)
        //{
        //    // NOTE: This client uses ADAL v2, not ADAL v4
        //    AuthenticationResult result = await _authContext.AcquireTokenAsync(aadGraphResourceId, _credential);
        //    HttpClient http = new HttpClient();
        //    string url = string.Format("{0}{1}{2}?{3}", aadGraphEndpoint, _tenant, api, aadGraphVersion);
        //    Log.Information("Sending PATCH request to Endpoint: {url} - Payload: {json}", url, json);
        //    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        //    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        //    HttpResponseMessage response = await http.SendAsync(request);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        string error = await response.Content.ReadAsStringAsync();
        //        object formatted = JsonConvert.DeserializeObject(error);
        //        throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
        //    }
        //    return await response.Content.ReadAsStringAsync();
        //}



    }
}
