using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Utils;

namespace Web.Views.UtentiCliente
{
    public class UtenteAddAbbonamentoViewComponent : ViewComponent
    {

        private readonly WebAPIClient _apiClient;

        public UtenteAddAbbonamentoViewComponent(WebAPIClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync(int idCliente, string userId, int idTipoAbbonamento, string htmlContainerId)
        {
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var tipoAbbonanemto = await _apiClient.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento, accessToken);
            var vm = new AbbonamentoUtenteViewModel()
            {
                DataInizioValidita = DateTime.Now,
                IdCliente = idCliente,
                UserId = userId,
                Importo = tipoAbbonanemto.Costo ?? 0,
                IngressiIniziali = tipoAbbonanemto.NumIngressi,
                NomeTipoAbbonamento = tipoAbbonanemto.Nome,
                Scadenza = DateTime.Now.AddMonths(tipoAbbonanemto.DurataMesi ?? 0)
            };
            ViewData["ContainerId"] = htmlContainerId;
            return View("UtenteEditAbbonamento", vm);
        }
    }
}-
