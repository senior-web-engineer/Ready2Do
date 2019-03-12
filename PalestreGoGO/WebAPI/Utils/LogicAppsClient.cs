using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Utils
{
    public class LogicAppsClient
    {
        private static HttpClient httpClient = new HttpClient();
        private IConfiguration _configuration;

        public LogicAppsClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<string> StartAppForAppuntamentoDaConfermare(int idCliente, int idSchedule, string userId, int timeoutMinutes)
        {
            Log.Information($"StartAppForAppuntamentoDaConfermare(idCliente:{idCliente}, idSchedule:{idSchedule}, userId:{userId}, timeoutMinutes:{timeoutMinutes}");
            string url = _configuration.GetValue<string>("Azure:LogicApps:AppuntamentiDaConfermareHandler:Url");
            Log.Debug("URL LogicApp AppuntamentiDaConfermareHandle: {Url}", url);
            StringContent sc = new StringContent($"{{'idCliente': {idCliente}, 'idSchedule':{idSchedule}, 'userId':'{userId}', 'timeoutMinutes':{timeoutMinutes}");
            var response = await httpClient.PostAsync(url, sc);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
