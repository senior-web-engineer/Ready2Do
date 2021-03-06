using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Proxies;
using Web.Utils;

namespace Web.ViewComponents
{
    public class UtenteViewCertificatiViewComponent : ViewComponent
    {
        private readonly UtentiProxy _apiClient;
        ILogger<UtenteViewCertificatiViewComponent> _logger;

        public UtenteViewCertificatiViewComponent(UtentiProxy apiClient,
                                       ILogger<UtenteViewCertificatiViewComponent> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public IViewComponentResult Invoke(IList<CertificatUtenteViewModel> certificati, string userId, int idCliente)
        {
            UtenteViewCertificatiViewModel vm = new UtenteViewCertificatiViewModel()
            {
                Certificati = certificati.OrderBy(c=>c.DataScadenza).ToList(),
                IdCliente = idCliente,
                UserId = userId
            };
            return View("UtenteViewCertificati", vm);
        }
    }
}
