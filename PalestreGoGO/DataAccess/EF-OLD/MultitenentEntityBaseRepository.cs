using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public class MultitenantEntityBaseRepository<T> : IMultitenantEntityRepository<int, T, int>
        where T : BaseMultitenantEntity
    {
        protected PalestreGoGoDbContext _context;

        public MultitenantEntityBaseRepository(PalestreGoGoDbContext context)
        {
            _context = context;
        }


        public virtual IEnumerable<T> GetAll(int idTenant)
        {
            return _context.Set<T>().Where(e => e.IdCliente.Equals(idTenant)).AsEnumerable();
        }

        public virtual int Count(int idTenant)
        {
            return _context.Set<T>().Count(e => e.IdCliente == idTenant);
        }
        //public virtual IEnumerable<T> AllIncluding(int idCliente, params Expression<Func<T, object>>[] includeProperties)
        //{
        //    IQueryable<T> query = _context.Set<T>().Where(c => c.IdCliente == idCliente);
        //    foreach (var includeProperty in includeProperties)
        //    {
        //        query = query.Include(includeProperty);
        //    }
        //    return query.AsEnumerable();
        //}

        public T GetSingle(int idTenant, int itemKey)
        {
            return _context.Set<T>().FirstOrDefault(x => (x.IdCliente == idTenant) && (x.Id == itemKey));
        }

        //public T GetSingle(Expression<Func<T, bool>> predicate)
        //{
        //    return _context.Set<T>().FirstOrDefault(predicate);
        //}

        //public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        //{
        //    IQueryable<T> query = _context.Set<T>();
        //    foreach (var includeProperty in includeProperties)
        //    {
        //        query = query.Include(includeProperty);
        //    }

        //    return query.Where(predicate).FirstOrDefault();
        //}

        public virtual IEnumerable<T> FindBy(int idTenant, Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(e=>e.IdCliente == idTenant).Where(predicate);
        }

        public virtual void Add(int idTenant, T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public virtual void Update(int idTenant, T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(int idTenant, int entityKey)
        {
            var entity =_context.Set<T>().Single(e=>e.IdCliente.Equals(idTenant) && e.Id.Equals(entityKey));
            this.Delete(idTenant, entity);
        }

        public virtual void Delete(int idTenant, T entity)
        {
            if (entity.IdCliente != idTenant) throw new ArgumentException("Invalid Tenant.");
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public virtual void Commit()
        {
            _context.SaveChanges();
        }
    }
}
