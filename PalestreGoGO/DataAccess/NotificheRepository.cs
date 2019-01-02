using Dapper;
using Microsoft.Extensions.Configuration;
using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PalestreGoGo.DataAccess
{
    public class NotificheRepository : BaseRepository, INotificheRepository
    {


        public NotificheRepository(IConfiguration configuration): base(configuration)
        {
        }

        public async Task<long> AddNotificaAsync(NotificaDM notifica)
        {
            using (var cn = GetConnection())
            {
                return await cn.ExecuteScalarAsync<long>("[dbo].[Notifiche_Add]",
                                                    new
                                                    {
                                                        pIdTipo = notifica.IdTipo,
                                                        pUserId = notifica.UserId,
                                                        pIdCliente = notifica.IdCliente,
                                                        pTitolo = notifica.Titolo,
                                                        pTesto = notifica.Testo,
                                                        pDataInizioVisibilita = notifica.DataInizioVisibilita,
                                                        pDataFineVisibilita = notifica.DataFineVisibilita,
                                                    }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<NotificaConTipoDM>> GetNotificheAsync(string userId, int? idCliente = null, FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive)
        {
            using (var cn = GetConnection())
            {
                return await cn.QueryAsync<NotificaConTipoDM, TipologiaNotificaDM, NotificaConTipoDM>("[dbo].[Notifiche_Lista]",
                                                    (notifica, tipoNotifica) =>
                                                    {
                                                        notifica.Tipo = tipoNotifica;
                                                        return notifica;
                                                    },
                                                    new { },
                                                    splitOn: "IdTipo",
                                                    commandType: System.Data.CommandType.StoredProcedure
                                                    );
            }
        }

        public async Task UpdateNotifica(long idNotifica, DateTime? dataView, DateTime? dataDismiss)
        {
            using (var cn = GetConnection())
            {
                await cn.ExecuteAsync("[dbo].[Notifiche_Aggiorna]", 
                                        new { pIdNotifica = idNotifica, pDataVisualizzazione = dataView, pDataDismiss = dataDismiss }, 
                                        commandType: System.Data.CommandType.StoredProcedure);
            }
        }        
    }
}
