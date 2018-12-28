//using Newtonsoft.Json;
//using PalestreGoGo.WebAPIModel;
//using ready2do.model.common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace PalestreGoGo.WebAPI.Model.Mappers
//{
//    public static class ClientiMappers
//    {
//        public static NuovoClienteInputDM ToDM(this NuovoClienteAPIModel nuovoCliente, string userOwnerId)
//        {
//            NuovoClienteInputDM result = new NuovoClienteInputDM()
//            {
//                Citta = nuovoCliente.Citta,
//                Country = nuovoCliente.Country,
//                Descrizione = nuovoCliente.Descrizione,
//                Email = nuovoCliente.Email,
//                IdTipologia = nuovoCliente.IdTipologia,
//                Indirizzo = nuovoCliente.Indirizzo,
//                Latitudine = nuovoCliente.Coordinate.Latitudine,
//                Longitudine = nuovoCliente.Coordinate.Longitudine,
//                NumTelefono = nuovoCliente.NumTelefono,
//                Nome = nuovoCliente.Nome,
//                RagioneSociale = nuovoCliente.RagioneSociale,
//                UrlRoute = nuovoCliente.UrlRoute,
//                ZipOrPostalCode = nuovoCliente.ZipOrPostalCode,
//                IdUserOwner = userOwnerId
//            };
//            return result;
//        }

//        /// <summary>
//        /// Ritorna un ClienteAPIModel a partire da un ClienteDM
//        /// </summary>
//        /// <param name="cliente"></param>
//        /// <param name="immagineHome"></param>
//        /// <returns></returns>
//        public static ClienteAPIModel ToAPI(this ClienteDM cliente, ImmagineClienteDM immagineHome)
//        {
//            ClienteAPIModel result = new ClienteAPIModel();
//            InternalMapClienteBase(result, cliente, immagineHome);
//            return result;
//        }

//        public static ClienteWithImagesAPIModel ToAPI(this ClienteDM cliente, ImmagineClienteDM immagineHome, IEnumerable<ImmagineClienteDM> immagini)
//        {
//            ClienteWithImagesAPIModel result = new ClienteWithImagesAPIModel();
//            InternalMapClienteBase(result, cliente, immagineHome);
//            result.Immagini = immagini;
//            return result;
//        }

//        private static void InternalMapClienteBase(ClienteAPIModel target, ClienteDM source, ImmagineClienteDM immagineHome)
//        {
//            target.Descrizione = source.Descrizione;
//            target.Email = source.Email;
//            target.IdCliente = source.Id.Value;
//            target.Indirizzo = new IndirizzoViewModel()
//            {
//                Citta = source.Citta,
//                Indirizzo = source.Indirizzo,
//                Coordinate = (source.Latitudine.HasValue && source.Longitudine.HasValue) ? new CoordinateAPIModel()
//                {
//                    Latitudine = source.Latitudine.Value,
//                    Longitudine = source.Longitudine.Value

//                } : null,
//                Country = source.Country,
//                PostalCode = source.ZipOrPostalCode
//            };
//            target.ImmagineHome = immagineHome;
//            target.Nome = source.Nome;
//            target.NumTelefono = source.NumTelefono;
//            target.RagioneSociale = source.RagioneSociale,;
//            target.OrarioApertura = DeserializeOrarioApertura(source.OrarioApertura);
//            target.StorageContainer = source.StorageContainer;
//            target.Tipologia = source.TipoCliente;
//            target.UrlRoute = source.UrlRoute;
//        }       
//    }
