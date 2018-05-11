using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Utils;

namespace Web.Services
{
    public class AccountServices
    {
        //private readonly IClientStore _clientStore;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;

        public AccountServices(
            IOptions<AppConfig> config,
            WebAPIClient apiClient)
        {
            _appConfig = config.Value;
            _apiClient = apiClient;
        }
        public async Task<ClientRegistrationViewModel> BuildRegisterClienteViewModelAsync(string returnUrl)
        {
            var allTipologie = await _apiClient.GetTipologiClientiAsync();
            var vm = new ClientRegistrationViewModel()
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
        public async Task<ClientRegistrationViewModel> BuildRegisterClienteViewModelAsync(ClientRegistrationInputModel model)
        {
            var result = await BuildRegisterClienteViewModelAsync(model.ReturnUrl);
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

        public async Task<bool> CheckEmailAsync(string email)
        {
            return await _apiClient.CheckEmail(email);
        }
    }
}
