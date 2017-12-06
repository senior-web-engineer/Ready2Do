using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public class ClientiRepository : IClientiRepository
    {
        private readonly PalestreGoGoDbContext _context;
        private readonly ILogger<ClientiRepository> _logger;

        public ClientiRepository(PalestreGoGoDbContext context, ILogger<ClientiRepository> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<int> AddAsync(Clienti cliente)
        {
            await _context.Clienti.AddAsync(cliente);
            await _context.SaveChangesAsync();
            return cliente.Id;        
        }

        public async Task ConfermaProvisioningAsync(string provisioningToken, Guid userId)
        {
            var user = _context.Set<Clienti>().First(c => c.ProvisioningToken.Equals(provisioningToken, StringComparison.InvariantCulture));
            if (user == null) throw new ArgumentException(nameof(provisioningToken));
            user.DataProvisioning = DateTime.Now;
            user.IdUserOwner = userId;
            await _context.SaveChangesAsync();
        }

        public Task<Clienti> GetAsync(int idCliente)
        {
            var cliente = _context
                            .Clienti
                            .AsNoTracking()
                            .Include(c => c.IdTipologiaNavigation)
                            .Include(c => c.ClientiMetadati)
                            .Where(c => (c.Id.Equals(idCliente)))
                            .Single();

            //Leggiamo anche le immagini
            var immagini = _context.ClientiImmagini
                            .AsNoTracking()
                            .Include(ci => ci.IdTipoImmagineNavigation)
                            .Where(ci => ci.IdCliente.Equals(idCliente) && 
                                        (ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_LOGO) ||
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_SFONDO)))
                            .ToList();
            cliente.ClientiImmagini = immagini;

            return Task.FromResult(cliente);
        }

        public Task UpdateAsync(Clienti cliente)
        {
            throw new NotImplementedException();
        }

        #region Immagini
        public async Task AddImagesAsync(int idCliente, IEnumerable<ClientiImmagini> immagini)
        {
            if (immagini.Any(i => i.IdCliente != idCliente)) throw new ArgumentException("Invalid Tenant");
            _context.ClientiImmagini.AddRange(immagini);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImage(int idCliente, int idImmagine)
        {
            var entity = await _context.ClientiImmagini.Where(ci => ci.IdCliente.Equals(idCliente) && ci.Id.Equals(idImmagine)).FirstOrDefaultAsync();
            if (entity == null) throw new ArgumentException("Invalid Tenant or Id");
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public IEnumerable<ClientiImmagini> GetImages(int idCliente)
        {
            return _context.ClientiImmagini.Where(ci => ci.IdCliente.Equals(idCliente)).AsNoTracking();
        }

        public IEnumerable<ClientiImmagini> GetImages(int idCliente, TipologieImmagini tipo)
        {
            if (tipo == null) throw new ArgumentNullException(nameof(tipo));
            return _context.ClientiImmagini.Where(ci => ci.IdCliente.Equals(idCliente) && ci.IdTipoImmagine.Equals(tipo.Id)).AsNoTracking();
        }
        #endregion

        #region Followers (UtentiClienti)
        public async Task AddUtenteFollowerAsync(int idCliente, Guid idUtente)
        {
            var entity = new ClientiUtenti()
            {
                IdCliente = idCliente,
                IdUtente = idUtente
            };
            await _context.ClientiUtenti.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUtenteFollowerAsync(int idCliente, Guid idUtente)
        {
            var entity = await _context.ClientiUtenti.Where(tl => tl.IdCliente.Equals(idCliente) && tl.IdUtente.Equals(idUtente)).FirstOrDefaultAsync();
            if (entity == null) throw new ArgumentException("Invalid Tenant or Id");
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            await _context.SaveChangesAsync();     
        }

        public  IEnumerable<ClientiUtenti> GetAllFollowers(int idCliente)
        {
            return _context.ClientiUtenti.Where(cu => cu.IdCliente.Equals(idCliente));
        }

        public async Task<ClientiUtenti> GetFollowerAsync(int idCliente, Guid idUtente)
        {
            return await _context.ClientiUtenti.Where(cu => cu.IdCliente.Equals(idCliente) && cu.IdUtente.Equals(idUtente)).SingleOrDefaultAsync();
        }

        #endregion

    }
}
