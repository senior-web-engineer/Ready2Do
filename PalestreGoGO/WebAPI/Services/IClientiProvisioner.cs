using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public interface IClientiProvisioner
    {
        Task ProvisionClienteAsync(int idCliente, string userId);
    }
}
