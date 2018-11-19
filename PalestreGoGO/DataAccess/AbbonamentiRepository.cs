using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class AbbonamentiRepository : IAbbonamentiRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AbbonamentiRepository> _logger;

        public AbbonamentiRepository(IConfiguration configuration, ILogger<AbbonamentiRepository> logger)
        {
            this._logger = logger;
            this._configuration = configuration;
        }

        public async Task<int> SaveAbbonamentoAsync(int idCliente, UtenteClienteAbbonamentoDM abbonamento)
        {
            var parametri = new DynamicParameters(
                    new
                    {
                        pIdCliente = abbonamento.IdCliente,
                        pUserId = abbonamento.UserId,
                        pIdTipoAbbonamento = abbonamento.IdTipoAbbonamento,
                        pDataInizioValidita = abbonamento.DataInizioValidita,
                        pScadenza = abbonamento.Scadenza,
                        pIngressiIniziali = abbonamento.IngressiIniziali,
                        pIngressiResidui = abbonamento.IngressiResidui,
                        pImporto = abbonamento.Importo,
                        pImportoPagato = abbonamento.ImportoPagato
                    });
            parametri.Add("pIdAbbonamento", abbonamento.Id, DbType.Int32, ParameterDirection.InputOutput);
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await cn.ExecuteAsync("[Clienti_Utenti_AbbonamentoSave]", parametri, commandType: CommandType.StoredProcedure);
                abbonamento.Id = parametri.Get<int>("pIdAbbonamento");
            }
            return abbonamento.Id.Value;
        }

        public async Task DeleteAbbonamentoAsync(int idCliente, string userId, int idAbbonamento)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await cn.ExecuteAsync("[Clienti_Utenti_AbbonamentoDelete", new
                {
                    pIdCliente = idCliente,
                    pUserId = userId,
                    pIdAbbonamento = idAbbonamento
                }, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task<IEnumerable<UtenteClienteAbbonamentoDM>> GetAbbonamentiForUserAsync(int idCliente, string userId, bool includeExpired, bool includeDeleted)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.QueryAsync<UtenteClienteAbbonamentoDM>("[Clienti_Utenti_AbbonamentoList]", new
                {
                    pIdCliente = idCliente,
                    pUserId = userId,
                    pIncludeDeleted = includeDeleted,
                    pIncludeExpired = includeExpired
                }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<UtenteClienteAbbonamentoDM> GetAbbonamentoAsync(int idCliente, int idAbbonamento)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.QuerySingleAsync<UtenteClienteAbbonamentoDM>("[Clienti_Utenti_AbbonamentoGet]", new
                {
                    pIdCliente = idCliente,
                    pIdAbbonamento = idAbbonamento
                }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
