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
    public class UtenteViewAppuntamentiViewComponent : ViewComponent
    {
        private readonly UtentiProxy _apiClient;
        ILogger<UtenteViewAppuntamentiViewComponent> _logger;

        public UtenteViewAppuntamentiViewComponent(UtentiProxy apiClient,
                                       ILogger<UtenteViewAppuntamentiViewComponent> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public IViewComponentResult Invoke(IList<AppuntamentoUtenteViewModel> appuntamenti, string userId, int idCliente)
        {
            UtenteViewAppuntamentiViewModel vm = new UtenteViewAppuntamentiViewModel()
            {
                Appuntamenti= appuntamenti,
                IdCliente = idCliente,
                UserId = userId
            };
            return View("UtenteViewAppuntamenti", vm);
        }
    }
}
