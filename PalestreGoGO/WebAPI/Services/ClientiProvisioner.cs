using PalestreGoGo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public class ClientiProvisioner : IClientiProvisioner
    {
        private readonly IClientiRepository _repositoryClienti;

        public ClientiProvisioner(IClientiRepository repositoryClienti)
        {
            _repositoryClienti = repositoryClienti;
        }

        public async Task ProvisionClienteAsync(string provisioningToken, Guid userId)
        {
            //Che deve fare il provisioning?
            //1. Aggiorniamo lo stato sul DB
            await _repositoryClienti.ConfermaProvisioningAsync(provisioningToken,userId);
            //2. Altro??
        }
    }
}
