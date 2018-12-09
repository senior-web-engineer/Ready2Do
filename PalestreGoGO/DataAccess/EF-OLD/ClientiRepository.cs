using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data;

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
            /*20180519#La route URL la facciamo inserire direttamente senza andarcela a generare */
            //cliente.UrlRoute = await this.internalCreateUrlRoute(cliente.Nome);
            await _context.Clienti.AddAsync(cliente);
            await _context.SaveChangesAsync();
            return cliente.Id;
        }

        private async Task<string> internalCreateUrlRoute(string nomeCliente)
        {
            var encodedNomeCliente = Uri.EscapeDataString(nomeCliente);
            string result = null;
            using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "CreateUrlRoute";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@pNomeClienteUrlEncoded", encodedNomeCliente));
                await _context.Database.OpenConnectionAsync();
                result = (string)await cmd.ExecuteScalarAsync();
            }
            return result;
        }

        public async Task ConfermaProvisioningAsync(string provisioningToken, string userId)
        {
            var user = _context.Set<Clienti>().First(c => c.SecurityToken.Equals(provisioningToken, StringComparison.InvariantCulture));
            if (user == null) throw new ArgumentException(nameof(provisioningToken));
            user.DataProvisioning = DateTime.Now;
            user.IdUserOwner = userId;
            await _context.SaveChangesAsync();
        }

        public async Task<Clienti> GetByUrlAsync(string urlRoute)
        {
            var cliente = await _context
                            .Clienti
                            .AsNoTracking()
                            .Include(c => c.IdTipologiaNavigation)
                            .Include(c => c.ClientiMetadati)
                            .Where(c => (c.UrlRoute.Equals(urlRoute)))
                            .SingleOrDefaultAsync();

            if (cliente == null) return null;
            //Leggiamo anche le immagini
            var immagini = _context.ClientiImmagini
                            .AsNoTracking()
                            .Include(ci => ci.IdTipoImmagineNavigation)
                            .Where(ci => ci.IdCliente.Equals(cliente.Id) &&
                                        (ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_LOGO) ||
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_SFONDO) ||
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_GALLERY)
                                         ))
                            .ToList();
            cliente.ClientiImmagini = immagini;

            return cliente;
        }

        public Task<Clienti> GetByTokenAsync(string securityToken)
        {
            var cliente = _context
                            .Clienti
                            .AsNoTracking()
                            .Include(c => c.IdTipologiaNavigation)
                            .Include(c => c.ClientiMetadati)
                            .Where(c => (c.SecurityToken.Equals(securityToken)))
                            .Single();

            //Leggiamo anche le immagini
            var immagini = _context.ClientiImmagini
                            .AsNoTracking()
                            .Include(ci => ci.IdTipoImmagineNavigation)
                            .Where(ci => ci.IdCliente.Equals(cliente.Id) &&
                                        (ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_LOGO) ||
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_SFONDO)) ||
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_GALLERY))
                            .ToList();
            cliente.ClientiImmagini = immagini;

            return Task.FromResult(cliente);
        }

        public async Task<Clienti> GetByIdUserOwnerAsync(string idOwner, bool includeImages = false)
        {
            var cliente = await _context
                                .Clienti
                                .AsNoTracking()
                                .Include(c => c.IdTipologiaNavigation)
                                .Include(c => c.ClientiMetadati)
                                .Where(c => (c.IdUserOwner.Equals(idOwner)))
                                .SingleOrDefaultAsync();
            if ((cliente != null) && includeImages)
            {
                //Leggiamo anche le immagini
                var immagini = _context.ClientiImmagini
                                .AsNoTracking()
                                .Include(ci => ci.IdTipoImmagineNavigation)
                                .Where(ci => ci.IdCliente.Equals(cliente.Id) &&
                                            (ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_LOGO) ||
                                             ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_SFONDO)) ||
                                             ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_GALLERY))
                                .ToList();
                cliente.ClientiImmagini = immagini;
            }
            return cliente;
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
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_SFONDO)) ||
                                         ci.IdTipoImmagineNavigation.Codice.Equals(Constants.TIPO_IMMAGINE_GALLERY))
                            .ToList();
            cliente.ClientiImmagini = immagini;

            return Task.FromResult(cliente);
        }

        public async Task UpdateAsync(Clienti cliente)
        {
            if (cliente == null) { throw new ArgumentNullException(nameof(cliente)); }
            EntityEntry dbEntityEntry = _context.Entry<Clienti>(cliente);
            dbEntityEntry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnagraficaAsync(AnagraficaClienteDM anagrafica)
        {
            using (var cn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_AnagraficaSave]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = anagrafica.IdCliente;
                cmd.Parameters.Add("@pNome", SqlDbType.NVarChar, 100).Value = anagrafica.Nome;
                cmd.Parameters.Add("@pRagioneSociale", SqlDbType.NVarChar, 100).Value = anagrafica.RagioneSociale;
                cmd.Parameters.Add("@pEmail", SqlDbType.NVarChar, 100).Value = anagrafica.Email;
                cmd.Parameters.Add("@pNumTelefono", SqlDbType.NVarChar, 50).Value = anagrafica.NumTelefono;
                cmd.Parameters.Add("@pDescrizione", SqlDbType.NVarChar, 1000).Value = anagrafica.Descrizione;
                cmd.Parameters.Add("@pIndirizzo", SqlDbType.NVarChar, 250).Value = anagrafica.Indirizzo;
                cmd.Parameters.Add("@pCitta", SqlDbType.NVarChar, 100).Value = anagrafica.Citta;
                cmd.Parameters.Add("@pPostalCode", SqlDbType.NVarChar, 10).Value = anagrafica.PostalCode;
                cmd.Parameters.Add("@pCountry", SqlDbType.NVarChar, 100).Value = anagrafica.Country;
                cmd.Parameters.Add("@pLatitudine", SqlDbType.Float).Value = anagrafica.Latitudine;
                cmd.Parameters.Add("@pLongitudine", SqlDbType.Float).Value = anagrafica.Longitudine;
                cmd.Parameters.Add("@pUrlRoute", SqlDbType.VarChar, 205).Value = anagrafica.UrlRoute;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateOrarioAperturaAsync(int idCliente, string orarioApertura)
        {
            using (var cn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_OrarioSave]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = idCliente;
                cmd.Parameters.Add("@pOrarioApertura", SqlDbType.VarChar, -1).Value = orarioApertura;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<UrlValidationResultDM> CheckUrlRouteValidity(string urlRoute, int? idCliente = null)
        {
            using (var cn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                SqlParameter paramOut = new SqlParameter("@pResult", SqlDbType.Int);
                paramOut.Direction = ParameterDirection.Output;
                var cmd = cn.CreateCommand();
                cmd.CommandText = "[dbo].[Clienti_RouteValidate]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@pUrlRoute", SqlDbType.VarChar, 200).Value = urlRoute;
                cmd.Parameters.Add("@pIdCliente", SqlDbType.Int).Value = (object)idCliente ?? DBNull.Value;
                cmd.Parameters.Add(paramOut);
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return (UrlValidationResultDM)paramOut.Value;
            }
        }

        #region Immagini
        public async Task AddImagesAsync(int idCliente, IEnumerable<ClientiImmagini> immagini)
        {
            if (immagini.Any(i => i.IdCliente != idCliente)) throw new ArgumentException("Invalid Tenant");
            _context.ClientiImmagini.AddRange(immagini);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int idCliente, int idImmagine)
        {
            var entity = await _context.ClientiImmagini.Where(ci => ci.IdCliente.Equals(idCliente) && ci.Id.Equals(idImmagine)).FirstOrDefaultAsync();
            if (entity == null) throw new ArgumentException("Invalid Tenant or Id");
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(int idCliente, ClientiImmagini immagine)
        {
            if (immagine == null) { throw new ArgumentNullException(nameof(immagine)); }
            if (immagine.IdCliente != idCliente) throw new ArgumentException("Invalid Tenant");
            EntityEntry dbEntityEntry = _context.Entry<ClientiImmagini>(immagine);
            dbEntityEntry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public ClientiImmagini GetImage(int idCliente, int idImmagine)
        {
            return _context.ClientiImmagini.Where(ci => ci.IdCliente.Equals(idCliente) && ci.Id.Equals(idImmagine)).AsNoTracking().Single();
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

  
    }
}
