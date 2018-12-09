using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class TipologicheMapper
    {
        public static TipologieLezioniViewModel ToVM(this TipologiaLezioneDM apiModel)
        {
            if (apiModel == null) return null;
            return new TipologieLezioniViewModel()
            {
                DataCancellazione = apiModel.DataCancellazione,
                DataCreazione = apiModel.DataCreazione,
                Descrizione = apiModel.Descrizione,
                Durata = apiModel.Durata,
                Id = apiModel.Id,
                IdCliente = apiModel.IdCliente,
                LimiteCancellazioneMinuti = apiModel.LimiteCancellazioneMinuti,
                Livello = apiModel.Livello,
                MaxPartecipanti = apiModel.MaxPartecipanti,
                Nome = apiModel.Nome,
                Prezzo = apiModel.Prezzo
            };
        }
    }
}
