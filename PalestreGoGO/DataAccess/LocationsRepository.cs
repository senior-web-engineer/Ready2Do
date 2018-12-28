using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class LocationsRepository : BaseRepository, ILocationsRepository
    {
        public LocationsRepository(IConfiguration config) : base(config)
        {
        }

        #region PRIVATE STUFF
        private Dictionary<string, int> ResolveColumns(SqlDataReader dr)
        {
            var result = new Dictionary<string, int>();
            result.Add("Id", dr.GetOrdinal("Id"));
            result.Add("IdCliente", dr.GetOrdinal("IdCliente"));
            result.Add("Nome", dr.GetOrdinal("Nome"));
            result.Add("Descrizione", dr.GetOrdinal("Descrizione"));
            result.Add("CapienzaMax", dr.GetOrdinal("CapienzaMax"));
            result.Add("DataCreazione", dr.GetOrdinal("DataCreazione"));
            result.Add("DataCancellazione", dr.GetOrdinal("DataCancellazione"));
            return result;
        }

        private async Task<LocationDM> InternalReadLocationAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            LocationDM result = new LocationDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.Nome = dr.GetString(columns["Nome"]);
            result.Descrizione = await dr.IsDBNullAsync(columns["Descrizione"]) ? null : dr.GetString(columns["Descrizione"]);
            result.CapienzaMax = await dr.IsDBNullAsync(columns["CapienzaMax"]) ? default(short?) : dr.GetInt16(columns["CapienzaMax"]);
            result.DataCreazione = dr.GetDateTime(columns["DataCreazione"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            return result;
        }
        #endregion

        public async Task<IEnumerable<LocationDM>> GetAllAsync(int idCliente, bool includeDeleted = false)
        {
            List<LocationDM> result = new List<LocationDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Locations_List]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    var columns = ResolveColumns(dr);
                    while (await dr.ReadAsync())
                    {
                        result.Add(await InternalReadLocationAsync(dr, columns));
                    }
                }
            }
            return result;
        }


        public async Task<LocationDM> GetSingleAsync(int idCliente, int idLocation, bool includeDeleted = false)
        {
            LocationDM result = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Locations_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdLocation", SqlDbType.Int).Value = idLocation;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    var columns = ResolveColumns(dr);
                    if (await dr.ReadAsync())
                    {
                        result = await InternalReadLocationAsync(dr, columns);
                    }
                }
            }
            return result;
        }

        public async Task<int> AddAsync(int idCliente, LocationInputDM location)
        {
            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.Output;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Locations_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = location.Nome;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, -1).Value = location.Descrizione;
                cmd.Parameters.Add("@pCapienzaMax", SqlDbType.SmallInt).Value = location.CapienzaMax;
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                location.Id = (int)parId.Value;
            }
            return location.Id.Value;
        }

        public async Task UpdateAsync(int idCliente, LocationInputDM location)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Locations_Modifica]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdLocation", SqlDbType.Int).Value = location.Id;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = location.Nome;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, -1).Value = location.Descrizione;
                cmd.Parameters.Add("@pCapienzaMax", SqlDbType.SmallInt).Value = location.CapienzaMax;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int idCliente, int idLocation)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Locations_Delete]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdLocation", SqlDbType.Int).Value = idLocation;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
