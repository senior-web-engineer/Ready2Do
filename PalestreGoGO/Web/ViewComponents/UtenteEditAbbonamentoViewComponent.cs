using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Proxies;
using Web.Utils;

namespace Web.Views.UtentiCliente
{
    public class UtenteEditAbbonamentoViewComponent: ViewComponent
    {

        private readonly UtentiProxy _apiClient;

        public UtenteEditAbbonamentoViewComponent(UtentiProxy apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync(int idCliente, string userId, int idAbbonamento, string htmlContainerId)
        {
            var vm = await _apiClient.GetAbbonamentoAsync(idCliente, idAbbonamento);
            ViewData["ContainerId"] = htmlContainerId;
            return View("UtenteEditAbbonamento", vm);
        }
    }
}
