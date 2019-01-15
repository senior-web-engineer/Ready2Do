using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ready2do.model.common;

namespace PalestreGoGo.DataAccess
{
    public class ClientiUtentiRepository : BaseRepository, IClientiUtentiRepository
    {
        internal static Dictionary<string, int> GetColumnsOrdinals(SqlDataReader dr, Dictionary<string, string> aliases = null)
        {
            Func<string, string> getColumnName = (s) => { if ((aliases != null) && aliases.ContainsKey(s)) return aliases[s]; else return s; };

            Dictionary<string, int> result = new Dictionary<string, int>();
            result["UserId"] = dr.GetOrdinal(getColumnName("UserId"));
            result["IdCliente"] = dr.GetOrdinal(getColumnName("IdCliente"));
            result["Nome"] = dr.GetOrdinal(getColumnName("Nome"));
            result["Cognome"] = dr.GetOrdinal(getColumnName("Cognome"));
            result["UserDisplayName"] = dr.GetOrdinal(getColumnName("UserDisplayName"));
            result["DataCreazione"] = dr.GetOrdinal(getColumnName("DataCreazione"));
            result["DataAggiornamento"] = dr.GetOrdinal(getColumnName("DataAggiornamento"));
            result["DataCancellazione"] = dr.GetOrdinal(getColumnName("DataCancellazione"));
            return result;
        }

        internal static async Task<T> ReadUtenteClienteAsync<T>(SqlDataReader reader, Dictionary<string, int> columns) where T : UtenteClienteDM, new()
        {
            T result = new T();
            result.Nome = reader.GetString(columns["Nome"]);
            result.Cognome = reader.GetString(columns["Cognome"]);
            result.DisplayName = reader.GetString(columns["UserDisplayName"]);
            result.UserId = reader.GetString(columns["UserId"]);
            result.IdCliente = reader.GetInt32(columns["IdCliente"]);
            result.DataAssociazione = reader.GetDateTime(columns["DataCreazione"]);
            result.UtlimoAggiornamento = reader.GetDateTime(columns["DataAggiornamento"]);
            result.DataCancellazione = await reader.IsDBNullAsync(columns["DataCancellazione"]) ? (DateTime?)null : reader.GetDateTime(columns["DataCancellazione"]);
            return result;
        }

        public ClientiUtentiRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<UtenteClienteDM>> GetUtentiCliente(int idCliente, bool includeStato,
                                                                        int pageNumber = 1, int pageSize = 25,
                                                                        ClientiUtentiListaSortColumnDM colSort = ClientiUtentiListaSortColumnDM.Nome,
                                                                        SortOrderDM sortOrder = SortOrderDM.Ascending)
        {
            List<UtenteClienteDM> result = new List<UtenteClienteDM>();
            UtenteClienteDM item = null;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_Utenti_Lista]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@pIdCliente", idCliente));
                cmd.Parameters.Add(new SqlParameter("@pIncludeStato", includeStato));
                cmd.Parameters.Add(new SqlParameter("@pPageSize", pageSize));
                cmd.Parameters.Add(new SqlParameter("@pPageNumber", pageNumber));
                cmd.Parameters.Add(new SqlParameter("@pSortColumn", colSort.ToString("F")));
                cmd.Parameters.Add(new SqlParameter("@pOrderAscending", (sortOrder == SortOrderDM.Ascending)));

                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var columns = GetColumnsOrdinals(reader);
                    while (reader.Read())
                    {
                        if (includeStato)
                        {
                            item = await ReadUtenteClienteAsync<UtenteClienteDetailsDM>(reader, columns);
                            ((UtenteClienteDetailsDM)item).Stato = await ReadStatoUtenteAsync(reader);
                        }
                        else
                        {
                            item = await ReadUtenteClienteAsync<UtenteClienteDM>(reader, columns);
                        }
                        result.Add(item);
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// Ritorna i dati di base di un Utente associato ad un cliente senza valorizzare lo stato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UtenteClienteDM> GetUtenteCliente(int idCliente, string userId, bool includeStato)
        {
            UtenteClienteDM result = null;
            int idColIdUser, idColNome, idColCognome, idColDisplpayName, idColDataCreaz, idColDataMod, idColDataDel;
            using (var cn = GetConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[Clienti_Utenti_Get]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pUserId", SqlDbType.VarChar, 100).Value = userId;
                cmd.Parameters.Add("@pIncludeStato", SqlDbType.Bit).Value = includeStato;
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var columns = GetColumnsOrdinals(reader);
                    if (reader.HasRows)
                    {
                        if (includeStato)
                        {
                            result = await ReadUtenteClienteAsync<UtenteClienteDetailsDM>(reader, columns);
                            ((UtenteClienteDetailsDM)result).Stato = await ReadStatoUtenteAsync(reader);
                        }
                        else
                        {
                            result = await ReadUtenteClienteAsync<UtenteClienteDM>(reader, columns);
                        }                        
                    }
                }
            }
            return result;
        }

