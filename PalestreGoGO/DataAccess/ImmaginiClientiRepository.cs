using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class ImmaginiClientiRepository : BaseRepository, IImmaginiClientiRepository
    {
        private readonly ILogger<ImmaginiClientiRepository> _logger;
        public ImmaginiClientiRepository(IConfiguration configuration, ILogger<ImmaginiClientiRepository> logger) : base(configuration)
        {
            _logger = logger;
        }
        #region PRIVATE STAFF

        private DataTable InternalBuildDataTableImmmaigini(int idCliente, IEnumerable<ImmagineClienteInputDM> immagini)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("IdCliente", typeof(int));
            dt.Columns.Add("IdTipoImmagine", typeof(int));
            dt.Columns.Add("Nome", typeof(string));
            dt.Columns.Add("Alt", typeof(string));
            dt.Columns.Add("Url", typeof(string));
            dt.Columns.Add("ThumbnailUrl", typeof(string));
            dt.Columns.Add("Descrizione", typeof(string));
            dt.Columns.Add("Ordinamento", typeof(int));
            foreach (var img in immagini)
            {
                if (idCliente != img.IdCliente) { throw new ArgumentException("Wrong Tenant for Image"); }
                var row = dt.NewRow();
                row["Id"] = img.Id;
                row["IdCliente"] = img.IdCliente;
                row["IdTipoImmagine"] = img.IdTipoImmagine;
                row["Nome"] = img.Nome;
                row["Alt"] = img.Alt;
                row["Url"] = img.Url;
                row["ThumbnailUrl"] = img.ThumbnailUrl;
                row["Descrizione"] = img.Descrizione;
                row["Ordinamento"] = img.Ordinamento;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private Dictionary<string, int> ResolveColumnsImmagini(SqlDataReader dr)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("Id", dr.GetOrdinal("Id"));
            result.Add("IdCliente", dr.GetOrdinal("IdCliente"));
            result.Add("IdTipoImmagine", dr.GetOrdinal("IdTipoImmagine"));
            result.Add("Nome", dr.GetOrdinal("Nome"));
            result.Add("Alt", dr.GetOrdinal("Alt"));
            result.Add("Url", dr.GetOrdinal("Url"));
            result.Add("ThumbnailUrl", dr.GetOrdinal("ThumbnailUrl"));
            result.Add("Descrizione", dr.GetOrdinal("Descrizione"));
            result.Add("Ordinamento", dr.GetOrdinal("Ordinamento"));
            result.Add("DataCancellazione", dr.GetOrdinal("DataCancellazione"));
            return result;
        }

        private async Task<ImmagineClienteDM> InternalReadImmagineAsync(SqlDataReader dr, Dictionary<string, int> columns)
        {
            ImmagineClienteDM result = new ImmagineClienteDM();
            result.Id = dr.GetInt32(columns["Id"]);
            result.IdCliente = dr.GetInt32(columns["IdCliente"]);
            result.IdTipoImmagine = dr.GetInt32(columns["IdTipoImmagine"]);
            result.Nome = dr.GetString(columns["Nome"]);
            result.Alt = await dr.IsDBNullAsync(columns["Alt"]) ? null : dr.GetString(columns["Alt"]);
            result.Url = dr.GetString(columns["Url"]);
            result.ThumbnailUrl = await dr.IsDBNullAsync(columns["ThumbnailUrl"]) ? null : dr.GetString(columns["ThumbnailUrl"]);
            result.Descrizione = await dr.IsDBNullAsync(columns["Descrizione"]) ? null : dr.GetString(columns["Descrizione"]);
            result.Ordinamento = dr.GetInt32(columns["Ordinamento"]);
            result.DataCancellazione = await dr.IsDBNullAsync(columns["DataCancellazione"]) ? default(DateTime?) : dr.GetDateTime(columns["DataCancellazione"]);
            return result;
        }

        #endregion

        public async Task AddImagesAsync(int idCliente, IEnumerable<ImmagineClienteInputDM> immagini)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ImmaginiCliente_AddList]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                var parTable = cmd.Parameters.AddWithValue("@pImmagini", InternalBuildDataTableImmmaigini(idCliente, immagini));
                parTable.SqlDbType = SqlDbType.Structured;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        //Ritorna l'immagine cancellata
        public async Task<ImmagineClienteDM> DeleteImageAsync(int idCliente, int idImmagine)
        {
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ImmaginiCliente_Delete]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdImmagine", SqlDbType.Int).Value = idImmagine;
                await cn.OpenAsync();
                //La SP ritorna l'immagine cancellata
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    var columns = ResolveColumnsImmagini(dr);
                    return await InternalReadImmagineAsync(dr, columns);
                }
            }
        }
        public async Task<int> AddImageAsync(int idCliente, ImmagineClienteInputDM immagine)
        {
            if (immagine == null) { throw new ArgumentNullException(nameof(immagine)); }
            if (immagine.IdCliente != idCliente) throw new ArgumentException("Invalid Tenant");
            SqlParameter parId = new SqlParameter("@pId", SqlDbType.Int);
            parId.Direction = ParameterDirection.Output;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ImmaginiCliente_Add]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdTipoImmagine", SqlDbType.Int).Value = immagine.IdTipoImmagine;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = immagine.Nome;
                cmd.Parameters.Add("@pAlt", SqlDbType.NVarChar, 100).Value = immagine.Alt;
                cmd.Parameters.Add("@pUrl", SqlDbType.NVarChar, 1000).Value = immagine.Url;
                cmd.Parameters.Add("@pThumbnailUrl", SqlDbType.NVarChar, 1000).Value = immagine.ThumbnailUrl;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 1000).Value = immagine.Descrizione;
                cmd.Parameters.Add("@pOrdinamento", SqlDbType.Int).Value = immagine.Ordinamento;
                cmd.Parameters.Add(parId);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                immagine.Id = (int)parId.Value;
            }
            return immagine.Id.Value;
        }

        public async Task UpdateImageAsync(int idCliente, ImmagineClienteInputDM immagine)
        {
            if (immagine == null) { throw new ArgumentNullException(nameof(immagine)); }
            if (immagine.IdCliente != idCliente) throw new ArgumentException("Invalid Tenant");
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ImmaginiCliente_Modifica]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdImmagine", SqlDbType.Int).Value = immagine.Id;

                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = immagine.Nome;
                cmd.Parameters.Add("@pAlt", SqlDbType.NVarChar, 100).Value = immagine.Alt;
                cmd.Parameters.Add("@pUrl", SqlDbType.NVarChar, 1000).Value = immagine.Url;
                cmd.Parameters.Add("@pThumbnailUrl", SqlDbType.NVarChar, 1000).Value = immagine.ThumbnailUrl;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 1000).Value = immagine.Descrizione;
                cmd.Parameters.Add("@pOrdinamento", SqlDbType.Int).Value = immagine.Ordinamento;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<ImmagineClienteDM> GetImage(int idCliente, int idImmagine, bool includeDeleted = false)
        {
            ImmagineClienteDM result = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ImmaginiCliente_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdImmagine", SqlDbType.Int).Value = idImmagine;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Int).Value = includeDeleted;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        var columns = ResolveColumnsImmagini(dr);
                        result = await InternalReadImmagineAsync(dr, columns);
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<ImmagineClienteDM>> GetImages(int idCliente, TipoImmagineDM? tipo = null, bool includeDeleted = false)
        {
            _logger.LogDebug($"Begin GetImages(idCliente:{idCliente}, tipo:{tipo}, includeDeleted:{includeDeleted})");
            List<ImmagineClienteDM> result = new List<ImmagineClienteDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[ImmaginiCliente_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pIdTipoImmagine", SqlDbType.Int).Value = tipo;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;

                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        var columns = ResolveColumnsImmagini(dr);
                        while (await dr.ReadAsync())
                        {
                            result.Add(await InternalReadImmagineAsync(dr, columns));
                        }
                    }
                }
            }
            _logger.LogDebug($"End GetImages(idCliente:{idCliente}, tipo:{tipo}, includeDeleted:{includeDeleted}). Num. Items Returned: {result?.Count ?? 0}");
            return result;
        }
    }
}
