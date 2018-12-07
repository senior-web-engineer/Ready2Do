using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class SchedulesRepository : ISchedulesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SchedulesRepository> _logger;

        public SchedulesRepository(IConfiguration configuration, ILogger<SchedulesRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<int> AddScheduleAsync(int idCliente, ScheduleBaseDM schedule)
        {
            if (!schedule.IdCliente.Equals(idCliente)) throw new ArgumentException("Bad Tenant");
            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.Output;
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[Schedules_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pTitle", SqlDbType.NVarChar, 100).Value = schedule.Title;
                cmd.Parameters.Add("@pIdTipoLezione", SqlDbType.Int).Value = schedule.IdTipoLezione;
                cmd.Parameters.Add("@pIdLocation", SqlDbType.Int).Value = schedule.IdLocation;
                cmd.Parameters.Add("@pDataOraInizio", SqlDbType.DateTime2).Value = schedule.DataOraInizio;
                cmd.Parameters.Add("@pIstruttore", SqlDbType.NVarChar, 150).Value = schedule.Istruttore;
                cmd.Parameters.Add("@pPosti", SqlDbType.Int).Value = schedule.PostiDisponibili;
                cmd.Parameters.Add("@pCancellazionePossib", SqlDbType.Bit).Value = schedule.CancellazioneConsentita;
                cmd.Parameters.Add("@pCancellabileFinoAl", SqlDbType.DateTime2).Value = schedule.CancellabileFinoAl;
                cmd.Parameters.Add("@pDataAperturaIscriz", SqlDbType.DateTime2).Value = schedule.DataAperturaIscrizione;
                cmd.Parameters.Add("@pDataChiusuraIscriz", SqlDbType.DateTime2).Value = schedule.DataChiusuraIscrizione;
                cmd.Parameters.Add("@pVisibileDal", SqlDbType.DateTime2).Value = schedule.VisibileDal;
                cmd.Parameters.Add("@pVisibileFinoAl", SqlDbType.DateTime2).Value = schedule.VisibileFinoAl;
                cmd.Parameters.Add("@pNote", SqlDbType.NVarChar, 1000).Value = schedule.Note;
                cmd.Parameters.Add("@pUserIdOwner", SqlDbType.NVarChar, 450).Value = schedule.UserIdOwner;
                cmd.Parameters.Add("@pRecurrency", SqlDbType.NVarChar, -1).Value = JsonConvert.SerializeObject(schedule.Recurrency);
                cmd.Parameters.Add("@pWaitListDisponibile", SqlDbType.Bit).Value = schedule.WaitListDisponibile;
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                schedule.Id = (int)parId.Value;
            }
            return schedule.Id.Value;
        }

        public async Task UpdateScheduleAsync(int idCliente, ScheduleBaseDM schedule, TipoModificaScheduleDM tipoModifica)
        {
            if (!schedule.IdCliente.Equals(idCliente)) throw new ArgumentException("Bad Tenant");
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[Schedules_Update]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = schedule.Id;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pTitle", SqlDbType.NVarChar, 100).Value = schedule.Title;
                cmd.Parameters.Add("@pIdTipoLezione", SqlDbType.Int).Value = schedule.IdTipoLezione;
                cmd.Parameters.Add("@pIdLocation", SqlDbType.Int).Value = schedule.IdLocation;
                cmd.Parameters.Add("@pDataOraInizio", SqlDbType.DateTime2).Value = schedule.DataOraInizio;
                cmd.Parameters.Add("@pIstruttore", SqlDbType.NVarChar, 150).Value = schedule.Istruttore;
                cmd.Parameters.Add("@pPosti", SqlDbType.Int).Value = schedule.PostiDisponibili;
                cmd.Parameters.Add("@pCancellazionePossib", SqlDbType.Bit).Value = schedule.CancellazioneConsentita;
                cmd.Parameters.Add("@pCancellabileFinoAl", SqlDbType.DateTime2).Value = schedule.CancellabileFinoAl;
                cmd.Parameters.Add("@pDataAperturaIscriz", SqlDbType.DateTime2).Value = schedule.DataAperturaIscrizione;
                cmd.Parameters.Add("@pDataChiusuraIscriz", SqlDbType.DateTime2).Value = schedule.DataChiusuraIscrizione;
                cmd.Parameters.Add("@pVisibileDal", SqlDbType.DateTime2).Value = schedule.VisibileDal;
                cmd.Parameters.Add("@pVisibileFinoAl", SqlDbType.DateTime2).Value = schedule.VisibileFinoAl;
                cmd.Parameters.Add("@pNote", SqlDbType.NVarChar, 1000).Value = schedule.Note;
                cmd.Parameters.Add("@pUserIdOwner", SqlDbType.NVarChar, 450).Value = schedule.UserIdOwner;
                cmd.Parameters.Add("@pRecurrency", SqlDbType.NVarChar, -1).Value = JsonConvert.SerializeObject(schedule.Recurrency);
                cmd.Parameters.Add("@pWaitListDisponibile", SqlDbType.Bit).Value = schedule.WaitListDisponibile;
                cmd.Parameters.Add("@pTipoModifica", SqlDbType.Bit).Value = (tipoModifica == TipoModificaScheduleDM.SingoloSchedule ? "S" : "N");
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<ScheduleDM>> GetScheduleListAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null,
                                                                        int? idTipoLezione = null, bool soloPostiDisp = false, bool soloIscrizAperte = false,
                                                                        int pageSize = 25, int pageNumber = 1, string sortColumn = "dataorainizio",
                                                                        bool ascending = true, bool includeDeleted = false)
        {
            List<ScheduleDM> result = new List<ScheduleDM>();
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[Schedules_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pStartDate", SqlDbType.DateTime2).Value = startDate;
                cmd.Parameters.Add("@pEndDate", SqlDbType.DateTime2).Value = endDate;
                cmd.Parameters.Add("@pIdLocation", SqlDbType.Int).Value = idLocation;
                cmd.Parameters.Add("@pTipoLezione", SqlDbType.Int).Value = idTipoLezione;
                cmd.Parameters.Add("@pSoloPostiDispon", SqlDbType.Bit).Value = soloPostiDisp;
                cmd.Parameters.Add("@pSoloIscrizAperte", SqlDbType.Bit).Value = soloIscrizAperte;
                cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
                cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@pSortColumn", SqlDbType.VarChar, 50).Value = sortColumn;
                cmd.Parameters.Add("@pOrderAscending", SqlDbType.Bit).Value = ascending;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        result.Add(await internalReadSchedule(dr, GetScheduleColumns(dr)));
                    }
                }
            }
            return result;
        }

        public async Task<ScheduleDM> GetScheduleAsync(int idCliente, int idSchedule, bool includeDeleted = false)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Schedules_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        return await internalReadSchedule(dr, GetScheduleColumns(dr));
                    }
                }
            }
            return null;
        }


        //public async Task<IEnumerable<Schedules>> GetSchedulesAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null)
        //{
        //    IEnumerable<Schedules> result = null;
        //    using (var cn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
        //    {
        //        result = await cn.QueryAsync<Schedules, Locations, TipologieLezioni, Schedules>("Schedules_GetForCliente",
        //                            (s, l, tl) =>
        //                            {
        //                                s.Location = l;
        //                                s.TipologiaLezione = tl;
        //                                return s;
        //                            },
        //                            splitOn: "IdLocation,IdTipoLezione",
        //                            commandType: System.Data.CommandType.StoredProcedure,
        //                            param: new { pIdCliente = idCliente, pStartDate = startDate, pEndDate = endDate, pIdLocation = idLocation }
        //                    );
        //    }
        //    return result;
        //}

        public async Task DeleteScheduleAsync(int idCliente, int idSchedule)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Schedules_Delete]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = idSchedule;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }


        #region Private Stuff
        private Dictionary<string, int> GetScheduleColumns(SqlDataReader reader)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", reader.GetOrdinal("Id"));
            result.Add("IdCliente", reader.GetOrdinal("IdCliente"));
            result.Add("Title", reader.GetOrdinal("Title"));
            result.Add("IdTipoLezione", reader.GetOrdinal("IdTipoLezione"));
            result.Add("IdLocation", reader.GetOrdinal("IdLocation"));
            result.Add("DataOraInizio", reader.GetOrdinal("DataOraInizio"));
            result.Add("Istruttore", reader.GetOrdinal("Istruttore"));
            result.Add("PostiDisponibili", reader.GetOrdinal("PostiDisponibili"));
            result.Add("PostiResidui", reader.GetOrdinal("PostiResidui"));
            result.Add("CancellazioneConsentita", reader.GetOrdinal("CancellazioneConsentita"));
            result.Add("CancellabileFinoAl", reader.GetOrdinal("CancellabileFinoAl"));
            result.Add("DataAperturaIscrizioni", reader.GetOrdinal("DataAperturaIscrizioni"));
            result.Add("DataChiusuraIscrizioni", reader.GetOrdinal("DataChiusuraIscrizioni"));
            result.Add("DataCancellazione", reader.GetOrdinal("DataCancellazione"));
            result.Add("UserIdOwner", reader.GetOrdinal("UserIdOwner"));
            result.Add("Note", reader.GetOrdinal("Note"));
            result.Add("VisibileDal", reader.GetOrdinal("VisibileDal"));
            result.Add("VisibileFinoAl", reader.GetOrdinal("VisibileFinoAl"));
            result.Add("WaitListDisponibile", reader.GetOrdinal("WaitListDisponibile"));
            result.Add("Recurrency", reader.GetOrdinal("Recurrency"));
            result.Add("IdParent", reader.GetOrdinal("IdParent"));
            result.Add("NomeLocation", reader.GetOrdinal("NomeLocation"));
            result.Add("CapienzaMaxLocation", reader.GetOrdinal("CapienzaMaxLocation"));
            result.Add("DescrizioneLocation", reader.GetOrdinal("DescrizioneLocation"));
            result.Add("NomeTipoLezione", reader.GetOrdinal("NomeTipoLezione"));
            result.Add("DescrizioneTipoLezione", reader.GetOrdinal("DescrizioneTipoLezione"));
            result.Add("DurataTipoLezione", reader.GetOrdinal("DurataTipoLezione"));
            result.Add("LimiteCancellazioneMinutiTipoLezione", reader.GetOrdinal("LimiteCancellazioneMinutiTipoLezione"));
            result.Add("LivelloTipoLezione", reader.GetOrdinal("LivelloTipoLezione"));
            result.Add("MaxPartecipantiTipoLezione", reader.GetOrdinal("MaxPartecipantiTipoLezione"));
            result.Add("TipoLezioneDataCreazione", reader.GetOrdinal("TipoLezioneDataCreazione"));
            result.Add("TipoLezioneDataCancellazione", reader.GetOrdinal("TipoLezioneDataCancellazione"));
            return result;
        }

        private async Task<ScheduleDM> internalReadSchedule(SqlDataReader reader, Dictionary<string, int> columns)
        {
            ScheduleDM result = new ScheduleDM();
            result.Id = reader.GetInt32(columns["Id"]);
            result.IdCliente = reader.GetInt32(columns["IdCliente"]);
            result.Title = reader.GetString(columns["Title"]);
            result.IdTipoLezione = reader.GetInt32(columns["IdTipoLezione"]);
            result.IdLocation = reader.GetInt32(columns["IdLocation"]);
            result.DataOraInizio = reader.GetDateTime(columns["DataOraInizio"]);
            result.Istruttore = await reader.IsDBNullAsync(columns["Istruttore"]) ? null : reader.GetString(columns["Istruttore"]);
            result.PostiDisponibili = reader.GetInt32(columns["PostiDisponibili"]);
            result.PostiResidui = reader.GetInt32(columns["PostiResidui"]);
            result.CancellazioneConsentita = reader.GetBoolean(columns["CancellazioneConsentita"]);
            result.CancellabileFinoAl = await reader.IsDBNullAsync(columns["CancellabileFinoAl"]) ? default(DateTime?) : reader.GetDateTime(columns["CancellabileFinoAl"]);
            result.DataAperturaIscrizione = await reader.IsDBNullAsync(columns["DataAperturaIscrizioni"]) ? default(DateTime?) : reader.GetDateTime(columns["DataAperturaIscrizioni"]);
            result.DataChiusuraIscrizione = await reader.IsDBNullAsync(columns["DataChiusuraIscrizioni"]) ? default(DateTime?) : reader.GetDateTime(columns["DataChiusuraIscrizioni"]);
            result.DataCancellazione = await reader.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : reader.GetDateTime(columns["DataCancellazione"]);
            result.UserIdOwner = await reader.IsDBNullAsync(columns["UserIdOwner"]) ? null : reader.GetString(columns["UserIdOwner"]);
            result.Note = await reader.IsDBNullAsync(columns["Note"]) ? null : reader.GetString(columns["Note"]);
            result.VisibileDal = await reader.IsDBNullAsync(columns["VisibileDal"]) ? default(DateTime?) : reader.GetDateTime(columns["VisibileDal"]);
            result.VisibileFinoAl = await reader.IsDBNullAsync(columns["VisibileFinoAl"]) ? default(DateTime?) : reader.GetDateTime(columns["VisibileFinoAl"]);
            result.Recurrency = await reader.IsDBNullAsync(columns["Recurrency"]) ? null : JsonConvert.DeserializeObject<ScheduleRecurrencyDM>(reader.GetString(columns["Recurrency"]));
            result.IdParent = await reader.IsDBNullAsync(columns["IdParent"]) ? default(int?) : reader.GetInt32(columns["IdParent"]);
            result.WaitListDisponibile = reader.GetBoolean(columns["WaitListDisponibile"]);
            result.TipologiaLezione = new TipologiaLezioneDM();
            result.TipologiaLezione.Id = result.IdTipoLezione;
            result.TipologiaLezione.IdCliente = result.IdCliente;
            result.TipologiaLezione.DataCancellazione = await reader.IsDBNullAsync(columns["TipoLezioneDataCancellazione"]) ? default(DateTime?) : reader.GetDateTime(columns["TipoLezioneDataCancellazione"]);
            result.TipologiaLezione.DataCreazione = reader.GetDateTime(columns["TipoLezioneDataCreazione"]);
            result.TipologiaLezione.Descrizione = reader.GetString(columns["DescrizioneTipoLezione"]);
            result.TipologiaLezione.Durata = reader.GetInt32(columns["DurataTipoLezione"]);
            result.TipologiaLezione.LimiteCancellazioneMinuti = await reader.IsDBNullAsync(columns["LimiteCancellazioneMinutiTipoLezione"]) ? default(short?) : reader.GetInt16(columns["LimiteCancellazioneMinutiTipoLezione"]);
            result.TipologiaLezione.Livello = reader.GetInt16(columns["LivelloTipoLezione"]);
            result.TipologiaLezione.MaxPartecipanti = await reader.IsDBNullAsync(columns["MaxPartecipantiTipoLezione"]) ? default(int?) : reader.GetInt32(columns["MaxPartecipantiTipoLezione"]);
            result.TipologiaLezione.Nome = reader.GetString(columns["NomeTipoLezione"]);
            result.Location = new LocationDM();
            result.Location.CapienzaMax = await reader.IsDBNullAsync(columns["CapienzaMaxLocation"]) ? default(short?) : reader.GetInt16(columns["CapienzaMaxLocation"]);
            result.Location.Descrizione = await reader.IsDBNullAsync(columns["DescrizioneLocation"]) ? null : reader.GetString(columns["DescrizioneLocation"]);
            result.Location.Id = result.IdLocation;
            result.Location.IdCliente = result.IdCliente;
            result.Location.Nome = reader.GetString(columns["NomeLocation"]);
            return result;
        }
        #endregion

    }
}
