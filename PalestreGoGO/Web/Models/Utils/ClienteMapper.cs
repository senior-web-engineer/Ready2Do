using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Models.Utils
{
    public static class ClienteMapper
    {
        public static ClienteHomeViewModel MapToHomeViewModel(this ClienteViewModel model)
        {
            if (model == null) { throw new ArgumentNullException(nameof(model)); }
            var result = new ClienteHomeViewModel()
            {
                IdCliente = model.IdCliente,
                Nome = model.Nome,
                Descrizione = model.Descrizione,
                Email = model.Email,
                //Non dovrebbe mai essere NULL
                ImmagineHome = model.ImmagineHome != null ? MapImageViewModel(model.ImmagineHome) : null,
                Indrizzo = string.Format("{0}, {1}, {2}", model.Indrizzo.Indirizzo, model.Indrizzo.PostalCode, model.Indrizzo.Citta),
                NumTelefono = model.NumTelefono,
                OrarioApertura = model.OrarioApertura,
                RagioneSociale = model.RagioneSociale                
            };
            foreach (var img in model.Immagini)
            {
                result.Images.Add(MapImageViewModel(img));
            }
            return result;
        }

        private static ImageViewModel MapImageViewModel(ImmagineViewModel img)
        {
            return new ImageViewModel()
            {
                Alt = img.Alt,
                Caption = img.Nome,
                Id = img.Id,
                Url = img.Url
            };
        }
    }
}
