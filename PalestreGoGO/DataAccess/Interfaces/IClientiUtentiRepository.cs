using System.Collections.Generic;
using System.Threading.Tasks;
using PalestreGoGo.DataModel;
using ready2do.model.common;

namespace PalestreGoGo.DataAccess
{
    public interface IClientiUtentiRepository
    {
        Task AssociaUtenteAsync(int idCliente, string userId, string nome, string cognome, string displayName);
        Task DisassociaUtenteFollowerAsync(int idCliente, string userId);
        Task<IEnumerable<UtenteClienteDM>> GetUtentiCliente(int idCliente, bool includeStato, int pageNumber = 1, int pageSize = 25, ClientiUtentiListaSortColumnDM colSort = ClientiUtentiListaSortColumnDM.Nome, SortOrderDM sortOrder = SortOrderDM.Ascending);
        Task<UtenteClienteDM> GetUtenteCliente(int idCliente, string userId, bool includeStato);

        Task DeleteCertificatoUtente(int idCliente, string userId, int idCertificato);
        Task<int> SaveCertificatoUtente(UtenteClienteCertificatoDM certificato);
        Task<IEnumerable<UtenteClienteCertificatoDM>> GetCertificatiUtente(int idCliente, string userId, bool includeExpired = true, bool includeDeleted = false);

    }
}