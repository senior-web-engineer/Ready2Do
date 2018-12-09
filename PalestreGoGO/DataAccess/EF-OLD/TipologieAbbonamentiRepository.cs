using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public class TipologieAbbonamentiRepository : ITipologieAbbonamentiRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public TipologieAbbonamentiRepository(PalestreGoGoDbContext context)
        {
            this._context = context;
        }

        public virtual IEnumerable<TipologieAbbonamenti> GetAll(int idTenant)
        {
            return _context.Set<TipologieAbbonamenti>().Where(e => e.IdCliente.Equals(idTenant));
        }

        public virtual int Count(int idTenant)
        {
            return _context.Set<TipologieAbbonamenti>().Count(e => e.IdCliente == idTenant);
        }

        public TipologieAbbonamenti GetSingle(int idTenant, int itemKey)
        {
            return _context.Set<TipologieAbbonamenti>().FirstOrDefault(tl => tl.Id.Equals(itemKey) && tl.IdCliente.Equals(idTenant));
        }

        public void Add(int idTenant, TipologieAbbonamenti entity)
        {
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            EntityEntry dbEntityEntry = _context.Entry<TipologieAbbonamenti>(entity);
            _context.Set<TipologieAbbonamenti>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(int idTenant, TipologieAbbonamenti entity)
        {
            //Attenzione! Non verifichiamo il tenant
            if (entity.IdCliente != idTenant) throw new ArgumentException("idTenant not valid");
            EntityEntry dbEntityEntry = _context.Entry<TipologieAbbonamenti>(entity);
            dbEntityEntry.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(int idTenant, int entityKey)
        {
            var entity = _context.TipologieAbbonamenti.Where(tl => tl.IdCliente.Equals(idTenant) && tl.Id.Equals(entityKey)).Single();
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
