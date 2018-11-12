using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PalestreGoGo.WebAPI.Utils
{
    public class B2CGraphClient
    {
        #region CONSTANTS
        const string aadInstance = "https://login.microsoftonline.com/";
        const string aadGraphResourceId = "https://graph.windows.net/";
        const string aadGraphEndpoint = "https://graph.windows.net/";
        const string aadGraphSuffix = "";
        const string aadGraphVersion = "api-version=1.6";


        const string API_USERS = "/users";
        #endregion
        string _clientId;
        string _clientSecret;
        string _tenant;
        AuthenticationContext _authContext;
        ClientCredential _credential;
        Serializer _msGraphSerializer;
        GraphAPIOptions _options;

        public B2CGraphClient(IOptions<GraphAPIOptions> options)
        {
            _options = options.Value;
            // The client_id, client_secret, and tenant are provided in Program.cs, which pulls the values from App.config
            this._clientId = _options.ClientId;
            this._clientSecret = _options.ClientSecret;
            this._tenant = _options.Tenant;

            // The AuthenticationContext is ADAL's primary class, in which you indicate the tenant to use.
            this._authContext = new AuthenticationContext("https://login.microsoftonline.com/" + _tenant);

            // The ClientCredential is where you pass in your client_id and client_secret, which are
            // provided to Azure AD in order to receive an access_token by using the app's identity.
            this._credential = new ClientCredential(_clientId, _clientSecret);

            //usiamo lo stesso serializare usato nella librearira Microsoft.Graph (che internamente usa cmq il NewtonSoft)
            this._msGraphSerializer = new Serializer();
        }

        public async Task<AzureUser> CreateUserAsync(AzureUser user)
        {
            var response = await SendGraphPostRequestAsync<AzureUser>(API_USERS, JsonConvert.SerializeObject(user));
            //var result = _msGraphSerializer.DeserializeObject<List<LocalAccountUser>>(response.Value);
            return response;
        }

        public async Task<AzureUser> GetUserById(string userId)
        {
            var response = await SendGraphGetRequestAsync($"{API_USERS}/{userId}", null);
            var result = _msGraphSerializer.DeserializeObject<AzureUser>(response);
            return result;
        }

        public async Task<AzureUser> GetUserByMailAsync(string email)
        {
            string query = $"$filter=(signInNames/any(x:x/value eq '{email}')) or (userPrincipalName eq '{email}')";
            var response = await SendGraphGetRequestAsync(API_USERS, query);
            var resp = JsonConvert.DeserializeObject<GraphResponse>(response);
            var result = _msGraphSerializer.DeserializeObject<List<AzureUser>>(resp.Value).FirstOrDefault();
            return result;
        }

        private async Task<T> SendGraphPostRequestAsync<T>(string api, string json)where T:class
        {
            // NOTE: This client uses ADAL v2, not ADAL v4
            AuthenticationResult result = await _authContext.AcquireTokenAsync(aadGraphResourceId, _credential);
            HttpClient http = new HttpClient();
            string url = aadGraphEndpoint + _tenant + api + "?" + aadGraphVersion;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }
            var reply = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string)) { return reply as T; }
            return JsonConvert.DeserializeObject<T>(reply);
        }

        public async Task<string> SendGraphGetRequestAsync(string api, string query)
        {
            // First, use ADAL to acquire a token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for; in this case, the Graph API.
            AuthenticationResult result = await _authContext.AcquireTokenAsync("https://graph.windows.net", _credential);

            // For B2C user managment, be sure to use the 1.6 Graph API version.
            HttpClient http = new HttpClient();
            string url = string.Format("{0}{1}{2}?{3}", "https://graph.windows.net/", _tenant, api, aadGraphVersion);
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

        private async Task<string> SendGraphPatchRequest(string api, string json)
        {
            // NOTE: This client uses ADAL v2, not ADAL v4
            AuthenticationResult result = await _authContext.AcquireTokenAsync(aadGraphResourceId, _credential);
            HttpClient http = new HttpClient();
            string url = string.Format("{0}{1}{2}?{3}", aadGraphEndpoint, _tenant, api, aadGraphVersion);

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            return await response.Content.ReadAsStringAsync();
        }



    }
}
