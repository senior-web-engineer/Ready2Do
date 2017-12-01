using System;
using System.Collections.Generic;
using System.Text;
using PalestreGoGo.DataModel;

namespace PalestreGoGo.DataAccess
{

    public interface IBaseRepository<T, TKey> where T:class, new()
    {
        IEnumerable<T> GetAll(int idTenant);
        int Count(int idTenant);
        T GetSingle(int idTenant, TKey itemKey);
        void Add(int idTenant, T entity);
        void Update(int idTenant, T entity);
        void Delete(int idTenant, TKey itemKey);
    }
}
