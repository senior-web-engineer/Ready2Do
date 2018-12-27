using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class ClientiProvisioner : IClientiProvisioner
    {
        private readonly IClientiRepository _repositoryClienti;
        private readonly ILogger<ClientiProvisioner> _logger;
        private readonly IConfiguration _config;

        /// <summary>
        /// Costruttore di default con DI
        /// </summary>
        /// <param name="config"></param>
        /// <param name="repositoryClienti"></param>
        /// <param name="logger"></param>
        public ClientiProvisioner(IConfiguration config,
                                  IClientiRepository repositoryClienti, 
                                  ILogger<ClientiProvisioner> logger)
        {
            _config = config;
            _repositoryClienti = repositoryClienti;
            _logger = logger;
        }

        /// <summary>
        /// Effettua il provisioning di un nuovo Cliente.
        /// Al momento l'unica operaziona svolta in fase di provisioning è l'associazione della Hero Image di default
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task ProvisionClienteAsync(int idCliente, string userId)
        {
            //Associamo l'immagine di default
            var defaultHeroImage = new ImmagineClienteDM()
            {
                IdTipoImmagine = (int)TipoImmagineDM.Sfondo,
                Url = _config.GetValue<string>("Provisioning:DefaultHeroImageUrl"),
                Nome = "Default Hero Image",
                IdCliente = idCliente
            };
            await _repositoryClienti.AddImagesAsync(idCliente, new ImmagineClienteDM[] { defaultHeroImage });
            //TODO: Aggiungere eventuali steps per il provisioning

            await _repositoryClienti.ConfermaProvisioningAsync(idCliente);
        }
    }
}
