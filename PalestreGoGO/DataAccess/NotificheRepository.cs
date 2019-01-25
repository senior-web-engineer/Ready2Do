using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;


namespace PalestreGoGo.DataAccess
{
    public class NotificheRepository : BaseRepository, INotificheRepository
    {


        public NotificheRepository(IConfiguration configuration) : base(configuration)
        {
        }


        internal static Dictionary<string, int> GetNotificheColumnsOrdinals(SqlDataReader reader, IDictionary<string, string> aliases = null)
        {
            if ((reader == null) || (!reader.HasRows)) return null;
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };


            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", reader.GetOrdinal(getColumnName("IdNotifiche")));
            result.Add("IdTipo", reader.GetOrdinal(getColumnName("IdTipoNotifiche")));
            result.Add("UserId", reader.GetOrdinal(getColumnName("UserIdNotifiche")));
            result.Add("IdCliente", reader.GetOrdinal(getColumnName("IdClienteNotifiche")));
            result.Add("Titolo", reader.GetOrdinal(getColumnName("TitoloNotifiche")));
            result.Add("Testo", reader.GetOrdinal(getColumnName("TestoNotifiche")));
            result.Add("DataCreazione", reader.GetOrdinal(getColumnName("DataCreazioneNotifiche")));
            result.Add("DataInizioVisibilita", reader.GetOrdinal(getColumnName("DataInizioVisibilitaNotifiche")));
            result.Add("DataFineVisibilita", reader.GetOrdinal(getColumnName("DataFineVisibilitaNotifiche")));
            result.Add("DataDismissione", reader.GetOrdinal(getColumnName("DataDismissioneNotifiche")));
            result.Add("DataPrimaVisualizzazione", reader.GetOrdinal(getColumnName("DataPrimaVisualizzazioneNotifiche")));
            result.Add("ActionUrl", reader.GetOrdinal(getColumnName("ActionUrlNotifiche")));
            return result;
        }

        internal static Dictionary<string, int> GetTipologieNotificheColumnsOrdinals(SqlDataReader reader, IDictionary<string, string> aliases = null)
        {
            if ((reader == null) || (!reader.HasRows)) return null;
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", reader.GetOrdinal(getColumnName("IdTipologieNotifiche")));
            result.Add("Code", reader.GetOrdinal(getColumnName("CodeTipologieNotifiche")));
            result.Add("AutoDismissAfter", reader.GetOrdinal(getColumnName("AutoDismissAfterTipologieNotifiche")));
            result.Add("Priority", reader.GetOrdinal(getColumnName("PriorityTipologieNotifiche")));
            result.Add("UserDismissable", reader.GetOrdinal(getColumnName("UserDismissableTipologieNotifiche")));
            return result;
        }

        internal static async Task<TipologiaNotificaDM> InternalReadTipologiaNotifica(SqlDataReader reader, IDictionary<string, int> columns)
        {
            TipologiaNotificaDM tipo = new TipologiaNotificaDM();
            tipo.AutoDismisAfter = await reader.IsDBNullAsync(columns["AutoDismissAfter"]) ? default(long?) : reader.GetInt64(columns["AutoDismissAfter"]);
            tipo.Code = reader.GetString(columns["Code"]);
            tipo.Id = reader.GetInt32(columns["Id"]);
            tipo.Priority = reader.GetInt32(columns["Priority"]);
            tipo.UserDismissable = reader.GetBoolean(columns["UserDismissable"]);
            return tipo;
        }

