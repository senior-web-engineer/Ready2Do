using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IClientiRepository
    {
        Task<int> AddAsync(Clienti cliente);

        Task ConfermaProvisioningAsync(string provisioningToken, string userId);

        Task<Clienti> GetAsync(int idCliente);
        Task<Clienti> GetByUrlAsync(string urlRoute);
        Task<Clienti> GetByTokenAsync(string urlRoute);
        Task<Clienti> GetByIdUserOwnerAsync(string idOwner, bool includeImages = false);

        Task UpdateAsync(Clienti cliente);

        Task<UrlValidationResultDM> CheckUrlRouteValidity(string urlRoute, int? idCliente = null);
        Task UpdateAnagraficaAsync(AnagraficaClienteDM anagrafica);
        Task UpdateOrarioAperturaAsync(int idCliente, string orarioApertura);

        #region Immagini
        Task AddImagesAsync(int idCliente, IEnumerable<ClientiImmagini> immagine);
        Task DeleteImageAsync(int idCliente, int idImmagine);
        Task UpdateImageAsync(int idCliente, ClientiImmagini immagine);
        ClientiImmagini GetImage(int idCliente, int idImmagine);
        IEnumerable<ClientiImmagini> GetImages(int idCliente);
        IEnumerable<ClientiImmagini> GetImages(int idCliente, TipologieImmagini tipoImmagine);
        #endregion

        #region Preferenze
        Task<string> GetPreferenzaCliente(int idCliente, string key);
        #endregion

    }


}
