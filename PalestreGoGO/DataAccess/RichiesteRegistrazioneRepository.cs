using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel.Exceptions;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class RichiesteRegistrazioneRepository : BaseRepository, IRichiesteRegistrazioneRepository
    {
        internal static async Task<RichiestaRegistrazioneDM> InternalReadRichiestaRegistrazioneAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            var result = new RichiestaRegistrazioneDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.DataRichiesta = dr.GetDateTime(columns["DataRichiesta"]);
            result.CorrelationId = dr.GetString(columns["CorrelationId"]);
            result.UserCode = dr.GetString(columns["UserCode"]);
            result.Username = dr.GetString(columns["Username"]);
            result.Expiration = dr.GetDateTime(columns["Expiration"]);
            result.DataConferma = await dr.IsDBNullAsync(columns["DataConferma"]) ? default(DateTime?) : dr.GetDateTime(columns["DataConferma"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            return result;
        }

        internal static Dictionary<string, int> ResolveColumnsRichiestaRegistrazione(SqlDataReader dr)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", dr.GetOrdinal("Id"));
            result.Add("DataRichiesta", dr.GetOrdinal("DataRichiesta"));
            result.Add("CorrelationId", dr.GetOrdinal("CorrelationId"));
            result.Add("UserCode", dr.GetOrdinal("UserCode"));
            result.Add("Username", dr.GetOrdinal("Username"));
            result.Add("Expiration", dr.GetOrdinal("Expiration"));
            result.Add("DataConferma", dr.GetOrdinal("DataConferma"));
            result.Add("DataCancellazione", dr.GetOrdinal("DataCancellazione"));
            return result;
        }

        public RichiesteRegistrazioneRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task AnnullaRichiestaRegistrazioneAsync(string username)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[RichiestaRegistrazione_Delete]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUsername", SqlDbType.VarChar, 500).Value = username;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task<EsitoConfermaRegistrazioneDM> CompletaRichiestaRegistrazioneAsync(string username, string code)
        {
            EsitoConfermaRegistrazioneDM result = new EsitoConfermaRegistrazioneDM();
            SqlParameter parEsito = new SqlParameter("@pEsitoConferma", SqlDbType.Int);
            parEsito.Direction = ParameterDirection.Output;
            SqlParameter parIdCliente = new SqlParameter("@pIdCliente", SqlDbType.Int);
            parIdCliente.Direction = ParameterDirection.Output;
            SqlParameter parIdRefereer = new SqlParameter("@pIdRefereer", SqlDbType.Int);
            parIdRefereer.Direction = ParameterDirection.Output;
            try
            {
                using (var cn = GetConnection())
                {
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = "RichiestaRegistrazione_Completa";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pUserCode", SqlDbType.VarChar, 1000).Value = code;
                    cmd.Parameters.Add("@pUserName", SqlDbType.VarChar, 500).Value = username;
                    cmd.Parameters.Add(parIdCliente);
                    cmd.Parameters.Add(parIdRefereer);
                    cmd.Parameters.Add(parEsito);
                    await cn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    result.EsitoConferma = ((int)parEsito.Value) == 1;
                    result.IdCliente = parIdCliente.Value != DBNull.Value ? (int?)parIdCliente.Value : default(int?);
                    result.IdRefereer = parIdRefereer.Value != DBNull.Value ? (int?)parIdRefereer.Value : default(int?);
                }
            }
            catch (SqlException exc)
            {
                Log.Error(exc, "Errore duranta la conferma dell'utente [{username}]", username);
                throw new UserConfirmationException($"Impossibile confermare la registrazione dell'utente [{username}] con il codice [{code}]", exc);
            }
            catch(Exception exc)
            {
                Log.Error(exc,"Errore durante la conferma dell'utente [{username}]", username);
                throw;
            }
            return result;
        }

        public async Task<RichiestaRegistrazioneDM> GetUltimaRichiestaRegistrazioneAsync(string userName)
        {
            RichiestaRegistrazioneDM result = default(RichiestaRegistrazioneDM);
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "RichiestaRegistrazione_GetLast";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUsername", SqlDbType.VarChar, 500).Value = userName;
                await cn.OpenAsync();
                using(var dr = await cmd.ExecuteReaderAsync())
                {
                    Dictionary<string, int> columns = ResolveColumnsRichiestaRegistrazione(dr);
                    if (await dr.ReadAsync())
                    {
                        result = await InternalReadRichiestaRegistrazioneAsync(dr, columns);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Creo una nuova richiesta di registrazione per l'utente se non ne esiste già una
        /// I parametri opzionali sono utilizzati solo nel caso di creazione di nuova richiesta
        /// </summary>
        /// <param name="username"></param>
        /// <param name="expiration"></param>
        /// <param name="correlationId"></param>
        /// <param name="idRefereer"></param>
        /// <returns></returns>
        public async Task<string> NuovaRichiestaRegistrazioneAsync(string username, DateTime? expiration, Guid? correlationId, int? idRefereer)
        {
            SqlParameter parCode = new SqlParameter("@pUserCode", SqlDbType.VarChar, 1000);
            parCode.Direction = ParameterDirection.Output;

            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[RichiestaRegistrazione_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUsername", SqlDbType.VarChar, 500).Value = username;
                cmd.Parameters.Add("@pCorrelationId", SqlDbType.UniqueIdentifier).Value = correlationId;
                cmd.Parameters.Add("@pExpiration", SqlDbType.DateTime2).Value = expiration;
                cmd.Parameters.Add(parCode);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return parCode.Value as string;
            }
        }
    }
}
