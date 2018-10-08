using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using PalestreGoGo.DataModel.Exceptions;
using Microsoft.Extensions.Configuration;

namespace PalestreGoGo.DataAccess
{
    public class UtentiRepository : IUtentiRepository
    {

        //private readonly PalestreGoGoDbContext _context;
        private IConfiguration _configuration;
        private readonly ILogger<UtentiRepository> _logger;

        public UtentiRepository(IConfiguration configuration, ILogger<UtentiRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<ClienteFollowed>> GetGlientiFollowedAsync(Guid userId)
        {
            IEnumerable<ClienteFollowed> result = null;
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                result = await cn.QueryAsync<ClienteFollowed>(StoredProcedure.SP_USER_CLIENTI_FOLLOWED,
                                                            new { pUserId = userId },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            }
            return result?.AsList();
        }

        public async Task<RichiestaRegistrazione> RichiestaRegistrazioneSalvaAsync(string username, string code, Guid? correlationId)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
                using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    return await cn.QuerySingleAsync<RichiestaRegistrazione>(StoredProcedure.SP_RICHIESTE_REGISTRAZIONE_COMPLETA,
                        new { pUserCode = code, pUsername = username},
                        commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            catch (SqlException exc)
            {
                throw new UserConfirmationException($"Impossibile confemrare la registrazione dell'utente [{username}] con il codice [{code}]", exc);
            }

        }
    }
}
