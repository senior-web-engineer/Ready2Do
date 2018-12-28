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

        public async Task<List<ClienteFollowedDM>> GetGlientiFollowedAsync(Guid userId)
        {
            IEnumerable<ClienteFollowedDM> result = null;
            using (var cn = GetConnection())
            {
                result = await cn.QueryAsync<ClienteFollowedDM>(StoredProcedure.SP_USER_CLIENTI_FOLLOWED,
                                                            new { pUserId = userId },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            }
            return result?.AsList();
        }

        public async Task<RichiestaRegistrazioneDM> RichiestaRegistrazioneSalvaAsync(string username, string code, Guid? correlationId)
        {
            using (var cn = GetConnection())
            {
                return await cn.QuerySingleAsync<RichiestaRegistrazioneDM>(StoredProcedure.SP_RICHIESTE_REGISTRAZIONE_INSERT,
                    new { pUserCode = code, pUsername = username, pCorrelationId = correlationId },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<RichiestaRegistrazioneDM> CompletaRichiestaRegistrazioneAsync(string username, string code)
        {
            try
            {
                using (var cn = GetConnection())
                {
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = "RichiestaRegistrazione_Completa";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pUserCode", SqlDbType.VarChar, 1000).Value = code;
                    cmd.Parameters.Add("@pUserName", SqlDbType.VarChar, 500).Value = username;

                    return await cn.QuerySingleAsync<RichiestaRegistrazioneDM>(RichiestaRegistrazione_Completa,
                        new { pUserCode = code, pUsername = username },
                        commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException exc)
            {
                throw new UserConfirmationException($"Impossibile confermare la registrazione dell'utente [{username}] con il codice [{code}]", exc);
            }
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
                await cn.ExecuteAsync(StoredProcedure.SP_USER_FOLLOW_CLIENTE, parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<bool>("pResult");
            }
        }
    }
}
