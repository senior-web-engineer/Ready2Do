using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class ClientiRepository : BaseRepository, IClientiRepository
    {
        private ILogger<ClientiRepository> _logger;
        public ClientiRepository(IConfiguration configuration, ILogger<ClientiRepository> logger) : base(configuration)
        {
            _logger = logger;
        }

        #region PRIVATE STUFF
        private Dictionary<string, int> ResolveColumnsCliente(SqlDataReader dr)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", dr.GetOrdinal("Id"));
            result.Add("Nome", dr.GetOrdinal("Nome"));
            result.Add("RagioneSociale", dr.GetOrdinal("RagioneSociale"));
            result.Add("Email", dr.GetOrdinal("Email"));
            result.Add("NumTelefono", dr.GetOrdinal("NumTelefono"));
            result.Add("Descrizione", dr.GetOrdinal("Descrizione"));
            result.Add("Indirizzo", dr.GetOrdinal("Indirizzo"));
            result.Add("Citta", dr.GetOrdinal("Citta"));
            result.Add("ZipOrPostalCode", dr.GetOrdinal("ZipOrPostalCode"));
            result.Add("Country", dr.GetOrdinal("Country"));
            result.Add("Latitudine", dr.GetOrdinal("Latitudine"));
            result.Add("Longitudine", dr.GetOrdinal("Longitudine"));
            result.Add("DataCreazione", dr.GetOrdinal("DataCreazione"));
            result.Add("DataProvisioning", dr.GetOrdinal("DataProvisioning"));
            result.Add("UrlRoute", dr.GetOrdinal("UrlRoute"));
            result.Add("OrarioApertura", dr.GetOrdinal("OrarioApertura"));
            result.Add("StorageContainer", dr.GetOrdinal("StorageContainer"));
            result.Add("IdTipologia", dr.GetOrdinal("IdTipologia"));
            result.Add("NomeTipologia", dr.GetOrdinal("NomeTipologia"));
            result.Add("DescrizioneTipologia", dr.GetOrdinal("DescrizioneTipologia"));
            return result;
        }

        private Dictionary<string, int> ResolveColumnsRichiestaRegistrazione(SqlDataReader dr)
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

        private async Task<ClienteDM> InternalReadClienteAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            ClienteDM result = new ClienteDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.Nome = dr.GetString(columns["Nome"]);
            result.RagioneSociale = dr.GetString(columns["RagioneSociale"]);
            result.Email = dr.GetString(columns["Email"]);
            result.NumTelefono = await dr.IsDBNullAsync(columns["NumTelefono"]) ? default(string) : dr.GetString(columns["NumTelefono"]);
            result.Descrizione = await dr.IsDBNullAsync(columns["Descrizione"]) ? default(string) : dr.GetString(columns["Descrizione"]);
            result.Indirizzo = dr.GetString(columns["Indirizzo"]);
            result.Citta = dr.GetString(columns["Citta"]);
            result.ZipOrPostalCode = await dr.IsDBNullAsync(columns["ZipOrPostalCode"]) ? default(string) : dr.GetString(columns["ZipOrPostalCode"]);
            result.Country = await dr.IsDBNullAsync(columns["Country"]) ? default(string) : dr.GetString(columns["Country"]);
            result.Latitudine = await dr.IsDBNullAsync(columns["Latitudine"]) ? default(double?) : dr.GetDouble(columns["Latitudine"]);
            result.Longitudine = await dr.IsDBNullAsync(columns["Longitudine"]) ? default(double?) : dr.GetDouble(columns["Longitudine"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.UrlRoute = dr.GetString(columns["UrlRoute"]);
            result.OrarioApertura = await dr.IsDBNullAsync(columns["OrarioApertura"]) ? null : JsonConvert.DeserializeObject<OrarioAperturaDM>(dr.GetString(columns["OrarioApertura"]));
            result.StorageContainer = dr.GetString(columns["StorageContainer"]);
            result.TipoCliente = new TipologiaClienteDM();
            result.TipoCliente.Id = dr.GetInt32(columns["IdTipologia"]);
            result.TipoCliente.Nome = dr.GetString(columns["NomeTipologia"]);
            result.TipoCliente.Descrizione = await dr.IsDBNullAsync(columns["DescrizioneTipologia"]) ? default(string) : dr.GetString(columns["DescrizioneTipologia"]);
            return result;
        }

        private async Task<RichiestaRegistrazioneDM> InternalReadRichiestaRegistrazioneAsync(SqlDataReader dr, Dictionary<string, int> columns)
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
        #endregion


        /// <summary>
        /// Crea un nuovo cliente sul DB NON provisioned, per cui non ancora visibile, e non confermato.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<(int idCliente, Guid correlationId)> CreateClienteAsync(NuovoClienteInputDM cliente)
        {
            (int idCliente, Guid correlationId) result;// = (idCliente: -1, richiestaReg: null);
            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.Output;
            SqlParameter parCorrelation = new SqlParameter("@pCorrelationId", SqlDbType.UniqueIdentifier);
            parCorrelation.Direction = ParameterDirection.Output;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = cliente.Nome;
                cmd.Parameters.Add("@pRagioneSociale", SqlDbType.NVarChar, 100).Value = cliente.RagioneSociale;
                cmd.Parameters.Add("@pEmail", SqlDbType.NVarChar, 100).Value = cliente.Email;
                cmd.Parameters.Add("@pNumTelefono", SqlDbType.VarChar, 50).Value = cliente.NumTelefono;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 1000).Value = cliente.Descrizione;
                cmd.Parameters.Add("@pIdTipologia", SqlDbType.Int).Value = cliente.IdTipologia;
                cmd.Parameters.Add("@pIndirizzo", SqlDbType.NVarChar, 250).Value = cliente.Indirizzo;
                cmd.Parameters.Add("@pCitta", SqlDbType.NVarChar, 100).Value = cliente.Citta;
                cmd.Parameters.Add("@pZipCode", SqlDbType.NVarChar, 10).Value = cliente.ZipOrPostalCode;
                cmd.Parameters.Add("@pCountry", SqlDbType.NVarChar, 100).Value = cliente.Country;
                cmd.Parameters.Add("@pLatitudine", SqlDbType.Float).Value = cliente.Latitudine;
                cmd.Parameters.Add("@pLongitudione", SqlDbType.Float).Value = cliente.Longitudine;
                cmd.Parameters.Add("@pUrlRoute", SqlDbType.VarChar, 200).Value = cliente.UrlRoute;
                cmd.Parameters.Add("@pStorageContainer", SqlDbType.NVarChar, 500).Value = cliente.StorageContainer;
                cmd.Parameters.Add("@pIdUserOwner", SqlDbType.VarChar, 100).Value = cliente.IdUserOwner;
                cmd.Parameters.Add(parId);
                cmd.Parameters.Add(parCorrelation);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                result.idCliente = (int)parId.Value;
                result.correlationId = (Guid)parCorrelation.Value;
            }
            return result;
        }


        public async Task CompensateCreateClienteAsync(int idCliente, Guid correlationId)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_Add_Undo]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pCorrelationId", SqlDbType.UniqueIdentifier).Value = correlationId;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
        /// <summary>
        /// Inserisce una nuova richiesta di registrazione per l'utente e ritorna il codice per la validazione dell'email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="correlationId"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public async Task<string> RichiestaRegistrazioneCreaAsync(string userName, Guid? correlationId, DateTime? expiration)
        {
            SqlParameter parCode = new SqlParameter("@pUserCode", SqlDbType.VarChar, 1000);
            parCode.Direction = ParameterDirection.Output;

            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[RichiestaRegistrazione_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUsername", SqlDbType.VarChar, 500).Value = userName;
                cmd.Parameters.Add("@pCorrelationId", SqlDbType.UniqueIdentifier).Value = correlationId;
                cmd.Parameters.Add("@pExpiration", SqlDbType.DateTime2).Value = expiration;
                cmd.Parameters.Add(parCode);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return parCode.Value as string;
            }
        }

        /// <summary>
        /// Cambia lo stato del Cliente da NotProvisioned a Provisioned
        /// </summary>
        /// <param name="provisioningToken"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task ConfermaProvisioningAsync(int idCliente, bool accountConfirmed)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_ConfermaProvisioning]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pAccountConfermato", SqlDbType.Bit).Value = accountConfirmed;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<ClienteDM> GetClienteByUrlRouteAsync(string urlRoute)
        {
            _logger.LogDebug($"Inizio GetClienteByUrlRouteAsync({urlRoute})");
            ClienteDM result = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_GetByUrlRoute]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUrlRoute", SqlDbType.VarChar, 205).Value = urlRoute;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        var columns = ResolveColumnsCliente(dr);
                        result = await InternalReadClienteAsync(dr, columns);
                    }
                    else
                    {
                        _logger.LogDebug($"Impossibile trovare il cliente con la UrlRoute: {urlRoute}");
                    }
                }
            }
            return result;
        }

        public async Task<ClienteDM> GetClienteByIdAsync(int idCliente)
        {
            ClienteDM result = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_GetById]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        var columns = ResolveColumnsCliente(dr);
                        result = await InternalReadClienteAsync(dr, columns);
                    }
                }
            }
            return result;
        }

        public async Task AggiornaAnagraficaClienteAsync(int idCliente, ClienteAnagraficaDM anagrafica)
        {
            if (idCliente != anagrafica.Id) { throw new ArgumentException("Bad Tenant"); }
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_AnagraficaSave]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = anagrafica.Id;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = anagrafica.Nome;
                cmd.Parameters.Add("@pRagioneSociale", SqlDbType.NVarChar, 100).Value = anagrafica.RagioneSociale;
                cmd.Parameters.Add("@pEmail", SqlDbType.NVarChar, 100).Value = anagrafica.Email;
                cmd.Parameters.Add("@pNumTelefono", SqlDbType.NVarChar, 50).Value = anagrafica.NumTelefono;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 1000).Value = anagrafica.Descrizione;
                cmd.Parameters.Add("@pIndirizzo", SqlDbType.NVarChar, 250).Value = anagrafica.Indirizzo;
                cmd.Parameters.Add("@pCitta", SqlDbType.NVarChar, 100).Value = anagrafica.Citta;
                cmd.Parameters.Add("@pPostalCode", SqlDbType.NVarChar, 10).Value = anagrafica.ZipOrPostalCode;
                cmd.Parameters.Add("@pCountry", SqlDbType.NVarChar, 100).Value = anagrafica.Country;
                cmd.Parameters.Add("@pLatitudine", SqlDbType.Float).Value = anagrafica.Latitudine;
                cmd.Parameters.Add("@pLongitudine", SqlDbType.Float).Value = anagrafica.Longitudine;
                cmd.Parameters.Add("@pUrlRoute", SqlDbType.VarChar, 205).Value = anagrafica.UrlRoute;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task AggiornaOrarioAperturaClienteAsync(int idCliente, OrarioAperturaDM orarioApertura)
        {
            string json = (orarioApertura != null) ? JsonConvert.SerializeObject(orarioApertura) : null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_OrarioAperturaSave]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pOrarioApertura", SqlDbType.VarChar, -1).Value = json;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<UrlValidationResultDM> CheckUrlRouteValidity(string urlRoute, int? idCliente = null)
        {
            using (var cn = GetConnection())
            {
                SqlParameter paramOut = new SqlParameter("@pResult", SqlDbType.Int);
                paramOut.Direction = ParameterDirection.Output;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_RouteValidate]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUrlRoute", SqlDbType.VarChar, 200).Value = urlRoute;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = (object)idCliente ?? DBNull.Value;
                cmd.Parameters.Add(paramOut);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return (UrlValidationResultDM)paramOut.Value;
            }
        }



        #region PREFERENZE
        public async Task<string> GetPreferenzaCliente(int idCliente, string key)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_Preferenze_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pKey", SqlDbType.VarChar, 100).Value = key;
                await cn.OpenAsync();
                return (string)await cmd.ExecuteScalarAsync();
            }
        }
        #endregion
    }
}