        private async Task<ClienteUtenteStato> ReadStatoUtenteAsync(SqlDataReader reader)
        {
            ClienteUtenteStato result = ClienteUtenteStato.Unknown;
            int idColHasAbbAtt = -1, idColStatoPag = -1, idColHasAbbDel = -1, idColHasAbbScad = -1, idColHasCertScad = -1,
                    idColHasCertValid = -1;
            bool hasCertScadIsNull, hasCertValidIsNull;

            idColHasAbbAtt = reader.GetOrdinal("HasAbbonamentoAttivo");
            idColStatoPag = reader.GetOrdinal("StatoPagamentoAbbonamentoAttivo");
            idColHasAbbDel = reader.GetOrdinal("HasAbbonamentoCancellato");
            idColHasAbbScad = reader.GetOrdinal("HasAbbonamentoScaduto");
            idColHasCertScad = reader.GetOrdinal("HasCertificatoScaduto");
            idColHasCertValid = reader.GetOrdinal("HasCertificatoValido");

            if (!(await reader.IsDBNullAsync(idColHasAbbAtt)) && reader.GetByte(idColHasAbbAtt) > 0)
            {
                result |= ClienteUtenteStato.AbbonamentoValido;
            }
            if (!(await reader.IsDBNullAsync(idColStatoPag)))
            {
                switch (reader.GetByte(idColStatoPag))
                {
                    case 1:
                    case 2:
                        result |= ClienteUtenteStato.PagamentoDovuto;
                        break;
                    case 3:
                        result |= ClienteUtenteStato.NessunPagamentoDovuto;
                        break;
                }
            }
            if (!(await reader.IsDBNullAsync(idColHasAbbScad)) && reader.GetByte(idColHasAbbScad) > 0)
            {
                result |= ClienteUtenteStato.AbbonamenoScaduto;
            }
            hasCertScadIsNull = await reader.IsDBNullAsync(idColHasCertScad);
            hasCertValidIsNull = await reader.IsDBNullAsync(idColHasCertValid);
            if (!hasCertScadIsNull && reader.GetByte(idColHasCertScad) > 0)
            {
                result |= ClienteUtenteStato.CertificatoScaduto;
            }
            if (!hasCertValidIsNull && reader.GetByte(idColHasCertValid) > 0)
            {
                result |= ClienteUtenteStato.CertificatoValido;
            }
            if (hasCertScadIsNull && hasCertValidIsNull)
            {
                result |= ClienteUtenteStato.CertificatoNonPresentato;
            }
            return result;
        }

        #region Associazione/Disassociazione Utenti

        public async Task AssociaUtenteAsync(int idCliente, string userId, string nome, string cognome, string displayName)
        {
            var parameters = new DynamicParameters(new
            {
                pUserId = userId,
                pIdCliente = idCliente,
                pNome = nome,
                pCognome = cognome,
                pDisplayName = displayName
            });
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[dbo].[Clienti_Utenti_Associa]", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DisassociaUtenteFollowerAsync(int idCliente, string userId)
        {
            var parameters = new DynamicParameters(new
            {
                pUserId = userId,
                pIdCliente = idCliente,
            });
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[dbo].[Clienti_Utenti_Disassocia]", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region Gestione Certificati

        public async Task<int> SaveCertificatoUtente(UtenteClienteCertificatoDM certificato)
        {
            var parametri = new DynamicParameters(new
            {
                pIdCliente = certificato.IdCliente,
                pUserId = certificato.UserId,
                pDataPresentazione = certificato.DataPresentazione,
                pDataScadenza = certificato.DataScadenza,
            });
            parametri.Add("pIdCertificato", certificato.Id, DbType.Int32, ParameterDirection.InputOutput);
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[Clienti_Utenti_CertificatoSave]", parametri, commandType: CommandType.StoredProcedure);
                certificato.Id = parametri.Get<int>("pIdCertificato");
            }
            return certificato.Id.Value;
        }

        public async Task DeleteCertificatoUtente(int idCliente, string userId, int idCertificato)
        {
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[Clienti_Utenti_CertificatoDelete]",
                        new
                        {
                            pIdCliente = idCliente,
                            pUserId = userId,
                            pIdCertificato = idCertificato
                        },
                        commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UtenteClienteCertificatoDM>> GetCertificatiUtente(int idCliente, string userId, bool includeExpired = true, bool includeDeleted = false)
        {
            string sql = @"SELECT Id, UserId, IdCliente, DataPresentazione, DataScadenza, Note, DataCancellazione FROM [ClientiUtentiCertificati] 
                            WHERE IdCliente = @idCliente
                            AND UserId = @userId";
            if (!includeExpired)
            {
                sql = $"{sql} AND DataScadenza < SYSDATETIME()";
            }
            if (!includeDeleted)
            {
                sql = $"{sql} AND DataCancellazione IS NULL";
            }
            using (var cn = GetConnection())
            {
                return await cn.QueryAsync<UtenteClienteCertificatoDM>(sql, new { idCliente, userId });
            }
        }
        #endregion

    }
}
