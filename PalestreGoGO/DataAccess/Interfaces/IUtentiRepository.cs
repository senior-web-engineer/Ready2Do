using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PalestreGoGo.DataModel;

namespace PalestreGoGo.DataAccess
{
    public interface IUtentiRepository
    {
        Task<List<ClienteFollowed>> GetGlientiFollowedAsync(Guid userId);
        Task<bool> UserFollowClienteAsync(string userId, int idCliente);
        Task<RichiestaRegistrazione> RichiestaRegistrazioneSalvaAsync(string username, string code, Guid? correlationId);
        Task<RichiestaRegistrazione> CompletaRichiestaRegistrazioneAsync(string username, string code);
    }
}
