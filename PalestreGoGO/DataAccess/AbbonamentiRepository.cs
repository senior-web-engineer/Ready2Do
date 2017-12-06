using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess.Interfaces;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class AbbonamentiRepository: IAbbonamentiRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<AbbonamentiRepository> _logger;

        public AbbonamentiRepository(PalestreGoGoDbContext context, ILogger<AbbonamentiRepository> logger)
        {
            this._logger = logger;
            this._context = context;
        }

        public async Task<int> AddAbbonamentoAsync(int idCliente, AbbonamentiUtenti abbonamento)
        {
            if (abbonamento == null) throw new ArgumentNullException(nameof(abbonamento));
            if (abbonamento.IdCliente != idCliente) throw new ArgumentException("Invalid Tenant");
            _context.AbbonamentiUtenti.Add(abbonamento);
            await _context.SaveChangesAsync();
            return abbonamento.Id.Value;
        }

        public async Task UpdateAbbonamentoAsync(int idCliente, AbbonamentiUtenti abbonamento)
        {
            if (abbonamento.IdCliente != idCliente) throw new ArgumentException("Invalid Tenant");
            _context.AbbonamentiUtenti.Update(abbonamento);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<AbbonamentiUtenti> GetAbbonamentiForUser(int idCliente, Guid userId)
        {
            var result = _context.AbbonamentiUtenti.Where(a => a.IdCliente.Equals(idCliente) && a.UserId.Equals(userId)).AsNoTracking();
            return result;
        }

        public async Task<AbbonamentiUtenti> GetAbbonamentoAsync(int idCliente, int idAbbonamento)
        {
            var result = await _context.AbbonamentiUtenti.SingleOrDefaultAsync(au => au.Id.Equals(idAbbonamento) && au.IdCliente.Equals(idCliente));
            return result;
        }
    }
}
