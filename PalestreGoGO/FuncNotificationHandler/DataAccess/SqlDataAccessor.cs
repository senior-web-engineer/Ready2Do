using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace FuncNotificationHandler.DataAccess
{
    public class SqlDataAccessor
    {
        public async static Task<MailTemplate> LoadMailTemplateAsync(string tipoTemplate)
        {
            using (var cn = new SqlConnection(ConfigurationManager.AppSettings["DB_CONNECTION"]))
            {
                return await cn.QueryFirstAsync<MailTemplate>("SELECT Id, TipoMail, Subject, TextBody, HtmlBody, OnlyText FROM MailTemplates WHERE TipoMail = @tipoMail AND DataCancellazione IS NULL",
                                                                new { tipoMail = tipoTemplate });
            }
        }

        public async static Task<long> AddSystemNotificationAsync(int idTipo, string idUtente, int idCliente, string titolo, string testo )
        {
            var parameters = new DynamicParameters(new
            {
                pIdTipo = idTipo,
                pIdUtente = idUtente,
                pIdCliente = idCliente,
                pTitolo = titolo,
                pTesto = testo
            });
            parameters.Add("pIdNotitifica", dbType: DbType.Int64, direction: ParameterDirection.Output);

            using (var cn = new SqlConnection(ConfigurationManager.AppSettings["DB_CONNECTION"]))
            {
                await cn.ExecuteAsync("[dbo].[Notifiche_Add]", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<long>("pIdNotitifica");
            }
        }
    }
}
