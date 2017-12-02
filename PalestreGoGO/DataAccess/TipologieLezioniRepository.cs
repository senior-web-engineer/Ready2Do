using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace PalestreGoGo.DataAccess
{
    public class TipologieLezioniRepository : ITipologieLezioniRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public TipologieLezioniRepository(PalestreGoGoDbContext context) 
        {
            this._context = context;
        }

        public virtual IEnumerable<TipologieLezioni> GetAll(int idTenant)
        {
            return _context.Set<TipologieLezioni>().Where(e => e.IdCliente.Equals(idTenant)).AsNoTracking();
        }

        public virtual int Count(int idTenant)
        {
            return _context.Set<TipologieLezioni>().Count(e => e.IdCliente == idTenant);
        }

        public TipologieLezioni GetSingle(int idTenant, int itemKey)
        {
            return _context.Set<TipologieLezioni>().FirstOrDefault(tl => tl.Id.Equals(itemKey) && tl.IdCliente.Equals(idTenant));
        }

        public void Add(int idTenant, TipologieLezioni entity)
        {
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            _context.Set<TipologieLezioni>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(int idTenant, TipologieLezioni entity)
        {
            //Attenzione! Non verifichiamo il tenant
            if(entity.IdCliente != idTenant) throw new ArgumentException("idTenant not valid"); 
            EntityEntry dbEntityEntry = _context.Entry<TipologieLezioni>(entity);
            dbEntityEntry.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(int idTenant, int entityKey)
        {
            var entity = _context.TipologieLezioni.Where(tl => tl.IdCliente.Equals(idTenant) && tl.Id.Equals(entityKey)).Single();
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            _context.SaveChanges();
        }        
    }
}
