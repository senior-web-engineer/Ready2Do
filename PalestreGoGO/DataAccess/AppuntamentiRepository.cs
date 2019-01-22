﻿using Microsoft.Extensions.Configuration;
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
                            var columnsUser = ClientiUtentiRepository.GetColumnsOrdinals(dr, new Dictionary<string, string>()
                            {

                            });
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



        public async Task<IEnumerable<AppuntamentoDaConfermareDM>> GetAppuntamentiDaConfermareScheduleAsync(int idCliente, int idSchedule, bool includeDeleted = false)
        {
            List<AppuntamentoDaConfermareDM> result = new List<AppuntamentoDaConfermareDM>();
            using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[AppuntamentiDaConfermare_Lista4Schedule]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        var columnsAppuntamento = GetAppuntamentoDaConfermareColumns(dr);
                        var columnsUser = ClientiUtentiRepository.GetColumnsOrdinals(dr);
                        while (await dr.ReadAsync())
                        {
                            result.Add(await ReadAppuntamentoDaConfermareAsync(dr, columnsAppuntamento, columnsUser));
                        }
                    }
                }
                return result;
            }
        }


        public async Task<IEnumerable<AppuntamentoDM>> GetAppuntamentiScheduleAsync(int idCliente, int idSchedule, bool includeDeleted = false)
        {
            List<AppuntamentoDM> result = new List<AppuntamentoDM>();
            Dictionary<string, int> columnsAppuntamento = null;
            Dictionary<string, int> columnsUser = null;
            AppuntamentoDM item;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Lista4Schedule]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        columnsAppuntamento = GetAppuntamentoColumns(dr);
                        columnsUser = ClientiUtentiRepository.GetColumnsOrdinals(dr);
                        while (await dr.ReadAsync())
                        {
                            item = await ReadAppuntamentoAsync(dr, columnsAppuntamento, null, null, null, columnsUser);
                            result.Add(item);
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
            Dictionary<string, int> columnsSchedule = null;
            Dictionary<string, int> columnsTipoLezione = null;
            Dictionary<string, int> columnsLocation = null;
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
                    if (dr.HasRows)
                    {
                        columnsAppuntamento = GetAppuntamentoColumns(dr);
                        columnsSchedule = SchedulesRepository.GetSchedulesColumns(dr);
                        columnsTipoLezione = TipologieLezioniRepository.GetTipologieLezioniColumnsOrdinals(dr);
                        columnsLocation = LocationsRepository.GetLocationColumnsOrdinals(dr);
                        if (await dr.ReadAsync())
                        {
                            result = await ReadAppuntamentoAsync(dr, columnsAppuntamento, columnsSchedule, columnsLocation, columnsTipoLezione, null);
                        }
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

        public async Task<IEnumerable<WaitListRegistrationDM>> GetWaitListRegistrationsScheduleAsync(int idCliente, int idSchedule, string userId = null, bool
                                                                                                   includeConverted = false, bool includeDeleted = false)
        {
            List<WaitListRegistrationDM> result = new List<WaitListRegistrationDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ListeAttesa_List4Schedule]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdSchedule", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
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
                            result.Add(await ReadListaAttesaAsync(dr, columns, colUser));
                        }
                    }
                }
            }
            return result;
        }


        #region STATIC STUFF (DATASET READ)
        internal static async Task<AppuntamentoDM> ReadAppuntamentoAsync(SqlDataReader dr, Dictionary<string, int> columns, Dictionary<string, int> columnsSchedule = null,
                                                                         Dictionary<string, int> columnsLocation = null,
                                                                         Dictionary<string, int> columnsTipoLez = null, Dictionary<string, int> columnsUser = null)
        {
            var result = new AppuntamentoDM();
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            result.DataCreazione = dr.GetDateTime(columns["DataPrenotazione"]);
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.ScheduleId = dr.GetInt32(columns["ScheduleId"]);
            result.UserId = await dr.IsDBNullAsync(columns["UserId"]) ? null : dr.GetString(columns["UserId"]);
            result.IdAbbonamento = await dr.IsDBNullAsync(columns["IdAbbonamento"]) ? default(int?) : dr.GetInt32(columns["IdAbbonamento"]);
            result.Nominativo = await dr.IsDBNullAsync(columns["Nominativo"]) ? null : dr.GetString(columns["Nominativo"]);
            result.Note = await dr.IsDBNullAsync(columns["Note"]) ? null : dr.GetString(columns["Note"]);
            if (columnsSchedule != null && columnsLocation != null && columnsTipoLez != null)
            {
                result.Schedule = await SchedulesRepository.InternalReadScheduleAsync(dr, columnsSchedule, columnsTipoLez, columnsLocation);
            }
            if (columnsUser != null)
            {
                result.User = columnsUser != null ? await ClientiUtentiRepository.ReadUtenteClienteAsync<UtenteClienteDM>(dr, columnsUser) : null;
            }
            return result;
        }

        internal static async Task<AppuntamentoDaConfermareDM> ReadAppuntamentoDaConfermareAsync(SqlDataReader dr, Dictionary<string, int> columns,
                                                                                                 Dictionary<string, int> columnsUser = null,
                                                                                                 Dictionary<string, int> columnsSchedules = null,
                                                                                                 Dictionary<string, int> columnsTipoLez = null,
                                                                                                 Dictionary<string, int> columnsLocation = null)
        {
            var result = new AppuntamentoDaConfermareDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.ScheduleId = dr.GetInt32(columns["ScheduleId"]);
            result.UserId = await dr.IsDBNullAsync(columns["UserId"]) ? null : dr.GetString(columns["UserId"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.DataExpiration = dr.GetDateTime(columns["DataExpiration"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            result.DataEsito = await dr.IsDBNullAsync(columns["DataEsito"]) ? default(DateTime?) : dr.GetDateTime(columns["DataEsito"]);
            result.IdAppuntamentoConfermato = await dr.IsDBNullAsync(columns["IdAppuntamento"]) ? default(int?) : dr.GetInt32(columns["IdAppuntamento"]);
            result.MotivoRifiuto = await dr.IsDBNullAsync(columns["MotivoRifiuto"]) ? null : dr.GetString(columns["MotivoRifiuto"]);
            if ((columnsUser?.Count ?? 0) > 0)
            {
                result.User = await ClientiUtentiRepository.ReadUtenteClienteAsync<UtenteClienteDM>(dr, columnsUser);
            }
            if (((columnsSchedules?.Count ?? 0) > 0) && ((columnsTipoLez?.Count ?? 0) > 0) && ((columnsLocation?.Count ?? 0) > 0))
            {
                result.Schedule = await SchedulesRepository.InternalReadScheduleAsync(dr, columnsSchedules, columnsTipoLez, columnsLocation);
            }
            return result;
        }

        internal static async Task<WaitListRegistrationDM> ReadListaAttesaAsync(SqlDataReader dr, Dictionary<string, int> columns, Dictionary<string, int> columnsUser,
                                                                                Dictionary<string, int> columnsSchedules = null, Dictionary<string, int> columnsTipoLez = null,
                                                                                Dictionary<string, int> columnsLocation = null)
        {
            WaitListRegistrationDM result = new WaitListRegistrationDM();
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
            if ((columnsUser?.Count ?? 0) > 0)
            {
                result.User = await ClientiUtentiRepository.ReadUtenteClienteAsync<UtenteClienteDM>(dr, columnsUser);
            }
            if (((columnsSchedules?.Count ?? 0) > 0) && ((columnsTipoLez?.Count ?? 0) > 0) && ((columnsLocation?.Count ?? 0) > 0))
            {
                result.Schedule = await SchedulesRepository.InternalReadScheduleAsync(dr, columnsSchedules, columnsTipoLez, columnsLocation);
            }
            return result;
        }

        internal static Dictionary<string, int> GetAppuntamentoColumns(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazioneAppuntamenti"));
            result["DataPrenotazione"] = dr.GetOrdinal(getColumnName("DataPrenotazioneAppuntamenti"));
            result["Id"] = dr.GetOrdinal(getColumnName("IdAppuntamenti"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdClienteAppuntamenti"));
            result["ScheduleId"] = dr.GetOrdinal(getColumnName("ScheduleIdAppuntamenti"));
            result["UserId"] = dr.GetOrdinal(getColumnName("UserIdAppuntamenti"));
            result["IdAbbonamento"] = dr.GetOrdinal(getColumnName("IdAbbonamentoAppuntamenti"));
            result["Nominativo"] = dr.GetOrdinal(getColumnName("NominativoAppuntamenti"));
            result["Note"] = dr.GetOrdinal(getColumnName("NoteAppuntamenti"));
            return result;
        }

        internal static Dictionary<string, int> GetAppuntamentoDaConfermareColumns(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["Id"] = dr.GetOrdinal(getColumnName("IdAppuntamentiDaConfermare"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdClienteAppuntamentiDaConfermare"));
            result["ScheduleId"] = dr.GetOrdinal(getColumnName("ScheduleIdAppuntamentiDaConfermare"));
            result["UserId"] = dr.GetOrdinal(getColumnName("UserIdAppuntamentiDaConfermare"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazioneAppuntamentiDaConfermare"));

            result["DataCreazione"] = dr.GetOrdinal(getColumnName("DataCreazioneAppuntamentiDaConfermare"));
            result["DataExpiration"] = dr.GetOrdinal(getColumnName("DataExpirationAppuntamentiDaConfermare"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazioneAppuntamentiDaConfermare"));
            result["DataEsito"] = dr.GetOrdinal(getColumnName("DataEsitoAppuntamentiDaConfermare"));
            result["IdAppuntamento"] = dr.GetOrdinal(getColumnName("IdAppuntamentoAppuntamentiDaConfermare"));
            result["MotivoRifiuto"] = dr.GetOrdinal(getColumnName("MotivoRifiutoAppuntamentiDaConfermare"));
            return result;
        }

        internal static Dictionary<string, int> GetListaAttesaColumns(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["Id"] = dr.GetOrdinal(getColumnName("IdListeAttesa"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdClienteListeAttesa"));
            result["ScheduleId"] = dr.GetOrdinal(getColumnName("ScheduleIdListeAttesa"));
            result["UserId"] = dr.GetOrdinal(getColumnName("UserIdListeAttesa"));
            result["IdAbbonamento"] = dr.GetOrdinal(getColumnName("IdAbbonamentoListeAttesa"));
            result["DataCreazione"] = dr.GetOrdinal(getColumnName("DataCreazioneListeAttesa"));
            result["DataScadenza"] = dr.GetOrdinal(getColumnName("DataScadenzaListeAttesa"));
            result["DataConversione"] = dr.GetOrdinal(getColumnName("DataConversioneListeAttesa"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazioneListeAttesa"));
            result["CausaleCancellazione"] = dr.GetOrdinal(getColumnName("CausaleCancellazioneListeAttesa"));
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
                    var columnsUser = ClientiUtentiRepository.GetColumnsOrdinals(dr, new Dictionary<string, string>()
                    {
                        { "DataCancellazione","DataCancellazioneUtente" },
                        {"DataCreazione","DataCreazioneUtente" }
                    });
                    while (await dr.ReadAsync())
                    {
                        result.Add(await ReadAppuntamentoAsync(dr, columnsAppuntamento, columnsUser));
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
                    var columnsUser = ClientiUtentiRepository.GetColumnsOrdinals(dr, new Dictionary<string, string>()
                    {
                        { "DataCancellazione","DataCancellazioneUtente" },
                        {"DataCreazione","DataCreazioneUtente" }
                    });
                    while (await dr.ReadAsync())
                    {
                        result.Add(await ReadAppuntamentoDaConfermareAsync(dr, columnsAppuntamento, columnsUser));
                    }
                }
                return result;
            }
        }
        #endregion

    }
}
