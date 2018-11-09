using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class TipologieLezioniRepository : ITipologieLezioniRepository
    {
        //private readonly PalestreGoGoDbContext _context;

        private IConfiguration _configuration;
        private readonly ILogger<TipologieLezioniRepository> _logger;

        public TipologieLezioniRepository(IConfiguration configuration, ILogger<TipologieLezioniRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<TipologieLezioni>> GetListAsync(int idTenant, string sortColumn = null, bool sortAsc = true, int pageNumber = 1, int pageSize = 1000)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.QueryAsync<TipologieLezioni>("[dbo].[TipologieLezioni_Lista]",
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
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [dbo].[TipologieLezioni] WHERE DataCancellazione IS NULL");
            }
        }

        public async Task<TipologieLezioni> GetAsync(int idTenant, int itemKey)
        {
            TipologieLezioni result = null;
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var list = await cn.QueryAsync<TipologieLezioni>("[dbo].[TipologieLezioni_Lista]",
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


        public async Task<int> AddAsync(int idTenant, TipologieLezioni entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                entity.Id = (int)parId.Value;
            }
            return entity.Id;
        }

        public async Task UpdateAsync(int idTenant, TipologieLezioni entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int idTenant, int entityKey)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
    }
}
