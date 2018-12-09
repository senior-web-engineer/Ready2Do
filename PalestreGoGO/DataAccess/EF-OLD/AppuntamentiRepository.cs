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
    public class AppuntamentiRepository : IAppuntamentiRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<AppuntamentiRepository> _logger;
        private readonly IConfiguration _config;

        public AppuntamentiRepository(ILogger<AppuntamentiRepository> logger, PalestreGoGoDbContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        public async Task<int> AddAppuntamentoAsync(int idCliente, Appuntamenti appuntamento)
        {
            if (appuntamento == null) throw new ArgumentNullException(nameof(appuntamento));
            if (!appuntamento.IdCliente.Equals(idCliente)) throw new ArgumentException("Wrong Tenant");
            using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var retVal = new SqlParameter();
                retVal.Direction = ParameterDirection.ReturnValue;
                var parId = new SqlParameter("@pId", SqlDbType.Int);
                parId.Direction = ParameterDirection.Output;

                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Appuntamenti_Add]";
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 50).Value = appuntamento.UserId?.ToString();
                cmd.Parameters.Add("@pScheduleId", SqlDbType.Int).Value = appuntamento.ScheduleId;
                cmd.Parameters.Add("@pIdAbbonamento", SqlDbType.Int).Value = appuntamento.IdAbbonamento;
                cmd.Parameters.Add("@pNote", SqlDbType.NVarChar, 1000).Value = appuntamento.Note;
                cmd.Parameters.Add("@pNominativo", SqlDbType.NVarChar, 200).Value = appuntamento.Nominativo;
                cmd.Parameters.Add(parId);
                cmd.Parameters.Add(retVal);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                appuntamento.Id = (int)parId.Value;
            }

            return appuntamento.Id;
        }

        public async Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento)
        {

            using (var cn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
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
                //if((int)retVal.Value != 1)
                //{
                //    throw new InvalidOperationException("Impossibile annullare l'appuntamento.");
                //}
            }
        }



        public IEnumerable<Appuntamenti> GetAppuntamentiForSchedule(int idCliente, int idSchedule)
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
