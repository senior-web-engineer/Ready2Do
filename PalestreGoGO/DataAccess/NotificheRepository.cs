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
using Dapper;

namespace PalestreGoGo.DataAccess
{
    public class NotificheRepository : INotificheRepository
    {

        //private readonly PalestreGoGoDbContext _context;
        private IConfiguration _configuration;
        private readonly ILogger<UtentiRepository> _logger;

        public NotificheRepository(IConfiguration configuration, ILogger<UtentiRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<long> AddNotificaAsync(NotificaDM notifica)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.ExecuteScalarAsync<long>("[dbo].[Notifiche_Add]",
                                                    new
                                                    {
                                                        pIdTipo = notifica.IdTipo,
                                                        pIdUtente = notifica.UserRef.UserId,
                                                        pIdCliente = notifica.IdCliente,
                                                        pTitolo = notifica.Titolo,
                                                        pTesto = notifica.Testo,
                                                        pDataInizioVisibilita = notifica.DataInizioVisibilita,
                                                        pDataFineVisibilita = notifica.DataFineVisibilita,
                                                    }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<NotificaConTipoDM>> GetNotificheAsync(UserReferenceDM userRef, int? idCliente = null, FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                return await cn.QueryAsync<NotificaConTipoDM, TipologiaNotifica, UserReferenceDM, NotificaConTipoDM>("[dbo].[Notifiche_Lista]",
                                                    (notifica, tipoNotifica, user) =>
                                                    {
                                                        notifica.UserRef = user;
                                                        notifica.Tipo = tipoNotifica;
                                                        return notifica;
                                                    },
                                                    new { },
                                                    splitOn: "IdUtente, IdTipo",
                                                    commandType: System.Data.CommandType.StoredProcedure
                                                    );
            }
        }

        public async Task UpdateNotifica(long idNotifica, DateTime? dataView, DateTime? dataDismiss)
        {
            using (var cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await cn.ExecuteAsync("[dbo].[Notifiche_Aggiorna]", 
                                        new { pIdNotifica = idNotifica, pDataVisualizzazione = dataView, pDataDismiss = dataDismiss }, 
                                        commandType: System.Data.CommandType.StoredProcedure);
            }
        }        
    }
}
