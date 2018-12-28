//using PalestreGoGo.DataModel;
//using PalestreGoGo.WebAPIModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PalestreGoGo.WebAPI.ViewModel.Mappers
//{
//    public static class CustomMapper
//    {
//        public static AnagraficaClienteDM /*ToDM*/ (this AnagraficaClienteApiModel model)
//        {
//            if(model == null) { return null; }
//            AnagraficaClienteDM result = new AnagraficaClienteDM()
//            {
//                Citta = model.Indirizzo?.Citta,
//                Country = model.Indirizzo?.Country,
//                Indirizzo = model.Indirizzo?.Indirizzo,
//                Latitudine = model.Indirizzo?.Coordinate?.Latitudine,
//                Longitudine = model.Indirizzo?.Coordinate?.Longitudine,
//                PostalCode = model.Indirizzo?.PostalCode,
//                IdCliente = model.IdCliente,
//                Descrizione = model.Descrizione,
//                Email= model.Email,
//                Nome = model.Nome,
//                NumTelefono = model.NumTelefono,
//                RagioneSociale = model.RagioneSociale,
//                UrlRoute = model.UrlRoute
//            };
//            return result;
//        }
//    }
//}