        internal static async Task<NotificaConTipoDM> InternalReadNotificaAsync(SqlDataReader reader, IDictionary<string, int> columns,
                                                                                IDictionary<string, int> columnsTipoNotifiche = null)
        {
            NotificaConTipoDM result = new NotificaConTipoDM();
            result.Id = reader.GetInt32(columns["Id"]);
            result.IdTipo = reader.GetInt32(columns["IdTipo"]);
            result.UserId = reader.GetString(columns["UserId"]);
            result.IdCliente = await reader.IsDBNullAsync(columns["IdCliente"]) ? default(int?) : reader.GetInt32(columns["IdCliente"]);
            result.Titolo = reader.GetString(columns["Titolo"]);
            result.Testo = reader.GetString(columns["Testo"]);
            result.DataCreazione = reader.GetDateTime(columns["DataCreazione"]);
            result.DataInizioVisibilita = await reader.IsDBNullAsync(columns["DataInizioVisibilita"]) ? default(DateTime?) : reader.GetDateTime(columns["DataInizioVisibilita"]);
            result.DataFineVisibilita = await reader.IsDBNullAsync(columns["DataFineVisibilita"]) ? default(DateTime?) : reader.GetDateTime(columns["DataFineVisibilita"]);
            result.DataDismissione = await reader.IsDBNullAsync(columns["DataDismissione"]) ? default(DateTime?) : reader.GetDateTime(columns["DataDismissione"]);
            result.DataPrimaVisualizzazione = await reader.IsDBNullAsync(columns["DataPrimaVisualizzazione"]) ? default(DateTime?) : reader.GetDateTime(columns["DataPrimaVisualizzazione"]);
            result.ActionUrl = await reader.IsDBNullAsync(columns["ActionUrl"]) ? default(string) : reader.GetString(columns["ActionUrl"]);
            if ((columnsTipoNotifiche?.Count ?? 0) > 0)
            {
                result.Tipo = await InternalReadTipologiaNotifica(reader, columnsTipoNotifiche);
            }
            return result;
        }


        public async Task<long> AddNotificaAsync(NotificaDM notifica)
        {
            Debug.Assert(notifica != null);
            SqlParameter parIdNotifica = new SqlParameter("@pIdNotitifica", SqlDbType.BigInt);
            parIdNotifica.Direction = ParameterDirection.Output;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Notifiche_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdTipo", SqlDbType.Int).Value = notifica.IdTipo;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = notifica.UserId;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = notifica.IdCliente;
                cmd.Parameters.Add("@pTitolo", SqlDbType.NVarChar, 50).Value = notifica.Titolo;
                cmd.Parameters.Add("@pTesto", SqlDbType.NVarChar, 250).Value = notifica.Testo;
                cmd.Parameters.Add("@pDataInizioVisibilita", SqlDbType.DateTime2).Value = notifica.DataInizioVisibilita;
                cmd.Parameters.Add("@pDataFineVisibilita", SqlDbType.DateTime2).Value = notifica.DataFineVisibilita;
                cmd.Parameters.Add("@pActionUrl", SqlDbType.VarChar, 5000).Value = notifica.ActionUrl;
                cmd.Parameters.Add(parIdNotifica);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            return (long)parIdNotifica.Value;
        }

        public async Task<IEnumerable<NotificaConTipoDM>> GetNotificheAsync(string userId, int? idCliente = null,
                                                                            FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive,
                                                                            int pageNumber = 1, int pageSize = 10)
        {
            Dictionary<string, int> columnsNotifica, columnsTipoNotifica;
            List<NotificaConTipoDM> result = new List<NotificaConTipoDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Notifiche_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pStatoNotifica", SqlDbType.TinyInt).Value = (byte)filtro;
                cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        columnsNotifica = GetNotificheColumnsOrdinals(dr);
                        columnsTipoNotifica = GetTipologieNotificheColumnsOrdinals(dr);
                        while (await dr.ReadAsync())
                        {
                            result.Add(await InternalReadNotificaAsync(dr, columnsNotifica, columnsTipoNotifica));
                        }
                    }
                }
                return result;
            }
        }

        public async Task UpdateNotifica(string userId, long idNotifica, DateTime? dataView, DateTime? dataDismiss)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Notifiche_Aggiorna]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pIdNotifica", SqlDbType.BigInt).Value = idNotifica;
                cmd.Parameters.Add("@pDataView", SqlDbType.DateTime).Value = dataView;
                cmd.Parameters.Add("@pDataDismiss", SqlDbType.DateTime).Value = dataDismiss;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
