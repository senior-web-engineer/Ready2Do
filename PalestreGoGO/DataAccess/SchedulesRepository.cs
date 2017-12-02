using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PalestreGoGo.DataModel;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace PalestreGoGo.DataAccess
{
    public class SchedulesRepository : ISchedulesRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public SchedulesRepository(PalestreGoGoDbContext context)
        {
            _context = context;
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
             var result = _context.Schedules.Where(s => (s.IdCliente.Equals(idCliente) &&
                                            (Utils.DateTimeFromDateAndTime(s.Data, s.OraInizio) >= startDate) &&
                                            (Utils.DateTimeFromDateAndTime(s.Data, s.OraInizio) <= endDate)));
            return result;
        }

        public async Task RemoveScheduleAsync(int idCliente, int idSchedule)
        {
            var entity = _context.Schedules.Single(tl => tl.IdCliente.Equals(idCliente) && tl.Id.Equals(idSchedule));
            if (entity == null) throw new ArgumentException("Invalid Tenant");
            entity.DataCancellazione = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSchedule(int idCliente, Schedules entity)
        {
            //Attenzione! Non verifichiamo il tenant
            if (entity.IdCliente != idCliente) throw new ArgumentException("idTenant not valid");
            EntityEntry dbEntityEntry = _context.Entry<Schedules>(entity);
            dbEntityEntry.State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }
    }
}
