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

        public async Task<IViewComponentResult> InvokeAsync(IList<AbbonamentoUtenteViewModel> abbonamenti)
        {
            return View("UtenteViewAbbonamenti", abbonamenti);
        }
    }
}
