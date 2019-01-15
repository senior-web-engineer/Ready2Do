using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class AppuntamentiRepository : BaseRepository, IAppuntamentiRepository
    {
        private readonly IConfiguration _config;

        public AppuntamentiRepository(ILogger<AppuntamentiRepository> logger, IConfiguration config) : base(config)
        {
            _config = config;
        }


        public async Task<AppuntamentoBaseDM> TakeAppuntamentoAsync(int idCliente, string userId, int idSchedule, int? idAbbonamento, string note, string nominativo, string payloadTimeoutManager)
        {
            AppuntamentoBaseDM result = null;
            using (var cn = GetConnection())
            {
                var retVal = new SqlParameter();
                retVal.Direction = ParameterDirection.ReturnValue;
                var parId = new SqlParameter("@pId", SqlDbType.Int);
                parId.Direction = ParameterDirection.Output;
                bool isConfermato = false;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Add]";
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 50).Value = userId;
                cmd.Parameters.Add("@pScheduleId", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIdAbbonamento", SqlDbType.Int).Value = idAbbonamento;
                cmd.Parameters.Add("@pNote", SqlDbType.NVarChar, 1000).Value = note;
                cmd.Parameters.Add("@pNominativo", SqlDbType.NVarChar, 200).Value = nominativo;
                cmd.Parameters.Add("@pTimeoutManagerPayload", SqlDbType.NVarChar, -1).Value = payloadTimeoutManager;
                cmd.Parameters.Add(parId);
                cmd.Parameters.Add(retVal);
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        isConfermato = dr.GetString(0).Equals("Confermato", StringComparison.InvariantCultureIgnoreCase);
                        await dr.NextResultAsync();
                        if (await dr.ReadAsync())
                        {
                            if (isConfermato)
                            {
                                result = await ReadAppuntamentoAsync(dr, GetAppuntamentoColumns(dr));
                            }
                            else
                            {
                                result = await ReadAppuntamentoDaConfermareAsync(dr, GetAppuntamentoDaConfermareColumns(dr));
                            }
                        }
                    }
                }
            }
            return result;
        }

        public async Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento)
        {
            using (var cn = GetConnection())
            {
                var retVal = new SqlParameter();
                retVal.Direction = ParameterDirection.ReturnValue;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Delete]";
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdAppuntamento", SqlDbType.Int).Value = idAppuntamento;
                cmd.Parameters.Add(retVal);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<string> CancelAppuntamentoDaConfermareAsync(int idCliente, int idAppuntamentoDaConfermare)
        {
            string result = null;
            using (var cn = GetConnection())
            {
                var retVal = new SqlParameter();
                retVal.Direction = ParameterDirection.ReturnValue;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[AppuntamentiDaConfermare_Delete]";
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdAppuntamentoDaConfermare", SqlDbType.Int).Value = idAppuntamentoDaConfermare;
                cmd.Parameters.Add(retVal);
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                        result = dr.GetString(dr.GetOrdinal("TimeoutManagerPayload"));
                }
            }
            return result;
        }

        public async Task<AppuntamentoDM> GetAppuntamentoAsync(int idCliente, int idSchedule, int idAppuntamento)
        {
            AppuntamentoDM result = null;
            Dictionary<string, int> columnsAppuntamento = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIdAppuntamento", SqlDbType.Int).Value = idAppuntamento;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    columnsAppuntamento = GetAppuntamentoColumns(dr);
                    if (await dr.ReadAsync())
                    {
                        result = await ReadAppuntamentoAsync(dr, columnsAppuntamento);
                    }
                }
            }
            return result;
        }
        public async Task<AppuntamentoDaConfermareDM> GetAppuntamentoDaConfermareAsync(int idCliente, int idSchedule, int idAppuntamentoDaConfermare)
        {
            AppuntamentoDaConfermareDM result = null;
            Dictionary<string, int> columnsAppuntamento = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[AppuntamentiDaConfermare_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIdAppuntamentoDaConf", SqlDbType.Int).Value = idAppuntamentoDaConfermare;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    columnsAppuntamento = GetAppuntamentoColumns(dr);
                    if (await dr.ReadAsync())
                    {
                        result = await ReadAppuntamentoDaConfermareAsync(dr, columnsAppuntamento);
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<AppuntamentoBaseDM>> GetAllAppuntamenti(int idCliente, int idSchedule, bool includiConfermati = true,
                                                                              bool includiNonConfermati = true, bool includeDeleted = false)
        {
            List<AppuntamentoBaseDM> result = new List<AppuntamentoBaseDM>();
            if (!includiConfermati && !includiNonConfermati) return result;
            Dictionary<string, int> columnsAppDaConfermare = null, columnsAppuntamento = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Lista4Schedule]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIncludeConfermati", SqlDbType.Bit).Value = includiConfermati;
                cmd.Parameters.Add("@pIncludeNonConfermati", SqlDbType.Bit).Value = includiNonConfermati;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows && includiConfermati)
                    {
                        columnsAppuntamento = GetAppuntamentoColumns(dr);
                    }
                    else if (dr.HasRows && includiNonConfermati)
                    {
                        columnsAppDaConfermare = GetAppuntamentoDaConfermareColumns(dr);
                    }
                    while (await dr.ReadAsync())
                    {
                        if (includiConfermati)
                        {
                            result.Add(await ReadAppuntamentoAsync(dr, columnsAppuntamento));
                        }
                        else
                        {
                            result.Add(await ReadAppuntamentoDaConfermareAsync(dr, columnsAppDaConfermare));
                        }
                    }
                    if (includiConfermati && includiNonConfermati)
                    {
                        await dr.NextResultAsync();
                        columnsAppDaConfermare = GetAppuntamentoDaConfermareColumns(dr);
                        while (await dr.ReadAsync())
                        {
                            result.Add(await ReadAppuntamentoDaConfermareAsync(dr, columnsAppDaConfermare));
                        }
                    }
                }
            }
            return result;
        }

        public async Task<AppuntamentoDM> GetAppuntamentoForUserAsync(int idCliente, int idSchedule, string userId)
        {
            AppuntamentoDM result = null;
            Dictionary<string, int> columnsAppuntamento = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Get4User]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = false;

                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    columnsAppuntamento = GetAppuntamentoColumns(dr);
                    if (await dr.ReadAsync())
                    {
                        result = await ReadAppuntamentoAsync(dr, columnsAppuntamento);
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<AppuntamentoDaConfermareDM>> GetAppuntamentoDaConfermareForUserAsync(int idCliente, int idSchedule, string userId, bool includeDeleted = false)
        {
            List<AppuntamentoDaConfermareDM> result = new List<AppuntamentoDaConfermareDM>();
            Dictionary<string, int> columnsAppuntamentoDaConf = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[AppuntamentiDaConfermare_Get4User]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    columnsAppuntamentoDaConf = GetAppuntamentoDaConfermareColumns(dr);
                    while (await dr.ReadAsync())
                    {
                        result.Add(await ReadAppuntamentoDaConfermareAsync(dr, columnsAppuntamentoDaConf));
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<AppuntamentoDM>> GetAppuntamentiUtenteAsync(string userId, int pageNumber = 1, int pageSize = 25,
                                                                                        DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                        string sortBy = "DataOraInizio", bool sortAscending = true,
                                                                                        bool includiCancellati = false)
        {
            return await InternalGetAppuntamentiUtenteAsync(null, userId, pageNumber, pageSize, dtInizioSchedule, dtFineSchedule, sortBy, sortAscending);

        }



        public async Task<IEnumerable<AppuntamentoDM>> GetAppuntamentiUtenteAsync(int idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                        DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                        string sortBy = "DataOraInizio", bool sortAscending = true,
                                                                                        bool includiCancellati = false)
        {
            return await InternalGetAppuntamentiUtenteAsync(idCliente, userId, pageNumber, pageSize, dtInizioSchedule, dtFineSchedule, sortBy, sortAscending);
        }

        public async Task<IEnumerable<AppuntamentoDaConfermareDM>> GetAppuntamentiDaConfermareUtenteAsync(string userId, int pageNumber = 1, int pageSize = 25,
                                                                                       DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                       string sortBy = "DataOraInizio", bool sortAscending = true,
                                                                                       bool includiCancellati = false)
        {
            return await InternalGetAppuntamentiDaConfermareUtenteAsync(null, userId, pageNumber, pageSize, dtInizioSchedule, dtFineSchedule, sortBy, sortAscending);

        }



        public async Task<IEnumerable<AppuntamentoDaConfermareDM>> GetAppuntamentiDaConfermareUtenteAsync(int idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                        DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                        string sortBy = "DataOraInizio", bool sortAscending = true,
                                                                                        bool includiCancellati = false)
        {
            return await InternalGetAppuntamentiDaConfermareUtenteAsync(idCliente, userId, pageNumber, pageSize, dtInizioSchedule, dtFineSchedule, sortBy, sortAscending);
        }

        public async Task<IEnumerable<WaitListRegistration>> GetWaitListRegistrationsAsync(int idCliente, int idSchedule, bool includeConverted = false, bool includeDeleted = false)
        {
            List<WaitListRegistration> result = new List<WaitListRegistration>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ListeAttesa_List4Schedule]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIncludeConverted", SqlDbType.Bit).Value = includeConverted;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        var columns = GetListaAttesaColumns(dr);
                        var colUser = ClientiUtentiRepository.GetColumnsOrdinals(dr, new Dictionary<string, string> {
                                { "DataCreazione", "DataCreazioneUtente" },
                                { "DataCancellazione","DataCancellazioneUtete" } });
                        while (await dr.ReadAsync())
                        {
                            result.Add(await ReadListaAttesaAsync(dr,columns, colUser));
                        }
                    }
                }
            }
            return result;
        }


        #region STATIC STUFF (DATASET READ)
        internal static async Task<AppuntamentoDM> ReadAppuntamentoAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            return new AppuntamentoDM()
            {
                DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]),
                DataCreazione = dr.GetDateTime(columns["DataPrenotazione"]),
                Id = dr.GetInt32(columns["Id"]),
                IdCliente = dr.GetInt32(columns["IdCliente"]),
                ScheduleId = dr.GetInt32(columns["ScheduleId"]),
                UserId = await dr.IsDBNullAsync(columns["UserId"]) ? null : dr.GetString(columns["UserId"]),
                IdAbbonamento = await dr.IsDBNullAsync(columns["IdAbbonamento"]) ? default(int?) : dr.GetInt32(columns["IdAbbonamento"]),
                Nominativo = await dr.IsDBNullAsync(columns["Nominativo"]) ? null : dr.GetString(columns["Nominativo"]),
                Note = await dr.IsDBNullAsync(columns["Note"]) ? null : dr.GetString(columns["Note"])
            };
        }

        internal static async Task<AppuntamentoDaConfermareDM> ReadAppuntamentoDaConfermareAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            return new AppuntamentoDaConfermareDM()
            {
                Id = dr.GetInt32(columns["Id"]),
                IdCliente = dr.GetInt32(columns["IdCliente"]),
                ScheduleId = dr.GetInt32(columns["ScheduleId"]),
                UserId = await dr.IsDBNullAsync(columns["UserId"]) ? null : dr.GetString(columns["UserId"]),
                DataCreazione = dr.GetDateTime(columns["DataCreazione"]),
                DataExpiration = dr.GetDateTime(columns["DataExpiration"]),
                DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]),
                DataEsito = await dr.IsDBNullAsync(columns["DataEsito"]) ? default(DateTime?) : dr.GetDateTime(columns["DataEsito"]),
                IdAppuntamentoConfermato = await dr.IsDBNullAsync(columns["IdAppuntamento"]) ? default(int?) : dr.GetInt32(columns["IdAppuntamento"]),
                MotivoRifiuto = await dr.IsDBNullAsync(columns["MotivoRifiuto"]) ? null : dr.GetString(columns["MotivoRifiuto"])
            };
        }

        internal static async Task<WaitListRegistration> ReadListaAttesaAsync(SqlDataReader dr, Dictionary<string,int> columns, Dictionary<string, int> userColumns)
        {
            WaitListRegistration result = new WaitListRegistration();
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.IdSchedule = dr.GetInt32(columns["ScheduleId"]);
            result.IdAbbonamento = dr.GetInt32(columns["IdAbbonamento"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.DataScadenza = dr.GetDateTime(columns["DataScadenza"]);
            result.DataConversione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataConversione"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            result.CausaleCancellazione = await dr.IsDBNullAsync(columns["CausaleCancellazione"]) ? default(byte?) : dr.GetByte(columns["CausaleCancellazione"]);
            result.IdSchedule = dr.GetInt32(columns["ScheduleId"]);
            result.User = await ClientiUtentiRepository.ReadUtenteClienteAsync<UtenteClienteDM>(dr, userColumns);
            return result;
        }

        internal static Dictionary<string, int> GetAppuntamentoColumns(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazione"));
            result["DataPrenotazione"] = dr.GetOrdinal(getColumnName("DataPrenotazione"));
            result["Id"] = dr.GetOrdinal(getColumnName("Id"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdCliente"));
            result["ScheduleId"] = dr.GetOrdinal(getColumnName("ScheduleId"));
            result["UserId"] = dr.GetOrdinal(getColumnName("UserId"));
            result["IdAbbonamento"] = dr.GetOrdinal(getColumnName("IdAbbonamento"));
            result["Nominativo"] = dr.GetOrdinal(getColumnName("Nominativo"));
            result["Note"] = dr.GetOrdinal(getColumnName("Note"));
            return result;
        }

        internal static Dictionary<string, int> GetAppuntamentoDaConfermareColumns(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["Id"] = dr.GetOrdinal(getColumnName("Id"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdCliente"));
            result["ScheduleId"] = dr.GetOrdinal(getColumnName("ScheduleId"));
            result["UserId"] = dr.GetOrdinal(getColumnName("UserId"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazione"));

            result["DataCreazione"] = dr.GetOrdinal(getColumnName("DataCreazione"));
            result["DataExpiration"] = dr.GetOrdinal(getColumnName("DataExpiration"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazione"));
            result["DataEsito"] = dr.GetOrdinal(getColumnName("DataEsito"));
            result["IdAppuntamento"] = dr.GetOrdinal(getColumnName("IdAppuntamento"));
            result["MotivoRifiuto"] = dr.GetOrdinal(getColumnName("MotivoRifiuto"));
            return result;
        }

        internal static Dictionary<string, int> GetListaAttesaColumns(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["Id"] = dr.GetOrdinal(getColumnName("Id"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdCliente"));
            result["ScheduleId"] = dr.GetOrdinal(getColumnName("ScheduleId"));
            result["UserId"] = dr.GetOrdinal(getColumnName("UserId"));
            result["IdAbbonamento"] = dr.GetOrdinal(getColumnName("IdAbbonamento"));
            result["DataCreazione"] = dr.GetOrdinal(getColumnName("DataCreazione"));
            result["DataScadenza"] = dr.GetOrdinal(getColumnName("DataScadenza"));
            result["DataConversione"] = dr.GetOrdinal(getColumnName("DataConversione"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazione"));
            result["CausaleCancellazione"] = dr.GetOrdinal(getColumnName("CausaleCancellazione"));
            return result;
        }


        #endregion

        #region PRIVATE STAFF

        private async Task<IEnumerable<AppuntamentoDM>> InternalGetAppuntamentiUtenteAsync(int? idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                string sortBy = "DataOraInizio", bool sortAscending = true,
                                                                                bool includiCancellati = false)
        {
            List<AppuntamentoDM> result = new List<AppuntamentoDM>();
            using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Lista4Utente]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pScheduleStartDate", SqlDbType.DateTime2).Value = dtInizioSchedule;
                cmd.Parameters.Add("@pScheduleEndDate", SqlDbType.DateTime2).Value = dtFineSchedule;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includiCancellati;
                cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
                cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@pSortColumn", SqlDbType.VarChar, 50).Value = sortBy;
                cmd.Parameters.Add("@pOrderAscending", SqlDbType.Bit).Value = sortAscending;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    var columnsAppuntamento = GetAppuntamentoColumns(dr);
                    while (await dr.ReadAsync())
                    {
                        result.Add(await ReadAppuntamentoAsync(dr, columnsAppuntamento));
                    }
                }
                return result;
            }
        }

        private async Task<IEnumerable<AppuntamentoDaConfermareDM>> InternalGetAppuntamentiDaConfermareUtenteAsync(int? idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                   DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                   string sortBy = "DataOraInizio", bool sortAscending = true,
                                                                                   bool includiCancellati = false)
        {
            List<AppuntamentoDaConfermareDM> result = new List<AppuntamentoDaConfermareDM>();
            using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[AppuntamentiDaConfermare_Lista4UtenteCliente]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pScheduleStartDate", SqlDbType.DateTime2).Value = dtInizioSchedule;
                cmd.Parameters.Add("@pScheduleEndDate", SqlDbType.DateTime2).Value = dtFineSchedule;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includiCancellati;
                cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
                cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@pSortColumn", SqlDbType.VarChar, 50).Value = sortBy;
                cmd.Parameters.Add("@pOrderAscending", SqlDbType.Bit).Value = sortAscending;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    var columnsAppuntamento = GetAppuntamentoDaConfermareColumns(dr);
                    while (await dr.ReadAsync())
                    {
                        result.Add(await ReadAppuntamentoDaConfermareAsync(dr, columnsAppuntamento));
                    }
                }
                return result;
            }
        }
        #endregion

    }
}
