using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Model.Mappers
{
    public static class ClientiMappers
    {
        public static NuovoClienteInputDM ToDM(this NuovoClienteAPIModel nuovoCliente, string userOwnerId)
        {
            NuovoClienteInputDM result = new NuovoClienteInputDM()
            {
                Citta = nuovoCliente.Citta,
                Country = nuovoCliente.Country,
                Descrizione = nuovoCliente.Descrizione,
                Email = nuovoCliente.Email,
                IdTipologia = nuovoCliente.IdTipologia,
                Indirizzo = nuovoCliente.Indirizzo,
                Latitudine = nuovoCliente.Coordinate.Latitudine,
                Longitudine = nuovoCliente.Coordinate.Longitudine,
                NumTelefono = nuovoCliente.NumTelefono,
                Nome = nuovoCliente.Nome,
                RagioneSociale = nuovoCliente.RagioneSociale,
                UrlRoute = nuovoCliente.UrlRoute,
                ZipOrPostalCode = nuovoCliente.ZipOrPostalCode,
                IdUserOwner = userOwnerId
            };
            return result;
        }
    }
}
