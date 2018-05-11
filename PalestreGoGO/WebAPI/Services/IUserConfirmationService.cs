using PalestreGoGo.WebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Services
{
    public interface IUserConfirmationService
    {
        Task EnqueueConfirmationMailRequestAsync(ConfirmationMailMessage mailReq);

        Task SendConfirmationMailRequestAsync(ConfirmationMailMessage mailReq);
    }
}
