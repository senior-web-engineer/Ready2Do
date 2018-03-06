﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PalestreGoGo.DataModel;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PalestreGoGo.DataAccess
{
    public class SchedulesRepository : ISchedulesRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<SchedulesRepository> _logger;

        public SchedulesRepository(PalestreGoGoDbContext context, ILogger<SchedulesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> AddScheduleAsync(int idCliente, Schedules schedule)
        {
            if (!schedule.IdCliente.Equals(idCliente)) throw new ArgumentException("Bad Tenant");
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule.Id;
        }

        public async Task<Schedules> GetScheduleAsync(int idCliente, int idSchedule)
        {
            var result = await _context.Schedules.SingleOrDefaultAsync(s => s.Id.Equals(idSchedule) && s.IdCliente.Equals(idCliente));
            return result;
        }

        public IEnumerable<Schedules> GetSchedules(int idCliente, DateTime startDate, DateTime endDate)
        {
            var result = _context.Schedules
                                    .Include(s=>s.TipologiaLezione)
                                    .Include(s=>s.Location)
                                    .Where(s => (s.IdCliente.Equals(idCliente) &&
                                           (Utils.DateTimeFromDateAndTime(s.Data, s.OraInizio) >= startDate) &&
                                           (Utils.DateTimeFromDateAndTime(s.Data, s.OraInizio) <= endDate)));
            return result;
        }

        public IEnumerable<Schedules> GetSchedules(int idCliente, DateTime startDate, DateTime endDate, int idLocation)
        {
            var result = _context.Schedules
                                    .Include(s => s.TipologiaLezione)
                                    .Include(s => s.Location)
                                    .Where(s => (s.IdCliente.Equals(idCliente) &&
                                                 s.IdLocation.Equals(idLocation) &&
                                               (Utils.DateTimeFromDateAndTime(s.Data, s.OraInizio) >= startDate) &&
                                               (Utils.DateTimeFromDateAndTime(s.Data, s.OraInizio) <= endDate)));
            return result;
        }
        public async Task RemoveScheduleAsync(int idCliente, int idSchedule)
        {
            var entity = _context.Schedules
                                .Include(s=>s.Appuntamenti)
                                .Single(tl => tl.IdCliente.Equals(idCliente) && tl.Id.Equals(idSchedule));
            if (entity == null) throw new ArgumentException("Invalid Tenant");
            if (Utils.DateTimeFromDateAndTime(entity.Data, entity.OraInizio) <= DateTime.Now)
            {
                throw new InvalidOperationException("Impossibile cancellare uno schedule passato.");
            }
            using (var trans = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    entity.DataCancellazione = DateTime.Now;
                    //Annulliamo gli appuntamenti
                    foreach(var app in entity.Appuntamenti)
                    {
                        await AppuntamentiRepository.CancellaAppuntamentoAsync(_context, idCliente, app);
                    }
                    await _context.SaveChangesAsync();
                    trans.Commit();
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, $"Errore durante la cancellazione dello Schedule. IdCliente: {idCliente}, IdSchedule:{idSchedule}");
                    trans.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateSchedule(int idCliente, Schedules entity)
        {
            //Attenzione! Non verifichiamo il tenant
            //TODO: Implementare le logiche di verifica di fattibilità
            if (entity.IdCliente != idCliente) throw new ArgumentException("idTenant not valid");
            EntityEntry dbEntityEntry = _context.Entry<Schedules>(entity);
            dbEntityEntry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
