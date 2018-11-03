using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Utils
{
    public static class ProfiloClienteMapper
    {
        public static AnagraficaClienteViewModel ToAnagraficaClienteViewModel(this ClienteWithImagesViewModel cliente)
        {
            if(cliente == null) { return null; }
            return new AnagraficaClienteViewModel()
            {
                CAP = cliente.Indirizzo?.PostalCode,
                Indirizzo = cliente.Indirizzo?.Indirizzo,
                Citta = cliente.Indirizzo?.Citta,
                Country = cliente.Indirizzo?.Country,
                Latitudine = cliente.Indirizzo?.Coordinate?.Latitudine.ToString(CultureInfo.InvariantCulture),
                Longitudine = cliente.Indirizzo?.Coordinate?.Longitudine.ToString(CultureInfo.InvariantCulture),
                Descrizione = cliente.Descrizione,
                Email = cliente.Email,
                EsitoLookup = (short)(cliente.Indirizzo != null ? 1 : 0),
                IdCliente = cliente.IdCliente,
                Nome = cliente.Nome,
                NumTelefono = cliente.NumTelefono,
                RagioneSociale = cliente.RagioneSociale,
                UrlRoute = cliente.UrlRoute
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
                    Coordinate = new CoordinateViewModel() {
                        Latitudine = float.Parse(model.Latitudine,CultureInfo.InvariantCulture),
                        Longitudine = float.Parse(model.Longitudine, CultureInfo.InvariantCulture)
                    }
                }
            };
            return result;
        }
    }
}
