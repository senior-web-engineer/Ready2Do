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

        public async Task<string> RichiestaRegistrazioneSalvaAsync(string username, DateTime expiration, Guid? correlationId, int? idRefereer)
        {
            SqlParameter parCode = new SqlParameter("@pUserCode", SqlDbType.VarChar, 1000);
            parCode.Direction = ParameterDirection.Output;

            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "RichiestaRegistrazione_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUsername", SqlDbType.VarChar, 500).Value = username;
                cmd.Parameters.Add("@pCorrelationId", SqlDbType.UniqueIdentifier).Value = correlationId;
                cmd.Parameters.Add("@pExpiration", SqlDbType.DateTime2).Value = expiration;
                cmd.Parameters.Add("@pRefereer", SqlDbType.Int).Value = idRefereer;
                cmd.Parameters.Add(parCode);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            return parCode.Value as string;
        }

        public async Task<EsitoConfermaRegistrazioneDM> CompletaRichiestaRegistrazioneAsync(string username, string code)
        {
            EsitoConfermaRegistrazioneDM result = new EsitoConfermaRegistrazioneDM();
            SqlParameter parEsito = new SqlParameter("@pEsitoConferma", SqlDbType.Int);
            parEsito.Direction = ParameterDirection.Output;
            SqlParameter parIdCliente = new SqlParameter("@pIdCliente", SqlDbType.Int);
            parIdCliente.Direction = ParameterDirection.Output;
            SqlParameter parIdRefereer = new SqlParameter("@pIdRefereer", SqlDbType.Int);
            parIdRefereer.Direction = ParameterDirection.Output;
            try
            {
                using (var cn = GetConnection())
                {
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = "RichiestaRegistrazione_Completa";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pUserCode", SqlDbType.VarChar, 1000).Value = code;
                    cmd.Parameters.Add("@pUserName", SqlDbType.VarChar, 500).Value = username;
                    cmd.Parameters.Add(parEsito);
                    cmd.Parameters.Add(parIdCliente);
                    cmd.Parameters.Add(parIdRefereer);
                    await cn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    result.EsitoConferma = (bool)parEsito.Value;
                    result.IdCliente = (int?)parIdCliente.Value;
                    result.IdRefereer = (int?)parIdRefereer.Value;
                }
            }
            catch (SqlException exc)
            {
                throw new UserConfirmationException($"Impossibile confermare la registrazione dell'utente [{username}] con il codice [{code}]", exc);
            }
            return result;
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
