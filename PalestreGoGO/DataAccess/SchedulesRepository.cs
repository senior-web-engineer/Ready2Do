using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class SchedulesRepository : BaseRepository, ISchedulesRepository
    {

        public SchedulesRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<int> AddScheduleAsync(int idCliente, ScheduleInputDM schedule)
        {
            if (!schedule.IdCliente.Equals(idCliente)) throw new ArgumentException("Bad Tenant");
            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.Output;
            string jsonRecurrency = (schedule.Recurrency != null ? JsonConvert.SerializeObject(schedule.Recurrency) : null);
            Log.Debug("Salvataggio nuovo schedule. {@Schedule} - SerializedRecurrency: {jsonRecurrency}", schedule, jsonRecurrency);
            using (var cn = GetConnection())
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
                cmd.Parameters.Add("@pRecurrency", SqlDbType.NVarChar, -1).Value = jsonRecurrency;
                cmd.Parameters.Add("@pWaitListDisponibile", SqlDbType.Bit).Value = schedule.WaitListDisponibile;
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                schedule.Id = (int)parId.Value;
            }
            return schedule.Id.Value;
        }

        public async Task UpdateScheduleAsync(int idCliente, ScheduleInputDM schedule, TipoModificaScheduleDM tipoModifica)
        {
            if (!schedule.IdCliente.Equals(idCliente)) throw new ArgumentException("Bad Tenant");
            using (var cn = GetConnection())
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
            using (var cn = GetConnection())
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
                    var columnsSchedule = GetSchedulesColumns(dr);
                    var columnsLocation = LocationsRepository.GetLocationColumnsOrdinals(dr);
                    var columnsTipoLezione = TipologieLezioniRepository.GetTipologieLezioniColumnsOrdinals(dr);

                    while (await dr.ReadAsync())
                    {
                        result.Add(await InternalReadScheduleAsync(dr, columnsSchedule, columnsTipoLezione, columnsLocation));
                    }
                }
            }
            return result;
        }

        //private Dictionary<string, string> GetAliasesTipologieLezioni()
        //{
        //    return new Dictionary<string, string>()
        //    {
        //        {"Nome","NomeTipoLezione"},
        //        {"Descrizione","DescrizioneTipoLezione"},
        //        {"Durata","DurataTipoLezione"},
        //        {"LimiteCancellazioneMinuti","LimiteCancellazioneMinutiTipoLezione"},
        //        {"LivelloTipo","LivelloTipoLezione"},
        //        {"MaxPartecipanti","MaxPartecipantiTipoLezione"},
        //        {"DataCreazione","TipoLezioneDataCreazione"},
        //        {"DataCancellazione","TipoLezioneDataCancellazione"},
        //        {"Prezzo","PrezzoTipologiaLezione"},
        //    };
        //}

        //private Dictionary<string, string> GetAliasesLocations()
        //{
        //    return new Dictionary<string, string>()
        //        {
        //            { "Nome", "NomeLocation" },
        //            { "CapienzaMax", "CapienzaMaxLocation"},
        //            { "Descrizione", "DescrizioneLocation"},
        //            { "Colore", "ColoreLocation"},
        //            { "ImageUrl", "ImageUrlLocation" },
        //            { "IconUrlLocation", "IconUrlLocation" }
        //        };
        //}

        public async Task<IEnumerable<ScheduleDM>> SchedulesLookupAsync(IEnumerable<int> idSchedules, bool includeDeleted = false)
        {
            List<ScheduleDM> result = new List<ScheduleDM>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            foreach (var i in idSchedules)
            {
                var row = dt.NewRow();
                row["Id"] = i;
                dt.Rows.Add(row);
            }
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Schedules_Lookup]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                var parTable = cmd.Parameters.AddWithValue("@pIdSchedules", dt);
                parTable.SqlDbType = SqlDbType.Structured;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    var columns = GetSchedulesColumns(dr);
                    var columnsLocation = LocationsRepository.GetLocationColumnsOrdinals(dr);
                    var columnsTipoLezione = TipologieLezioniRepository.GetTipologieLezioniColumnsOrdinals(dr);

                    while (await dr.ReadAsync())
                    {
                        result.Add(await InternalReadScheduleAsync(dr, columns, columnsTipoLezione, columnsLocation));
                    }
                }
            }
            return result;
        }


        public async Task<ScheduleDM> GetScheduleAsync(int idCliente, int idSchedule, bool includeDeleted = false)
        {
            using (var cn = GetConnection())
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
                        var columns = GetSchedulesColumns(dr);
                        var columnsLocation = LocationsRepository.GetLocationColumnsOrdinals(dr);
                        var columnsTipoLezione = TipologieLezioniRepository.GetTipologieLezioniColumnsOrdinals(dr);

                        return await InternalReadScheduleAsync(dr, columns, columnsTipoLezione, columnsLocation);
                    }
                }
            }
            return null;
        }


        public async Task DeleteScheduleAsync(int idCliente, int idSchedule)
        {
            using (var cn = GetConnection())
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


        #region Static Stuff
        internal static Dictionary<string, int> GetSchedulesColumns(SqlDataReader reader, Dictionary<string, string> aliases = null)
        {
            if ((reader == null) || (!reader.HasRows)) return null;
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };


            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", reader.GetOrdinal(getColumnName("IdSchedules")));
            result.Add("IdCliente", reader.GetOrdinal(getColumnName("IdClienteSchedules")));
            result.Add("Title", reader.GetOrdinal(getColumnName("TitleSchedules")));
            result.Add("IdTipoLezione", reader.GetOrdinal(getColumnName("IdTipoLezioneSchedules")));
            result.Add("IdLocation", reader.GetOrdinal(getColumnName("IdLocationSchedules")));
            result.Add("DataOraInizio", reader.GetOrdinal(getColumnName("DataOraInizioSchedules")));
            result.Add("Istruttore", reader.GetOrdinal(getColumnName("IstruttoreSchedules")));
            result.Add("PostiDisponibili", reader.GetOrdinal(getColumnName("PostiDisponibiliSchedules")));
            result.Add("PostiResidui", reader.GetOrdinal(getColumnName("PostiResiduiSchedules")));
            result.Add("NumPrenotazioni", reader.GetOrdinal(getColumnName("NumPrenotazioniSchedules")));            
            result.Add("CancellazioneConsentita", reader.GetOrdinal(getColumnName("CancellazioneConsentitaSchedules")));
            result.Add("CancellabileFinoAl", reader.GetOrdinal(getColumnName("CancellabileFinoAlSchedules")));
            result.Add("DataAperturaIscrizioni", reader.GetOrdinal(getColumnName("DataAperturaIscrizioniSchedules")));
            result.Add("DataChiusuraIscrizioni", reader.GetOrdinal(getColumnName("DataChiusuraIscrizioniSchedules")));
            result.Add("DataCancellazione", reader.GetOrdinal(getColumnName("DataCancellazioneSchedules")));
            result.Add("UserIdOwner", reader.GetOrdinal(getColumnName("UserIdOwnerSchedules")));
            result.Add("Note", reader.GetOrdinal(getColumnName("NoteSchedules")));
            result.Add("WaitListDisponibile", reader.GetOrdinal(getColumnName("WaitListDisponibileSchedules")));
            result.Add("VisibileDal", reader.GetOrdinal(getColumnName("VisibileDalSchedules")));
            result.Add("VisibileFinoAl", reader.GetOrdinal(getColumnName("VisibileFinoAlSchedules")));
            result.Add("Recurrency", reader.GetOrdinal(getColumnName("RecurrencySchedules")));
            result.Add("IdParent", reader.GetOrdinal(getColumnName("IdParentSchedules")));
            result.Add("DataCreazione", reader.GetOrdinal(getColumnName("DataCreazioneSchedules")));

            return result;
        }

        internal static async Task<ScheduleDM> InternalReadScheduleAsync(SqlDataReader reader, Dictionary<string, int> columns,
                                                            Dictionary<string, int> columnsTipoLez, Dictionary<string, int> columnsLocat)
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
            result.TipologiaLezione = await TipologieLezioniRepository.ReadTipologiaLezioneAsync(reader, columnsTipoLez);

            result.Location = await LocationsRepository.InternalReadLocationAsync(reader, columnsLocat);

            return result;
        }
        #endregion

    }
}
