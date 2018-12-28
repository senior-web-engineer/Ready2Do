using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IImmaginiClientiRepository
    {
        Task AddImagesAsync(int idCliente, IEnumerable<ImmagineClienteInputDM> immagini);
        Task<int> AddImageAsync(int idCliente, ImmagineClienteInputDM immagine);
        Task<ImmagineClienteDM> DeleteImageAsync(int idCliente, int idImmagine);
        Task UpdateImageAsync(int idCliente, ImmagineClienteInputDM immagine);
        Task<ImmagineClienteDM> GetImage(int idCliente, int idImmagine, bool includeDeleted = false);
        Task<IEnumerable<ImmagineClienteDM>> GetImages(int idCliente, TipoImmagineDM? tipo = null, bool includeDeleted = false);
    }
}
