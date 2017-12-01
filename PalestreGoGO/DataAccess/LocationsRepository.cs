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
    public class LocationsRepository : ILocationsRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public LocationsRepository(PalestreGoGoDbContext context) 
        {
            this._context = context;
        }

        public virtual IEnumerable<Locations> GetAll(int idTenant)
        {
            return _context.Set<Locations>().Where(e => e.IdCliente.Equals(idTenant));
        }

        public virtual int Count(int idTenant)
        {
            return _context.Set<Locations>().Count(e => e.IdCliente == idTenant);
        }

        public Locations GetSingle(int idTenant, int itemKey)
        {
            return _context.Set<Locations>().FirstOrDefault(tl => tl.Id.Equals(itemKey) && tl.IdCliente.Equals(idTenant));
        }

        public void Add(int idTenant, Locations entity)
        {
            if (!entity.IdCliente.Equals(idTenant)) throw new ArgumentException("idTenant not valid");
            _context.Set<Locations>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(int idTenant, Locations entity)
        {
            //Attenzione! Non verifichiamo il tenant
            if(entity.IdCliente != idTenant) throw new ArgumentException("idTenant not valid"); 
            EntityEntry dbEntityEntry = _context.Entry<Locations>(entity);
            dbEntityEntry.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public virtual void Delete(int idTenant, int entityKey)
        {
            var entity = _context.Locations.Single(tl => tl.IdCliente.Equals(idTenant) && tl.Id.Equals(entityKey));
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            _context.SaveChanges();
        }        
    }
}
