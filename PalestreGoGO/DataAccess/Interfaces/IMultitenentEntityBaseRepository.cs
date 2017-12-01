using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public interface IMultitenantEntityRepository<TenantKey, T, TKey> where T: BaseMultitenantEntity
    {
        IEnumerable<T> GetAll(TenantKey idTenant);
        int Count(TenantKey idTenant);
        T GetSingle(TenantKey idTenant, TKey itemKey);
        IEnumerable<T> FindBy(TenantKey idTenant, Expression<Func<T, bool>> predicate);
        void Add(TenantKey idTenant, T entity);
        void Update(TenantKey idTenant, T entity);
        void Delete(TenantKey idTenant, TKey itemKey);
        void Delete(TenantKey idTenant, T item);
        void Commit();

        //IEnumerable<T> AllIncluding(int idTenant, params Expression<Func<T, object>>[] includeProperties);
        //T GetSingle(TenKey idTenant, Expression<Func<T, bool>> predicate);
        //T GetSingle(TenKey idTenant, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        //void DeleteWhere(int idTenant, Expression<Func<T, bool>> predicate);
    }
}
