using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Azure.Func.WaitingListExpiration
{
    public static class WaitingListExpirationHandler
    {
        private const string CONN_STRING_KEY = "DB_CONNECTION";
        private const string MAXITEMS_KEY = "MAXITEMS_WAITINGLIST";
        private const int MAXITEMS_DEFAULT = 1000;

        [FunctionName("Ready2Do-WaitingListExpirationHandler")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {

            log.LogDebug($"Ready2Do.WaitingListExpirationHandler trigger function started at: {DateTime.Now}");
            try
            {
                string connString = Environment.GetEnvironmentVariable(CONN_STRING_KEY);
                string appo = Environment.GetEnvironmentVariable(CONN_STRING_KEY);
                int maxItems;
                if (!int.TryParse(appo, out maxItems))
                {
                    maxItems = MAXITEMS_DEFAULT;
                }
                if (string.IsNullOrWhiteSpace(connString))
                {
                    log.LogError($"Impossibile recuperera la ConnectionString con Key:{CONN_STRING_KEY}");
                    return;
                }
                using (var cn = new SqlConnection(connString))
                {
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = "ListeAttesa_CleanExpired";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pMaxItems", SqlDbType.Int).Value = maxItems;
                    await cn.OpenAsync();
                    int numItemsProcessed = (int)await cmd.ExecuteScalarAsync();
                    log.LogInformation($"Processati {numItemsProcessed} items.");
                }
            }
            finally
            {
                log.LogDebug($"Ready2Do.WaitingListExpirationHandler trigger function finished at: {DateTime.Now}");
            }
        }
    }
}
