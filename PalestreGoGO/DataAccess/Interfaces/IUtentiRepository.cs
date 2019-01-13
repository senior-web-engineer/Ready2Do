using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IUtentiRepository
    {
        Task<List<ClienteFollowedDM>> GetGlientiFollowedAsync(string userId);
        Task<bool> UserFollowClienteAsync(string userId, int idCliente);
        Task<string> RichiestaRegistrazioneSalvaAsync(string username, DateTime expiration, Guid? correlationId, int? idRefereer);
        Task<EsitoConfermaRegistrazioneDM> CompletaRichiestaRegistrazioneAsync(string username, string code);
    }
}
