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

        Task ConfermaProvisioningAsync(string provisioningToken, Guid userId);

        Task<Clienti> GetAsync(int idCliente);

        Task UpdateAsync(Clienti cliente);

        #region Immagini
        Task AddImagesAsync(int idCliente, IEnumerable<ClientiImmagini> immagine);
        Task DeleteImage(int idCliente, int idImmagine);
        IEnumerable<ClientiImmagini> GetImages(int idCliente);
        IEnumerable<ClientiImmagini> GetImages(int idCliente, TipologieImmagini tipoImmagine);
        #endregion

        #region Followers
        Task AddUtenteFollowerAsync(int idCliente, Guid idUtente);
        Task RemoveUtenteFollowerAsync(int idCliente, Guid idUtente);
        IEnumerable<ClientiUtenti> GetAllFollowers(int idCliente);
        Task<ClientiUtenti> GetFollowerAsync(int idCliente, Guid idUtente);
        #endregion

        #region Abbonamenti
        #endregion
    }


}
