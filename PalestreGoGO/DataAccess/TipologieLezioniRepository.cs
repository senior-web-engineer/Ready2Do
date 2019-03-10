using Dapper;
using Microsoft.Extensions.Configuration;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class TipologieLezioniRepository : BaseRepository, ITipologieLezioniRepository
    {
        #region STATIC STAFF
        internal static Dictionary<string, int> GetTipologieLezioniColumnsOrdinals(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            if ((dr == null) || (!dr.HasRows)) return null;
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            var result = new Dictionary<string, int>();
            result.Add("Id", dr.GetOrdinal(getColumnName("IdTipologieLezioni")));
            result.Add("IdCliente", dr.GetOrdinal(getColumnName("IdClienteTipologieLezioni")));
            result.Add("DataCancellazione", dr.GetOrdinal(getColumnName("DataCancellazioneTipologieLezioni")));
            result.Add("DataCreazione", dr.GetOrdinal(getColumnName("DataCreazioneTipologieLezioni")));
            result.Add("Descrizione", dr.GetOrdinal(getColumnName("DescrizioneTipologieLezioni")));
            result.Add("Durata", dr.GetOrdinal(getColumnName("DurataTipologieLezioni")));
            result.Add("LimiteCancellazioneMinuti", dr.GetOrdinal(getColumnName("LimiteCancellazioneMinutiTipologieLezioni")));
            result.Add("Livello", dr.GetOrdinal(getColumnName("LivelloTipologieLezioni")));
            result.Add("MaxPartecipanti", dr.GetOrdinal(getColumnName("MaxPartecipantiTipologieLezioni")));
            result.Add("Nome", dr.GetOrdinal(getColumnName("NomeTipologieLezioni")));
            result.Add("Prezzo", dr.GetOrdinal(getColumnName("PrezzoTipologieLezioni")));
            return result;
        }

        internal static async Task<TipologiaLezioneDM> ReadTipologiaLezioneAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            TipologiaLezioneDM result = new TipologiaLezioneDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.Descrizione = await dr.IsDBNullAsync(columns["Descrizione"]) ? null : dr.GetString(columns["Descrizione"]);
            result.Durata = dr.GetInt32(columns["Durata"]);
            result.MaxPartecipanti = await dr.IsDBNullAsync(columns["MaxPartecipanti"]) ? default(int?) : dr.GetInt32(columns["MaxPartecipanti"]);
            result.LimiteCancellazioneMinuti = await dr.IsDBNullAsync(columns["LimiteCancellazioneMinuti"]) ? default(short?) : dr.GetInt16(columns["LimiteCancellazioneMinuti"]);
            result.Livello = dr.GetInt16(columns["Livello"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.Prezzo = await dr.IsDBNullAsync(columns["Prezzo"]) ? default(decimal?) : dr.GetDecimal(columns["Prezzo"]);
            result.Nome = dr.GetString(columns["Nome"]);
            return result;
        }
        #endregion

        public TipologieLezioniRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<TipologiaLezioneDM>> GetListAsync(int idTenant, string sortColumn = null, bool sortAsc = true, int pageNumber = 1, int pageSize = 1000, bool includeDeleted = false)
        {
            List<TipologiaLezioneDM> result = new List<TipologiaLezioneDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieLezioni_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idTenant;
                cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
                cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@pSortColumn", SqlDbType.VarChar, 50).Value = sortColumn;
                cmd.Parameters.Add("@pOrderAscending", SqlDbType.Bit).Value = sortAsc;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        var columns = GetTipologieLezioniColumnsOrdinals(dr);
                        while (await dr.ReadAsync())
                        {
                            result.Add(await ReadTipologiaLezioneAsync(dr, columns));
                        }
                    }
                }
                return result;
            }
        }

        public async Task<int> CountAsync(int idTenant)
        {
            using (var cn = GetConnection())
            {
                return await cn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [dbo].[TipologieLezioni] WHERE DataCancellazione IS NULL");
            }
        }

        public async Task<TipologiaLezioneDM> GetAsync(int idTenant, int itemKey)
        {
            TipologiaLezioneDM result = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieLezioni_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idTenant;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = itemKey;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        var columns = GetTipologieLezioniColumnsOrdinals(dr);
                        await dr.ReadAsync();
                        result = await ReadTipologiaLezioneAsync(dr, columns);
                    }
                }
                return result;
            }            
        }


        public async Task<int> AddAsync(int idTenant, TipologiaLezioneDM entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            using (var cn = GetConnection())
            {
                SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
                parId.Direction = ParameterDirection.Output;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieLezioni_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = entity.IdCliente;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = entity.Nome;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 500).Value = entity.Descrizione;
                cmd.Parameters.Add("@pDurata", SqlDbType.Int).Value = entity.Durata;
                cmd.Parameters.Add("@pMaxPartecipanti", SqlDbType.Int).Value = entity.MaxPartecipanti;
                cmd.Parameters.Add("@pLimiteCancellazioneMinuti", SqlDbType.SmallInt).Value = entity.LimiteCancellazioneMinuti;
                cmd.Parameters.Add("@pLivello", SqlDbType.Int).Value = entity.Livello;
                cmd.Parameters.Add("@pPrezzo", SqlDbType.Decimal).Value = entity.Prezzo;
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                entity.Id = (int)parId.Value;
            }
            return entity.Id.Value;
        }

        public async Task UpdateAsync(int idTenant, TipologiaLezioneDM entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieLezioni_Modifica]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = entity.Id;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = entity.IdCliente;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = entity.Nome;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 500).Value = entity.Descrizione;
                cmd.Parameters.Add("@pDurata", SqlDbType.Int).Value = entity.Durata;
                cmd.Parameters.Add("@pMaxPartecipanti", SqlDbType.Int).Value = entity.MaxPartecipanti;
                cmd.Parameters.Add("@pLimiteCancellazioneMinuti", SqlDbType.SmallInt).Value = entity.LimiteCancellazioneMinuti;
                cmd.Parameters.Add("@pLivello", SqlDbType.Int).Value = entity.Livello;
                cmd.Parameters.Add("@pPrezzo", SqlDbType.Decimal).Value = entity.Prezzo;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int idTenant, int entityKey)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieLezioni_Delete]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = entityKey;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idTenant;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Verifica se il nome specificato è già stato utilizzato
        /// </summary>
        /// <param name="idTenant"></param>
        /// <param name="nome"></param>
        /// <returns>True se il nome è disponibile, False se già utilizzato</returns>
        public async Task<bool> CheckNameAsync(int idTenant, string nome, int? id)
        {
            var parameters = new DynamicParameters(new
            {
                pIdCliente = idTenant,
                pNomeTipoLezione = nome,
                pId = id
            });
            parameters.Add("pResult", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[dbo].[TipologieLezioni_CheckNome]", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<bool>("pResult");
            }
        }
    }
}
