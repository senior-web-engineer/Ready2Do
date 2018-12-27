using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IUtentiRepository
    {
        Task<List<ClienteFollowedDM>> GetGlientiFollowedAsync(Guid userId);
        Task<bool> UserFollowClienteAsync(string userId, int idCliente);
        Task<RichiestaRegistrazioneDM> RichiestaRegistrazioneSalvaAsync(string username, string code, Guid? correlationId);
        Task<RichiestaRegistrazioneDM> CompletaRichiestaRegistrazioneAsync(string username, string code);
    }
}
