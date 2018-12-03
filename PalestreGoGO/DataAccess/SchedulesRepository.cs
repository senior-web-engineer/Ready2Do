using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PalestreGoGo.DataModel;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Newtonsoft.Json;

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

        public async Task<int> SaveScheduleAsync(int idCliente, ScheduleDM schedule)
        {
            if (!schedule.IdCliente.Equals(idCliente)) throw new ArgumentException("Bad Tenant");
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add
            }

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule.Id;
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
            result.Add("CancellabileFinoAl", reader.GetOrdinal("CancellabileFinoAl"));
            result.Add("DataAperturaIscrizioni", reader.GetOrdinal("DataAperturaIscrizioni"));
            result.Add("DataChiusuraIscrizioni", reader.GetOrdinal("DataChiusuraIscrizioni"));
            result.Add("DataCancellazione", reader.GetOrdinal("DataCancellazione"));
            result.Add("UserIdOwner", reader.GetOrdinal("UserIdOwner"));
            result.Add("Note", reader.GetOrdinal("Note"));
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
            result.CancellabileFinoAl = reader.GetDateTime(columns["CancellabileFinoAl"]);
            result.DataAperturaIscrizione = reader.GetDateTime(columns["DataAperturaIscrizioni"]);
            result.DataChiusuraIscrizione = reader.GetDateTime(columns["DataChiusuraIscrizioni"]);
            result.DataCancellazione = await reader.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : reader.GetDateTime(columns["DataCancellazione"]);
            result.UserIdOwner = await reader.IsDBNullAsync(columns["UserIdOwner"]) ? null : reader.GetString(columns["UserIdOwner"]);
            result.Note = await reader.IsDBNullAsync(columns["Note"]) ? null : reader.GetString(columns["Note"]);
            result.Recurrency = await reader.IsDBNullAsync(columns["Recurrency"]) ? null : JsonConvert.DeserializeObject<ScheduleRecurrencyDM>(reader.GetString(columns["Recurrency"]));
            result.IdParent = await reader.IsDBNullAsync(columns["IdParent"]) ? default(int?) : reader.GetInt32(columns["IdParent"]);
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

        public IEnumerable<Schedules> GetSchedules(int idCliente, DateTime startDate, DateTime endDate)
        {
            var result = _context.Schedules
                                    .Include(s => s.TipologiaLezione)
                                    .Include(s => s.Location)
                                    .Where(s => (s.IdCliente.Equals(idCliente) &&
                                           (s.DataOraInizio >= startDate) &&
                                           (s.DataOraInizio <= endDate)));
            return result;
        }

        public async Task<IEnumerable<Schedules>> GetSchedulesAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null)
        {
            IEnumerable<Schedules> result = null;
            using (var cn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                result = await cn.QueryAsync<Schedules, Locations, TipologieLezioni, Schedules>("Schedules_GetForCliente",
                                    (s, l, tl) =>
                                    {
                                        s.Location = l;
                                        s.TipologiaLezione = tl;
                                        return s;
                                    },
                                    splitOn: "IdLocation,IdTipoLezione",
                                    commandType: System.Data.CommandType.StoredProcedure,
                                    param: new { pIdCliente = idCliente, pStartDate = startDate, pEndDate = endDate, pIdLocation = idLocation }
                            );
            }
            return result;
        }
        public async Task RemoveScheduleAsync(int idCliente, int idSchedule)
        {
            var entity = _context.Schedules
                                .Include(s => s.Appuntamenti)
                                .Single(tl => tl.IdCliente.Equals(idCliente) && tl.Id.Equals(idSchedule));
            if (entity == null) throw new ArgumentException("Invalid Tenant");
            if (entity.DataOraInizio <= DateTime.Now)
            {
                throw new InvalidOperationException("Impossibile cancellare uno schedule passato.");
            }

            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    entity.DataCancellazione = DateTime.Now;
                    foreach (var app in entity.Appuntamenti)
                    {
                        app.DataCancellazione = DateTime.Now;
                        //Rimborisamo gli appuntamenti (solo i NON GUEST)
                        if (app.UserId != null)
                        {
                            var abbonamento = await _context.AbbonamentiUtenti.FirstOrDefaultAsync(au => au.IdCliente.Equals(idCliente) && au.UserId.Equals(app.UserId));
                            // Avoid overflow
                            if ((abbonamento != null) && (abbonamento.IngressiResidui < Int16.MaxValue))
                            {
                                abbonamento.IngressiResidui++;
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, $"Errore durante la cancellazione dello Schedule. IdCliente: {idCliente}, IdSchedule:{idSchedule}");
                    trans.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateSchedule(int idCliente, Schedules entity)
        {
            //Attenzione! Non verifichiamo il tenant
            //TODO: Implementare le logiche di verifica di fattibilità

            if (entity.IdCliente != idCliente) throw new ArgumentException("idTenant not valid");
            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                var oldItem = await _context.Schedules.AsNoTracking().SingleAsync(s => s.Id.Equals(entity.Id));
                var numPrenotazioni = oldItem.PostiDisponibili - oldItem.PostiResidui;
                if (entity.PostiDisponibili < numPrenotazioni)
                {
                    //Impossibile impostare un numero di posti disponibile inferiore a quelli già riservati (ma funziona questo controllo?)
                    throw new ApplicationException("Numero di Posti Disponibili inferiore al numero di prenotazioni già presenti");
                }
                //Calcoliamo i nuovi posti residui 
                entity.PostiResidui = entity.PostiDisponibili - numPrenotazioni;
                EntityEntry dbEntityEntry = _context.Entry<Schedules>(entity);
                dbEntityEntry.State = EntityState.Modified;
                await _context.SaveChangesAsync();
                trans.Commit();
            }
        }
    }
}
