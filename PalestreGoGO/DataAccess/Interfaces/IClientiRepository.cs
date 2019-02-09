using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IClientiRepository
    {
        Task<(int idCliente, Guid correlationId)> CreateClienteAsync(NuovoClienteInputDM cliente);
        Task CompensateCreateClienteAsync(int idCliente, Guid correlationId);
        Task<string> RichiestaRegistrazioneCreaAsync(string userName, Guid? correlationId, DateTime? expiration);
        Task ConfermaProvisioningAsync(int idCliente);
        Task<ClienteDM> GetClienteByIdAsync(int idCliente);
        Task<ClienteDM> GetClienteByUrlRouteAsync(string urlRoute);
        Task AggiornaAnagraficaClienteAsync(int idCliente, ClienteAnagraficaDM anagrafica);

        Task AggiornaOrarioAperturaClienteAsync(int idCliente, OrarioAperturaDM orarioApertura);

        Task<UrlValidationResultDM> CheckUrlRouteValidity(string urlRoute, int? idCliente = null);


        #region Preferenze
        Task<string> GetPreferenzaCliente(int idCliente, string key);
        #endregion

    }


}
