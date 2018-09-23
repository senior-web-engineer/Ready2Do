using PalestreGoGo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using PalestreGoGo.DataModel;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel.Exceptions;

namespace PalestreGoGo.DataAccess
{
    public class AppuntamentiRepository : IAppuntamentiRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<AppuntamentiRepository> _logger;

        public AppuntamentiRepository(ILogger<AppuntamentiRepository> logger, PalestreGoGoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<int> AddAppuntamentoAsync(int idCliente, Appuntamenti appuntamento)
        {
            if (appuntamento == null) throw new ArgumentNullException(nameof(appuntamento));
            if (!appuntamento.IdCliente.Equals(idCliente)) throw new ArgumentException("Wrong Tenant");
            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.IdCliente.Equals(idCliente) && (s.Id.Equals(appuntamento.ScheduleId)));
                    if (schedule == null) throw new ArgumentException("Ivalid Schedule");
                    //Se non è un guest ==> deve avere un abbonamento da cui scalare gli ingressi
                    if (appuntamento.UserId != null)
                    {
                        //recupero l'abbonamento per l'utente, non deve essere scaduto ed avere ancora ingressi disponibili
                        var abbonamento = await _context.AbbonamentiUtenti.FirstOrDefaultAsync(a => a.IdCliente.Equals(idCliente) &&
                                                                                                    a.UserId.Equals(appuntamento.UserId) &&
                                                                                                    a.Scadenza >= schedule.Data &&
                                                                                                    a.IngressiResidui > 0);
                        if (abbonamento == null)
                        {
                            _logger.LogWarning($"Cliente {idCliente} - non è stato trovato un abbonamento valido per l'utente {appuntamento.UserId}.");
                            throw new AbbonamentoNotFoundedException($"Cliente {idCliente} - non è stato trovato un abbonamento valido per l'utente {appuntamento.UserId}.");
                        }
                        _context.Appuntamenti.Add(appuntamento); //Salviamo l'appuntamento
                        abbonamento.IngressiResidui--; //decrementiamo gli ingressi
                    }
                    schedule.PostiResidui--; //decrementiamo i posti disponibili
                    await _context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, $"Errore durante l'aggiunta dell'appuntamento. IdCliente: {idCliente}, IdUser:{appuntamento?.UserId }, ScheduleId:{appuntamento?.ScheduleId}");
                    trans.Rollback();
                    throw;
                }
            }
            return appuntamento.Id;
        }

        public async Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento)
        {
            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var appuntamento = await _context.Appuntamenti.SingleAsync(tl => tl.IdCliente.Equals(idCliente) && tl.Id.Equals(idAppuntamento));
                    var schedule = await _context.Schedules.SingleAsync(s => s.Id.Equals(appuntamento.ScheduleId));
                    appuntamento.DataCancellazione = DateTime.Now;
                    //Se la cancellazione avviene entro il termine previsto, rimborsiamo l'ingresso
                    if (schedule.CancellabileFinoAl >= DateTime.Now)
                    {
                        var abbonamento = await _context.AbbonamentiUtenti.FirstOrDefaultAsync(au => au.IdCliente.Equals(idCliente) && au.UserId.Equals(appuntamento.UserId));
                        // Avoid overflow
                        if ((abbonamento != null) && (abbonamento.IngressiResidui < Int16.MaxValue))
                        {
                            abbonamento.IngressiResidui++;
                        }
                    }
                    schedule.PostiResidui++;
                    await _context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"Errore durante la cancellazione dell'appuntamento. IdCliente: {idCliente}, IdAppuntamento:{idAppuntamento}");
                    trans.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<Appuntamenti> GetAppuntamentiForSchedule(int idCliente, int idSchedule)
        {
            var appuntamenti = _context.Appuntamenti.Where(a => a.Id.Equals(idSchedule) && a.IdCliente.Equals(idCliente));
            return appuntamenti;
        }

        public async Task<Appuntamenti> GetAppuntamentoAsync(int idCliente, int idAppuntamento)
        {
            return await _context.Appuntamenti.Where(a => a.IdCliente.Equals(idCliente) && a.Id.Equals(idAppuntamento)).FirstOrDefaultAsync();
        }

        public async Task<Appuntamenti> GetAppuntamentoForScheduleAsync(int idCliente, int idSchedule, Guid userId)
        {
            return await _context.Appuntamenti.Where(a => a.IdCliente.Equals(idCliente) && a.ScheduleId.Equals(idSchedule) && a.UserId.Equals(userId)).FirstOrDefaultAsync();
        }

        public IEnumerable<Appuntamenti> GetAppuntamentiForUser(int idCliente, Guid userId, bool includePast=false)
        {
            return _context
                    .Appuntamenti
                    .AsNoTracking()
                    .Include(a => a.Cliente)
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.TipologiaLezione)
                    .Where(a => a.IdCliente.Equals(idCliente) && a.UserId.Equals(userId) );
        }

        public IEnumerable<Appuntamenti> GetAppuntamentiForUser(Guid userId, bool includePast)
        {
            return _context
                    .Appuntamenti
                    .AsNoTracking()
                    .Include(a => a.Cliente)
                    .Include(a => a.Schedule)
                        .ThenInclude(s => s.TipologiaLezione)
                    .Where(a => a.UserId.Equals(userId) && (!includePast || a.Schedule.Data > DateTime.Now));
        }
    }
}
