using Dapper;
using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel.Exceptions;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class UtentiRepository : BaseRepository, IUtentiRepository
    {

        public UtentiRepository(IConfiguration configuration): base(configuration)
        {

        }

        public async Task<List<ClienteFollowedDM>> GetGlientiFollowedAsync(string userId)
        {
            IEnumerable<ClienteFollowedDM> result = null;
            using (var cn = GetConnection())
            {
                result = await cn.QueryAsync<ClienteFollowedDM>("[dbo].[Utenti_ClientiFollowed]",
                                                            new { pUserId = userId },
                                                            commandType: CommandType.StoredProcedure);
            }
            return result?.AsList();
        }     

        public async Task<bool> UserFollowClienteAsync(string userId, int idCliente)
        {
            var parameters = new DynamicParameters(new
            {
                pUserId = userId,
                pIdCliente = idCliente
            });
            parameters.Add("pResult", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("Utenti_ClienteIsFollowed", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<bool>("pResult");
            }
        }
    }
}
