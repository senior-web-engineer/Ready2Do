using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class AppuntamentiMapper
    {
        public static AppuntamentoUtenteViewModel MapToViewModel (this AppuntamentoUserApiModel apiModel)
        {
            if (apiModel == null) return null;
            return new AppuntamentoUtenteViewModel()
            {
                Cancellabile = (apiModel.Schedule.CancellabileFinoAl > DateTime.Now),
                DataCancellazione = apiModel.DataCancellazione,
                DataOra = apiModel.Schedule.DataOraInizio,
                DataOraIscrizione = apiModel.DataPrenotazione,
                Id = apiModel.IdAppuntamento,
                IdEvento = apiModel.Schedule.Id.Value,
                Nome = apiModel.Schedule.Title                
            };
        }

        public static IEnumerable<AppuntamentoUtenteViewModel> MapToViewModel(this IEnumerable<AppuntamentoUserApiModel> apiModel)
        {
            foreach(var am in apiModel)
            {
                yield return am.MapToViewModel();
            }
        }

    }
}
