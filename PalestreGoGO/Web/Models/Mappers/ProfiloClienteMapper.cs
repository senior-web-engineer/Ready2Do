using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class ProfiloClienteMapper
    {
        public static AnagraficaClienteViewModel ToAnagraficaClienteViewModel(this ClienteDM cliente)
        {
            if (cliente == null) { return null; }
            return new AnagraficaClienteViewModel()
            {
                CAP = cliente.ZipOrPostalCode,
                Indirizzo = cliente.Indirizzo,
                Citta = cliente.Citta,
                Country = cliente.Country,
                Latitudine = cliente.Latitudine?.ToString(CultureInfo.InvariantCulture),
                Longitudine = cliente.Indirizzo?.ToString(CultureInfo.InvariantCulture),
                Descrizione = cliente.Descrizione,
                Email = cliente.Email,
                EsitoLookup = (short)(cliente.Indirizzo != null ? 1 : 0),
                IdCliente = cliente.Id.Value,
                Nome = cliente.Nome,
                NumTelefono = cliente.NumTelefono,
                RagioneSociale = cliente.RagioneSociale,
                UrlRoute = cliente.UrlRoute,                
            };
        }

        public static AnagraficaClienteApiModel ToApiModel(this AnagraficaClienteEditViewModel model)
        {
            if(model == null) { return null; }
            AnagraficaClienteApiModel result = new AnagraficaClienteApiModel()
            {
                Descrizione = model.Descrizione,
                Email = model.Email,
                IdCliente = model.IdCliente,
                Nome = model.Nome,
                RagioneSociale = model.RagioneSociale,
                UrlRoute = model.UrlRoute,
                NumTelefono = model.NumTelefono,
                Indirizzo = new IndirizzoViewModel()
                {
                    Citta = model.Citta,
                    Country = model.Country,
                    PostalCode = model.CAP,
                    Indirizzo = model.Indirizzo,
                    Coordinate = new CoordinateAPIModel() {
                        Latitudine = float.Parse(model.Latitudine,CultureInfo.InvariantCulture),
                        Longitudine = float.Parse(model.Longitudine, CultureInfo.InvariantCulture)
                    }
                }
            };
            return result;
        }
    }
}
