using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PalestreGoGo.DataModel;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class TipologieClientiRepository : ITipologieClientiRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public TipologieClientiRepository(PalestreGoGoDbContext context)
        {
            this._context = context;
        }

        public IEnumerable<TipologiaCliente> GetAll()
        {
            return _context.TipologieClienti.AsNoTracking();
        }


        public async Task<TipologiaCliente> GetOneAsync(int idTipologia)
        {
            return await _context.TipologieClienti.SingleAsync(t => t.Id.Equals(idTipologia));
        }
    }
}
