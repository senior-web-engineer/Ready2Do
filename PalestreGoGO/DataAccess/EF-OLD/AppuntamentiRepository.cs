using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using PalestreGoGo.DataModel.Exceptions;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        private async Task<AppuntamentoDM> InternalReadAppuntamentoAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            return new AppuntamentoDM()
            {
                DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]),
                DataCreazione = dr.GetDateTime(columns["DataPrenotazione"]),
                Id = dr.GetInt32(columns["Id")),
                IdCliente = dr.GetInt32(columns["IdCliente"]),
                ScheduleId = dr.GetInt32(columns["ScheduleId"]),
                UserId = await dr.IsDBNullAsync(columns["UserId"]) ? null : dr.GetString(columns["UserId"]),
                IdAbbonamento = await dr.IsDBNullAsync(columns["IdAbbonamento"]) ? default(int?) : dr.GetInt32(columns["IdAbbonamento"]),
                Nominativo = await dr.IsDBNullAsync(columns["Nominativo"]) ? null : dr.GetString(columns["Nominativo"]),
                Note = await dr.IsDBNullAsync(columns["Note"]) ? null : dr.GetString(columns["Note"])
            };
        }

        private async Task<AppuntamentoDaConfermareDM> InternalReadAppuntamentoDaConfermareAsync(SqlDataReader dr, Dictionary<string, int> columns)
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

        private Dictionary<string, int> GetAppuntamentoColumns(SqlDataReader dr)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result["DataCancellazione"] = dr.GetOrdinal("DataCancellazione");
            result["DataPrenotazione"] = dr.GetOrdinal("DataPrenotazione");
            result["Id"] = dr.GetOrdinal("Id");
            result["IdCliente"] = dr.GetOrdinal("IdCliente");
            result["ScheduleId"] = dr.GetOrdinal("ScheduleId");
            result["UserId"] = dr.GetOrdinal("UserId");
            result["IdAbbonamento"] = dr.GetOrdinal("IdAbbonamento");
            result["Nominativo"] = dr.GetOrdinal("Nominativo");
            result["Note"] = dr.GetOrdinal("Note");
            return result;
        }

        private Dictionary<string, int> GetAppuntamentoDaConfermareColumns(SqlDataReader dr)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result["Id"] = dr.GetOrdinal("Id");
            result["IdCliente"] = dr.GetOrdinal("IdCliente");
            result["ScheduleId"] = dr.GetOrdinal("ScheduleId");
            result["UserId"] = dr.GetOrdinal("UserId");
            result["DataCancellazione"] = dr.GetOrdinal("DataCancellazione");

            result["DataCreazione"] = dr.GetOrdinal("DataCreazione");
            result["DataExpiration"] = dr.GetOrdinal("DataExpiration");
            result["DataCancellazione"] = dr.GetOrdinal("DataCancellazione");
            result["DataEsito"] = dr.GetOrdinal("DataEsito");
            result["IdAppuntamento"] = dr.GetOrdinal("IdAppuntamento");
            result["MotivoRifiuto"] = dr.GetOrdinal("MotivoRifiuto");
            return result;
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
                                result = await InternalReadAppuntamentoAsync(dr, GetAppuntamentoColumns(dr));
                            }
                            else
                            {
                                result = await InternalReadAppuntamentoDaConfermareAsync(dr, GetAppuntamentoDaConfermareColumns(dr));
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


        public async Task<IEnumerable<AppuntamentoBaseDM>> GetAllAppuntamenti(int idCliente, int idSchedule, bool includiConfermati, bool includiNonConfermati, bool includeDeleted);
        {
            using (var cn = GetConnection())
            {

        var appuntamenti = _context.Appuntamenti.Where(a => a.Id.Equals(idSchedule) && a.IdCliente.Equals(idCliente));
            return appuntamenti;
        }

    public async Task<Appuntamenti> GetAppuntamentoAsync(int idCliente, int idAppuntamento)
    {
        return await _context.Appuntamenti.Where(a => a.IdCliente.Equals(idCliente) && a.Id.Equals(idAppuntamento)).FirstOrDefaultAsync();
    }

    public async Task<Appuntamenti> GetAppuntamentoForScheduleAsync(int idCliente, int idSchedule, string userId)
    {
        return await _context.Appuntamenti.Where(a => a.IdCliente.Equals(idCliente) && a.ScheduleId.Equals(idSchedule) && a.UserId.Equals(userId)).FirstOrDefaultAsync();
    }


    /// <summary>
    /// Ritorna tutti gli appuntamenti per un utente per tutti i clienti
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="includePast"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UtenteClienteAppuntamentoDM>> GetAppuntamentiUtenteAsync(string userId, int pageNumber = 1, int pageSize = 25,
                                                            DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                            string sortBy = "DataOraInizio", bool sortAscending = true)
    {
        return await InternalGetAppuntamentiUtenteAsync(null, userId, pageNumber, pageSize, dtInizioSchedule, dtFineSchedule, sortBy, sortAscending);

    }



    public async Task<IEnumerable<UtenteClienteAppuntamentoDM>> GetAppuntamentiUtenteAsync(int idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                    DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                    string sortBy = "DataOraInizio", bool sortAscending = true)
    {
        return await InternalGetAppuntamentiUtenteAsync(idCliente, userId, pageNumber, pageSize, dtInizioSchedule, dtFineSchedule, sortBy, sortAscending);
    }

    private async Task<IEnumerable<UtenteClienteAppuntamentoDM>> InternalGetAppuntamentiUtenteAsync(int? idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                string sortBy = "DataOraInizio", bool sortAscending = true)
    {
        UtenteClienteAppuntamentoDM item;
        List<UtenteClienteAppuntamentoDM> result = new List<UtenteClienteAppuntamentoDM>();
        using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
        {
            var cmd = cn.CreateCommand();
            cmd.CommandText = "[dbo].[Clienti_Utenti_AppuntamentiList]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
            cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
            cmd.Parameters.Add("@pScheduleStartDate", SqlDbType.DateTime2).Value = dtInizioSchedule;
            cmd.Parameters.Add("@pScheduleEndDate", SqlDbType.DateTime2).Value = dtFineSchedule;
            cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
            cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
            cmd.Parameters.Add("@pSortColumn", SqlDbType.VarChar, 50).Value = sortBy;
            cmd.Parameters.Add("@pOrderAscending", SqlDbType.Bit).Value = sortAscending;
            await cn.OpenAsync();
            using (var dr = await cmd.ExecuteReaderAsync())
            {
                Dictionary<string, int> colsApp, colsTipoLez, colsSched, colsLocat;
                colsApp = GetColumnsAppuntamento(dr);
                colsLocat = GetColumnsLocation(dr);
                colsSched = GetColumnsSchedule(dr);
                colsTipoLez = GetColumnsTipologiaLezione(dr);
                while (await dr.ReadAsync())
                {
                    item = InternalReadAppuntamento(dr, colsApp);
                    item.Schedule = InternalReadSchedule(dr, colsSched);
                    item.Schedule.Location = InternalReadLocation(dr, colsLocat);
                    item.Schedule.TipologiaLezione = InternalReadTipologiaLezione(dr, colsTipoLez);
                    result.Add(item);
                }
            }
            return result;
        }
    }


    private Dictionary<string, int> GetColumnsAppuntamento(SqlDataReader reader)
    {
        Dictionary<string, int> colsAppuntamenti = new Dictionary<string, int>();
        colsAppuntamenti["Id"] = reader.GetOrdinal("Id");
        colsAppuntamenti["IdCliente"] = reader.GetOrdinal("IdCliente");
        colsAppuntamenti["DataPrenotazione"] = reader.GetOrdinal("DataPrenotazione");
        colsAppuntamenti["DataCancellazione"] = reader.GetOrdinal("DataCancellazione");
        colsAppuntamenti["IdAbbonamento"] = reader.GetOrdinal("IdAbbonamento");
        colsAppuntamenti["Nominativo"] = reader.GetOrdinal("Nominativo");
        colsAppuntamenti["Note"] = reader.GetOrdinal("Note");
        colsAppuntamenti["ScheduleId"] = reader.GetOrdinal("ScheduleId");
        return colsAppuntamenti;
    }

    private Dictionary<string, int> GetColumnsSchedule(SqlDataReader reader)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        result["ScheduleId"] = reader.GetOrdinal("ScheduleId");
        result["IdCliente"] = reader.GetOrdinal("IdCliente");
        result["CancellabileFinoAl"] = reader.GetOrdinal("CancellabileFinoAl");
        result["DataCancellazione"] = reader.GetOrdinal("DataCancellazione");
        result["DataChiusuraIscrizioni"] = reader.GetOrdinal("DataChiusuraIscrizioni");
        result["DataOraInizio"] = reader.GetOrdinal("DataOraInizio");
        result["IdLocation"] = reader.GetOrdinal("IdLocation");
        result["IdTipoLezione"] = reader.GetOrdinal("IdTipoLezione");
        result["Istruttore"] = reader.GetOrdinal("Istruttore");
        result["NoteSchedule"] = reader.GetOrdinal("NoteSchedule");
        result["PostiDisponibili"] = reader.GetOrdinal("PostiDisponibili");
        result["PostiResidui"] = reader.GetOrdinal("PostiResidui");
        result["Title"] = reader.GetOrdinal("Title");
        result["UserIdOwner"] = reader.GetOrdinal("UserIdOwner");
        return result;
    }

    private Dictionary<string, int> GetColumnsTipologiaLezione(SqlDataReader reader)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        result["IdTipologiaLezione"] = reader.GetOrdinal("IdTipologiaLezione");
        result["IdCliente"] = reader.GetOrdinal("IdCliente");
        result["NomeTipologiaLezione"] = reader.GetOrdinal("NomeTipologiaLezione");
        result["DescrizioneTipologiaLezione"] = reader.GetOrdinal("DescrizioneTipologiaLezione");
        result["Durata"] = reader.GetOrdinal("Durata");
        result["MaxPartecipanti"] = reader.GetOrdinal("MaxPartecipanti");
        result["LimiteCancellazioneMinuti"] = reader.GetOrdinal("LimiteCancellazioneMinuti");
        result["Livello"] = reader.GetOrdinal("Livello");
        result["DataCancellazioneTipologiaLezione"] = reader.GetOrdinal("DataCancellazioneTipologiaLezione");
        result["DataCreazioneTipologiaLezione"] = reader.GetOrdinal("DataCreazioneTipologiaLezione");
        return result;
    }

    private Dictionary<string, int> GetColumnsLocation(SqlDataReader reader)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        result["IdLocation"] = reader.GetOrdinal("IdLocation");
        result["IdCliente"] = reader.GetOrdinal("IdCliente");
        result["NomeLocation"] = reader.GetOrdinal("NomeLocation");
        result["DescrizioneLocation"] = reader.GetOrdinal("DescrizioneLocation");
        result["CapienzaMax"] = reader.GetOrdinal("CapienzaMax");
        return result;
    }

    private UtenteClienteAppuntamentoDM InternalReadAppuntamento(SqlDataReader reader, Dictionary<string, int> cols)
    {
        return new UtenteClienteAppuntamentoDM()
        {
            IdAppuntamento = reader.GetInt32(cols["Id"]),
            IdCliente = reader.GetInt32(cols["IdCliente"]),
            DataCancellazione = reader.IsDBNull(cols["CancellabileFinoAl"]) ? (DateTime?)null : reader.GetDateTime(cols["CancellabileFinoAl"]),
            DataPrenotazione = reader.GetDateTime(cols["DataPrenotazione"]),
            IdAbbonamento = reader.GetInt32(cols["IdAbbonamento"]),
            IdSchedule = reader.GetInt32(cols["ScheduleId"]),
            Note = reader.IsDBNull(cols["Note"]) ? null : reader.GetString(cols["Note"]),
            Nominativo = reader.IsDBNull(cols["Nominativo"]) ? null : reader.GetString(cols["Nominativo"])
        };
    }

    private ScheduleDM InternalReadSchedule(SqlDataReader reader, Dictionary<string, int> cols)
    {
        return new ScheduleDM()
        {
            Id = reader.GetInt32(cols["ScheduleId"]),
            IdCliente = reader.GetInt32(cols["IdCliente"]),
            CancellabileFinoAl = reader.GetDateTime(cols["CancellabileFinoAl"]),
            DataChiusuraIscrizione = reader.GetDateTime(cols["DataChiusuraIscrizioni"]),
            DataOraInizio = reader.GetDateTime(cols["DataOraInizio"]),
            IdLocation = reader.GetInt32(cols["IdLocation"]),
            IdTipoLezione = reader.GetInt32(cols["IdTipoLezione"]),
            Istruttore = reader.IsDBNull(cols["Istruttore"]) ? null : reader.GetString(cols["Istruttore"]),
            DataCancellazione = reader.IsDBNull(cols["DataCancellazione"]) ? (DateTime?)null : reader.GetDateTime(cols["DataCancellazione"]),
            Note = reader.IsDBNull(cols["NoteSchedule"]) ? null : reader.GetString(cols["NoteSchedule"]),
            PostiDisponibili = reader.GetInt32(cols["PostiDisponibili"]),
            PostiResidui = reader.GetInt32(cols["PostiResidui"]),
            Title = reader.IsDBNull(cols["Title"]) ? null : reader.GetString(cols["Title"]),
            UserIdOwner = reader.IsDBNull(cols["UserIdOwner"]) ? null : reader.GetString(cols["UserIdOwner"])
        };
    }

    private TipologiaLezioneDM InternalReadTipologiaLezione(SqlDataReader reader, Dictionary<string, int> cols)
    {
        return new TipologiaLezioneDM()
        {
            Id = reader.GetInt32(cols["IdTipologiaLezione"]),
            IdCliente = reader.GetInt32(cols["IdCliente"]),
            Nome = reader.GetString(cols["NomeTipologiaLezione"]),
            Descrizione = reader.IsDBNull(cols["DescrizioneTipologiaLezione"]) ? null : reader.GetString(cols["DescrizioneTipologiaLezione"]),
            Durata = reader.GetInt32(cols["Durata"]),
            LimiteCancellazioneMinuti = reader.IsDBNull(cols["LimiteCancellazioneMinuti"]) ? (short?)null : reader.GetInt16(cols["LimiteCancellazioneMinuti"]),
            Livello = reader.GetInt16(cols["Livello"]),
            MaxPartecipanti = reader.IsDBNull(cols["MaxPartecipanti"]) ? (int?)null : reader.GetInt32(cols["MaxPartecipanti"]),
            DataCreazione = reader.GetDateTime(cols["DataCreazioneTipologiaLezione"]),
            DataCancellazione = reader.IsDBNull(cols["DataCancellazioneTipologiaLezione"]) ? (DateTime?)null : reader.GetDateTime(cols["DataCancellazioneTipologiaLezione"]),
            Prezzo = reader.IsDBNull(cols["PrezzoTipologiaLezione"]) ? (decimal?)null : reader.GetDecimal(cols["PrezzoTipologiaLezione"]),
        };
    }

    private LocationDM InternalReadLocation(SqlDataReader reader, Dictionary<string, int> cols)
    {
        return new LocationDM()
        {
            Id = reader.GetInt32(cols["IdLocation"]),
            CapienzaMax = reader.IsDBNull(cols["CapienzaMax"]) ? (short?)null : reader.GetInt16(cols["CapienzaMax"]),
            Descrizione = reader.IsDBNull(cols["NomeLocation"]) ? null : reader.GetString(cols["NomeLocation"]),
            IdCliente = reader.GetInt32(cols["Id"]),
            Nome = reader.GetString(cols["NomeLocation"])
        };
    }
}
}
