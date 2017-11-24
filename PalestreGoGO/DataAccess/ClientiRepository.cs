using System;
using System.Collections.Generic;
using System.Text;
using PalestreGoGo.DataModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;


namespace PalestreGoGo.DataAccess
{
    public class ClientiRepository : IClientiRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public ClientiRepository(PalestreGoGoDbContext context)
        {
            this._context = context;
        }

        public async Task AddAsync(Clienti cliente)
        {
            EntityEntry dbEntityEntry = _context.Entry<Clienti>(cliente);
            await _context.Set<Clienti>().AddAsync(cliente);
        }

        public async Task ConfermaProvisioningAsync(string provisioningToken, Guid userId)
        {
            var user = _context.Set<Clienti>().First(c => c.ProvisioningToken.Equals(provisioningToken, StringComparison.InvariantCulture));
            if (user == null) throw new ArgumentException(nameof(provisioningToken));
            user.DataProvisioning = DateTime.Now;
            user.IdUserOwner = userId;
            await _context.SaveChangesAsync();
        }

        public Task CommitAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Delete(int idCliente)
        {
            throw new NotImplementedException();
        }

        public void Delete(Clienti cliente)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Clienti> GetAll(int idTenant)
        {
            throw new NotImplementedException();
        }

        public Clienti GetSingle(int idCliente)
        {
            throw new NotImplementedException();
        }

        public void Update(Clienti cliente)
        {
            throw new NotImplementedException();
        }
    }
}
