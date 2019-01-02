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
        public TipologieLezioniRepository(IConfiguration configuration):base(configuration)
        {

        }

        public async Task<IEnumerable<TipologiaLezioneDM>> GetListAsync(int idTenant, string sortColumn = null, bool sortAsc = true, int pageNumber = 1, int pageSize = 1000)
        {
            using (var cn = GetConnection())
            {
                return await cn.QueryAsync<TipologiaLezioneDM>("[dbo].[TipologieLezioni_Lista]",
                                                                new
                                                                {
                                                                    pIdCliente = idTenant,
                                                                    pPageSize = pageSize,
                                                                    pPageNumber = pageNumber,
                                                                    pSortColumn = sortColumn,
                                                                    pOrderAscending = sortAsc
                                                                },
                                                                commandType: CommandType.StoredProcedure);
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
                var list = await cn.QueryAsync<TipologiaLezioneDM>("[dbo].[TipologieLezioni_Lista]",
                                                            new
                                                            {
                                                                pIdCliente = idTenant,
                                                                pId = itemKey
                                                            },
                                                            commandType: CommandType.StoredProcedure);

                if (list != null)
                {
                    result = list.FirstOrDefault();
                }
            }
            return result;
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
                cmd.CommandText = "[dbo].[TipologieLezioni_Modifica]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = idTenant;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = entityKey;
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
