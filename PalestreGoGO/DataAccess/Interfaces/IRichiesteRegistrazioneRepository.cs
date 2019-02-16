using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IRichiesteRegistrazioneRepository
    {
        Task<string> NuovaRichiestaRegistrazioneAsync(string username, DateTime? expiration, Guid? correlationId, int? idRefereer);

        Task<EsitoConfermaRegistrazioneDM> CompletaRichiestaRegistrazioneAsync(string username, string code);

        Task AnnullaRichiestaRegistrazioneAsync(string username);

        Task<RichiestaRegistrazioneDM> GetUltimaRichiestaRegistrazioneAsync(string userName);
    }
}
