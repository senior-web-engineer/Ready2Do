using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Proxies;
using Web.Utils;

namespace Web.Views.UtentiCliente
{
    public class UtenteAddAbbonamentoViewComponent : ViewComponent
    {

        private readonly TipologicheProxy _tipologicheProxy;

        public UtenteAddAbbonamentoViewComponent(TipologicheProxy tipologicheProxy)
        {
            _tipologicheProxy = tipologicheProxy;
        }

        public async Task<IViewComponentResult> InvokeAsync(int idCliente, string userId, int idTipoAbbonamento, string htmlContainerId)
        {
            
            var tipoAbbonanemto = await _tipologicheProxy.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento);
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
}
