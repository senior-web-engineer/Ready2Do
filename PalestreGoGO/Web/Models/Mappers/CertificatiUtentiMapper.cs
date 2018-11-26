using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class CertificatiUtentiMapper
    {
        public static CertificatUtenteViewModel MapToViewModel(this ClienteUtenteCertificatoApiModel apiModel)
        {
            if (apiModel == null) return null;
            var result = new CertificatUtenteViewModel()
            {
                Id = apiModel.Id,
                IdCliente = apiModel.IdCliente,
                UserId = apiModel.UserId,
                DataPresentazione = apiModel.DataPresentazione,
                DataScadenza = apiModel.DataScadenza,
                Note = apiModel.Note
            };
            return result;
        }

        public static IEnumerable<CertificatUtenteViewModel> MapToViewModel(this IEnumerable<ClienteUtenteCertificatoApiModel> apiModel)
        {
            if (apiModel == null) { throw new ArgumentNullException(nameof(apiModel)); }
            foreach (var item in apiModel)
            {
                yield return item.MapToViewModel();
            }
        }

        public static ClienteUtenteCertificatoApiModel MapToApiModel (this CertificatUtenteViewModel viewModel)
        {
            if (viewModel == null) return null;
            return new ClienteUtenteCertificatoApiModel()
            {
                DataPresentazione = viewModel.DataPresentazione,
                DataScadenza = viewModel.DataScadenza,
                Id = viewModel.Id,
                UserId = viewModel.UserId,
                IdCliente = viewModel.IdCliente,
                Note = viewModel.Note,
            };
        }

    }
}
