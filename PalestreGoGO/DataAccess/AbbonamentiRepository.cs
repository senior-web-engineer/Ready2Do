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
    public class AbbonamentiRepository : BaseRepository, IAbbonamentiRepository
    {

        public AbbonamentiRepository(IConfiguration configuration) : base(configuration)
        {
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
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[Clienti_Utenti_AbbonamentoSave]", parametri, commandType: CommandType.StoredProcedure);
                abbonamento.Id = parametri.Get<int>("pIdAbbonamento");
            }
            return abbonamento.Id.Value;
        }

        public async Task DeleteAbbonamentoAsync(int idCliente, string userId, int idAbbonamento)
        {
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[Clienti_Utenti_AbbonamentoDelete", new
                {
                    pIdCliente = idCliente,
                    pUserId = userId,
                    pIdAbbonamento = idAbbonamento
                }, commandType: CommandType.StoredProcedure);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <param name="includeExpired"></param>
        /// <param name="includeDeleted"></param>
        /// <param name="idEvento">se specificato limita i risultati ai soli abbonamenti compatibili con l'evento</param>
        /// <returns></returns>
        public async Task<IEnumerable<UtenteClienteAbbonamentoDM>> GetAbbonamentiForUserAsync(int idCliente, string userId, bool includeExpired, bool includeDeleted, int? idEvento = null)
        {
            List<UtenteClienteAbbonamentoDM> result = new List<UtenteClienteAbbonamentoDM>();
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[Clienti_Utenti_AbbonamentoList]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pIdEvento", SqlDbType.Int).Value = idEvento;
                cmd.Parameters.Add("@pIncludeDeleted", SqlDbType.Bit).Value = includeDeleted;
                cmd.Parameters.Add("@pIncludeExpired", SqlDbType.Bit).Value = includeExpired;
                await cn.OpenAsync();
                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        result.Add(new UtenteClienteAbbonamentoDM()
                        {
                            DataCancellazione = dr.IsDBNull(dr.GetOrdinal("DataCancellazione")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("DataCancellazione")),
                            DataCreazione = dr.GetDateTime(dr.GetOrdinal("DataCreazione")),
                            DataInizioValidita = dr.GetDateTime(dr.GetOrdinal("DataCreazione")),
                            Id = dr.GetInt32(dr.GetOrdinal("Id")),
                            IdCliente = dr.GetInt32(dr.GetOrdinal("IdCliente")),
                            IdTipoAbbonamento = dr.GetInt32(dr.GetOrdinal("IdTipoAbbonamento")),
                            Importo = dr.IsDBNull(dr.GetOrdinal("Importo")) ? (decimal?)null : dr.GetDecimal(dr.GetOrdinal("Importo")),
                            ImportoPagato = dr.IsDBNull(dr.GetOrdinal("ImportoPagato")) ? (decimal?)null : dr.GetDecimal(dr.GetOrdinal("ImportoPagato")),
                            IngressiIniziali = dr.IsDBNull(dr.GetOrdinal("IngressiIniziali")) ? (short?)null : dr.GetInt16(dr.GetOrdinal("IngressiIniziali")),
                            IngressiResidui = dr.IsDBNull(dr.GetOrdinal("IngressiResidui")) ? (short?)null : dr.GetInt16(dr.GetOrdinal("IngressiResidui")),
                            Scadenza = dr.GetDateTime(dr.GetOrdinal("Scadenza")),
                            UserId = dr.GetString(dr.GetOrdinal("UserId")),
                            TipoAbbonamento = new TipoAbbonamentoDM()
                            {
                                Costo = dr.IsDBNull(dr.GetOrdinal("Importo")) ? (decimal?)null : dr.GetDecimal(dr.GetOrdinal("Importo")),
                                Id = dr.GetInt32(dr.GetOrdinal("IdTipoAbbonamento")),
                                DurataMesi = dr.IsDBNull(dr.GetOrdinal("DurataMesi")) ? (short?)null : dr.GetInt16(dr.GetOrdinal("DurataMesi")),
                                NumIngressi = dr.IsDBNull(dr.GetOrdinal("NumIngressi")) ? (short?)null : dr.GetInt16(dr.GetOrdinal("NumIngressi")),
                                IdCliente = dr.GetInt32(dr.GetOrdinal("IdCliente")),
                                MaxLivCorsi = dr.IsDBNull(dr.GetOrdinal("MaxLivCorsi")) ? (short?)null : dr.GetInt16(dr.GetOrdinal("MaxLivCorsi")),
                                Nome = dr.GetString(dr.GetOrdinal("Nome"))
                            }
                        });
                    }
                }
            }
            return result;
        }

        public async Task<UtenteClienteAbbonamentoDM> GetAbbonamentoAsync(int idCliente, int idAbbonamento)
        {
            using (var cn = GetConnection())
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
