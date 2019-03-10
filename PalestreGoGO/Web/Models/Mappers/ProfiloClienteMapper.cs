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
                Longitudine = cliente.Longitudine?.ToString(CultureInfo.InvariantCulture),
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

        public static ClienteAnagraficaDM ToApiModel(this AnagraficaClienteEditViewModel model)
        {
            if(model == null) { return null; }
            ClienteAnagraficaDM result = new ClienteAnagraficaDM()
            {
                Descrizione = model.Descrizione,
                Email = model.Email,
                Id = model.IdCliente,
                Nome = model.Nome,
                RagioneSociale = model.RagioneSociale,
                UrlRoute = model.UrlRoute,
                NumTelefono = model.NumTelefono,
                Citta = model.Citta,
                Country = model.Country,
                ZipOrPostalCode = model.CAP,
                Indirizzo = model.Indirizzo,
                Latitudine = double.Parse(model.Latitudine, CultureInfo.InvariantCulture),
                Longitudine = double.Parse(model.Longitudine, CultureInfo.InvariantCulture)
            };
            return result;
        }
    }
}
