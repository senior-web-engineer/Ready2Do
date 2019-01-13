using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Business
{
    public class UtentiBusiness
    {
        private readonly ILogger<UtentiBusiness> _logger;
        private readonly IClientiRepository _clientiRepo;
        private readonly IClientiUtentiRepository _clientiUtentiRepo;
        private readonly IAppuntamentiRepository _appuntamentiRepo;
        private readonly IAbbonamentiRepository _abbonamentiRepo;

        public UtentiBusiness(ILogger<UtentiBusiness> logger,
                              IClientiRepository clientiRepo,
                              IClientiUtentiRepository clientiUtentiRepo,
                              IAppuntamentiRepository appuntamentiRepo,
                              IAbbonamentiRepository abbonamentiRepo
)
        {
            _logger = logger;
            _clientiRepo = clientiRepo;
            _clientiUtentiRepo = clientiUtentiRepo;
            _appuntamentiRepo = appuntamentiRepo;
            _abbonamentiRepo = abbonamentiRepo;
        }

        public async Task<AssociazioneUtenteClienteDM> GetDettagliAssociazionoConCliente(int idCliente, string userId, int? numAbbonamentiToInclude = 0, DateTime? appuntamentiFrom = null,
                                                                                            DateTime? appuntamentiTo = null, bool? includeAppuntamentiDaConfermare = false, 
                                                                                            bool? includeCertificati = false)
        {
            AssociazioneUtenteClienteDM result = new AssociazioneUtenteClienteDM();
            result.IdCliente = idCliente;
            result.UtenteCliente = await _clientiUtentiRepo.GetUtenteCliente(idCliente, userId, false);
            //Se l'utente non risulta mai associato o con l'associazione eliminata, non ritorniamo dettagli
            if (result.UtenteCliente == null || !result.UtenteCliente.DataCancellazione.HasValue)
            {
                return result;
            }
            if (numAbbonamentiToInclude.HasValue && numAbbonamentiToInclude.Value > 0)
            {
                result.Abbonamenti = await _abbonamentiRepo.GetAbbonamentiForUserAsync(idCliente, userId, true, false, null, numAbbonamentiToInclude.Value);
            }
            if (appuntamentiFrom.HasValue)
            {
                result.Appuntamenti = await _appuntamentiRepo.GetAppuntamentiUtenteAsync(idCliente, userId, pageNumber: 1, pageSize: int.MaxValue, dtInizioSchedule: appuntamentiFrom, dtFineSchedule: appuntamentiTo, sortAscending: false);
                if (includeAppuntamentiDaConfermare.HasValue && includeAppuntamentiDaConfermare.Value)
                {
                    result.AppuntamentiDaConfermare = await _appuntamentiRepo.GetAppuntamentiDaConfermareUtenteAsync(idCliente, userId, pageNumber: 1, pageSize: int.MaxValue, dtInizioSchedule: appuntamentiFrom, dtFineSchedule: appuntamentiTo, sortAscending: false);
                }
            }
            if (includeCertificati.HasValue && includeCertificati.Value)
            {
                result.Certificati = await _clientiUtentiRepo.GetCertificatiUtente(idCliente, userId, true, false);
            }
            return result;
        }
    }
}
