using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Views.UtentiCliente
{
    public class UtenteEditAbbonamentoViewComponent: ViewComponent
    {

        private readonly WebAPIClient _apiClient;

        public UtenteEditAbbonamentoViewComponent(WebAPIClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync(int idCliente, string userId, int idAbbonamento, string htmlContainerId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var vm = await _apiClient.GetAbbonamentoAsync(idCliente, idAbbonamento, accessToken);
            ViewData["ContainerId"] = htmlContainerId;
            return View("UtenteEditAbbonamento", vm);
        }
    }
}
