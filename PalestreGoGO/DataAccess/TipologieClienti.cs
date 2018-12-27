using Microsoft.Extensions.Configuration;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class TipologieClientiRepository : BaseRepository, ITipologieClientiRepository
    {        
        public TipologieClientiRepository(IConfiguration configuration): base(configuration)
        {
        }

        public async Task<IEnumerable<TipologiaClienteDM>> GetAllAsync(bool includeDeleted = false)
        {
            List<TipologiaClienteDM> result = new List<TipologiaClienteDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "TipologieClienti_Lista";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                await cn.OpenAsync();
                using(var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        result.Add(new TipologiaClienteDM()
                        {
                            Id = dr.GetInt32(dr.GetOrdinal("Id")),
                            Nome = dr.GetString(dr.GetOrdinal("Nome")),
                            Descrizione = dr.IsDBNull(dr.GetOrdinal("Descrizione")) ? default(string) : dr.GetString(dr.GetOrdinal("Descrizione")),
                            DataCancellazione = dr.IsDBNull(dr.GetOrdinal("DataCancellazione")) ? default(DateTime?) : dr.GetDateTime(dr.GetOrdinal("DataCancellazione")),
                        });
                    }
                }
            }
            return result;
        }        
    }
}
