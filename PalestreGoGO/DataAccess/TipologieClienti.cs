using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using PalestreGoGo.DataModel;

namespace PalestreGoGo.DataAccess
{
    public class TipologieClientiRepository : ITipologieClientiRepository
    {
        private readonly PalestreGoGoDbContext _context;

        public TipologieClientiRepository(PalestreGoGoDbContext context)
        {
            this._context = context;
        }

        public IEnumerable<TipologieClienti> GetAll()
        {
            return _context.TipologieClienti.AsNoTracking();
        }
    }
}
