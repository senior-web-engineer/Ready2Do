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
    public class UtenteViewAbbonamentiViewComponent : ViewComponent
    {
        private readonly UtentiProxy _apiClient;
        ILogger<UtenteViewAbbonamentiViewComponent> _logger;

        public UtenteViewAbbonamentiViewComponent(UtentiProxy apiClient,
                                       ILogger<UtenteViewAbbonamentiViewComponent> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public IViewComponentResult Invoke(IList<AbbonamentoUtenteViewModel> abbonamenti, string userId, int idCliente)
        {
            UtenteViewAbbonamentiViewModel vm = new UtenteViewAbbonamentiViewModel()
            {
                Abbonamenti = abbonamenti,
                IdCliente = idCliente,
                UserId = userId
            };
            return View("UtenteViewAbbonamenti", vm);
        }
    }
}
