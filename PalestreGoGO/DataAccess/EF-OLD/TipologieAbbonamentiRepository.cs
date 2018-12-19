using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel;
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
    public class TipologieAbbonamentiRepository : BaseRepository, ITipologieAbbonamentiRepository
    {
        public TipologieAbbonamentiRepository(IConfiguration configuration) : base(configuration)
        {
        }

        private Dictionary<string, int> InternalGetColumnsOrdinal(SqlDataReader dr)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", dr.GetOrdinal("Id"));
            result.Add("IdCliente", dr.GetOrdinal("IdCliente"));
            result.Add("Nome", dr.GetOrdinal("Nome"));
            result.Add("DurataMesi", dr.GetOrdinal("DurataMesi"));
            result.Add("NumIngressi", dr.GetOrdinal("NumIngressi"));
            result.Add("Costo", dr.GetOrdinal("Costo"));
            result.Add("MaxLivCorsi", dr.GetOrdinal("MaxLivCorsi"));
            result.Add("ValidoDal", dr.GetOrdinal("ValidoDal"));
            result.Add("ValidoFinoAl", dr.GetOrdinal("ValidoFinoAl"));
            result.Add("DataCreazione", dr.GetOrdinal("DataCreazione"));
            result.Add("DataCancellazione", dr.GetOrdinal("DataCancellazione"));
            return result;
        }

        private async Task<TipologiaAbbonamentoDM> InternalReadTipologiaAbbonamento(SqlDataReader dr, Dictionary<string, int> columns)
        {
            var result = new TipologiaAbbonamentoDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.Nome = dr.GetString(columns["Nome"]);
            result.DurataMesi = await dr.IsDBNullAsync(columns["DurataMesi"]) ? default(short?) : dr.GetInt16(columns["DurataMesi"]);
            result.NumIngressi = await dr.IsDBNullAsync(columns["NumIngressi"]) ? default(short?) : dr.GetInt16(columns["NumIngressi"]);
            result.Costo = await dr.IsDBNullAsync(columns["Costo"]) ? default(decimal?) : dr.GetDecimal(columns["Costo"]);
            result.MaxLivCorsi = await dr.IsDBNullAsync(columns["MaxLivCorsi"]) ? default(short?) : dr.GetInt16(columns["MaxLivCorsi"]);
            result.ValidoDal = dr.GetDateTime(columns["ValidoDal"]);
            result.ValidoFinoAl = await dr.IsDBNullAsync(columns["ValidoFinoAl"]) ? default(DateTime?) : dr.GetDateTime(columns["ValidoFinoAl"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            return result;
        }

        public async Task<Tuple<IEnumerable<TipologiaAbbonamentoDM>, int>> GetListAsync(int idCliente, int pageSize = 25, int pageNumber = 1, int? id = null,
                                                                                         string sortColumn = "DataCreazione", bool sortAscending = false,
                                                                                         bool includiCancellati = false, bool includiNonAttivi = false,
                                                                                         DateTime? dataValutazione = null)
        {
            SqlParameter paramNumRecords = new SqlParameter("@pTotalNumRecords", SqlDbType.Int);
            paramNumRecords.Direction = ParameterDirection.Output;
            Dictionary<string, int> columns = null;
            List<TipologiaAbbonamentoDM> result = new List<TipologiaAbbonamentoDM>();
            int numRecords = -1;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieAbbonamenti_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@pPageSize", SqlDbType.Int).Value = pageSize;
                cmd.Parameters.Add("@pPageNumber", SqlDbType.Int).Value = pageNumber;
                cmd.Parameters.Add("@pSortColumn", SqlDbType.VarChar, 50).Value = sortColumn;
                cmd.Parameters.Add("@pOrderAscending", SqlDbType.Bit).Value = sortAscending;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = sortAscending;
                cmd.Parameters.Add("@pIncludeNotActive", SqlDbType.Bit).Value = sortAscending;
                cmd.Parameters.Add(paramNumRecords);
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    columns = InternalGetColumnsOrdinal(dr);
                    while (await dr.ReadAsync())
                    {
                        result.Add(await InternalReadTipologiaAbbonamento(dr, columns));
                    }
                    numRecords = (int)paramNumRecords.Value;
                }
            }
            return new Tuple<IEnumerable<TipologiaAbbonamentoDM>, int>(result, numRecords);
        }

        public async Task<TipologiaAbbonamentoDM> GetOneAsync(int idCliente, int id)
        {
            return (await GetListAsync(idCliente, 25, 1, id)).Item1.SingleOrDefault();
        }

        public async Task<int> AddAsync(int idCliente, TipologiaAbbonamentoDM entity)
        {
            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.Output;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieAbbonamenti_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = entity.Nome;
                cmd.Parameters.Add("@pDurataMesi", SqlDbType.SmallInt).Value = entity.DurataMesi;
                cmd.Parameters.Add("@pNumIngressi", SqlDbType.SmallInt).Value = entity.NumIngressi;
                cmd.Parameters.Add("@pMaxLivCorsi", SqlDbType.SmallInt).Value = entity.MaxLivCorsi;
                cmd.Parameters.Add("@pCosto", SqlDbType.Decimal).Value = entity.Costo;
                cmd.Parameters.Add("@pValidoDal", SqlDbType.DateTime2).Value = entity.ValidoDal;
                cmd.Parameters.Add("@ValidoFinoAl", SqlDbType.DateTime2).Value = entity.ValidoFinoAl;
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            return (int)parId.Value;
        }

        public async Task UpdateAsync(int idCliente, TipologiaAbbonamentoDM entity)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieAbbonamenti_Modifica]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = entity.Id;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = entity.Nome;
                cmd.Parameters.Add("@pDurataMesi", SqlDbType.SmallInt).Value = entity.DurataMesi;
                cmd.Parameters.Add("@pNumIngressi", SqlDbType.SmallInt).Value = entity.NumIngressi;
                cmd.Parameters.Add("@pMaxLivCorsi", SqlDbType.SmallInt).Value = entity.MaxLivCorsi;
                cmd.Parameters.Add("@pCosto", SqlDbType.Decimal).Value = entity.Costo;
                cmd.Parameters.Add("@pValidoDal", SqlDbType.DateTime2).Value = entity.ValidoDal;
                cmd.Parameters.Add("@ValidoFinoAl", SqlDbType.DateTime2).Value = entity.ValidoFinoAl;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int idCliente, int id)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieAbbonamenti_Delete]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> CheckNomeAsync(int idCliente, string nome)
        {
            SqlParameter parResult = new SqlParameter("@pResult", SqlDbType.Bit);
            parResult.Direction = ParameterDirection.Output;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[TipologieAbbonamenti_CheckNome]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pNomeTipoAbbonamento", SqlDbType.NVarChar, 100).Value = nome;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add(parResult);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return (bool)parResult.Value;
            }
        }
    }
}
