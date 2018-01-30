using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Utils;

namespace Web.Services
{
    public class AccountServices
    {
        //private readonly IClientStore _clientStore;
        private readonly WebAPIConfig _apiOptions;

        public AccountServices(
            //IClientStore clientStore,
            IOptions<WebAPIConfig> apiOptions)
        {
            //_clientStore = clientStore;
            _apiOptions = apiOptions.Value;

        }
        public async Task<RegistrationViewModel> BuildRegisterViewModelAsync(string returnUrl)
        {
            var allTipologie = await WebAPIClient
                                    .GetTipologiClientiAsync(_apiOptions.BaseAddress);
            var vm = new RegistrationViewModel()
            {
                ReturnUrl = returnUrl,
                TipologieClienti = allTipologie
                                        .Select(i => new SelectListItem()
                                        {
                                            Value = i.Id.ToString(),
                                            Text = i.Nome
                                        })
            };
            return vm;
        }
        public async Task<RegistrationViewModel> BuildRegisterViewModelAsync(RegistrationInputModel model)
        {
            var result = await BuildRegisterViewModelAsync(model.ReturnUrl);
            result.Cognome = model.Cognome;
            result.Email = model.Email;
            result.EmailStruttura = model.EmailStruttura;
            result.IdTipologia = model.IdTipologia;
            result.Indirizzo = model.Indirizzo;
            result.Nome = model.Nome;
            result.NomeStruttura = model.NomeStruttura;
            result.RagioneSociale = model.RagioneSociale;
            result.ReturnUrl = model.ReturnUrl;
            result.Telefono = model.Telefono;
            return result;
        }
    }
}
