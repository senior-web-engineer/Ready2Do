using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IClientiRepository
    {
        IEnumerable<Clienti> GetAll(int idTenant);
        Clienti GetSingle(int idCliente);
        Task AddAsync(Clienti cliente);
        void Update(Clienti cliente);
        void Delete(int idCliente);
        void Delete(Clienti cliente);

        Task ConfermaProvisioningAsync(string provisioningToken, Guid userId);
        Task CommitAsync();

    }


}
