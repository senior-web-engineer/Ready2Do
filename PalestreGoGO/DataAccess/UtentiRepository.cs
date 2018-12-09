﻿using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using PalestreGoGo.DataModel.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace PalestreGoGo.DataAccess
{
    public class UtentiRepository : BaseRepository, IUtentiRepository
    {

        public UtentiRepository(IConfiguration configuration): base(configuration)
        {

        }

        public async Task<List<ClienteFollowed>> GetGlientiFollowedAsync(Guid userId)
        {
            IEnumerable<ClienteFollowed> result = null;
            using (var cn = GetConnection())
            {
                result = await cn.QueryAsync<ClienteFollowed>(StoredProcedure.SP_USER_CLIENTI_FOLLOWED,
                                                            new { pUserId = userId },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            }
            return result?.AsList();
        }

        public async Task<RichiestaRegistrazione> RichiestaRegistrazioneSalvaAsync(string username, string code, Guid? correlationId)
        {
            using (var cn = GetConnection())
            {
                return await cn.QuerySingleAsync<RichiestaRegistrazione>(StoredProcedure.SP_RICHIESTE_REGISTRAZIONE_INSERT,
                    new { pUserCode = code, pUsername = username, pCorrelationId = correlationId },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<RichiestaRegistrazione> CompletaRichiestaRegistrazioneAsync(string username, string code)
        {
            try
            {
                using (var cn = GetConnection())
                {
                    return await cn.QuerySingleAsync<RichiestaRegistrazione>(StoredProcedure.SP_RICHIESTE_REGISTRAZIONE_COMPLETA,
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
