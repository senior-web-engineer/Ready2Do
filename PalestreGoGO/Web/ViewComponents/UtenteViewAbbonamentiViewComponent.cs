using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Utils;

namespace Web.ViewComponents
{
    public class UtenteViewAbbonamentiViewComponent : ViewComponent
    {
        WebAPIClient _apiClient;
        ILogger<UtenteViewAbbonamentiViewComponent> _logger;

        public UtenteViewAbbonamentiViewComponent(WebAPIClient apiClient,
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
