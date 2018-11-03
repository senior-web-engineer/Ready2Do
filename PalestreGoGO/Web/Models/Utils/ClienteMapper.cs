using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Models.Utils
{
    public static class ClienteMapper
    {
        public static ClienteHomeViewModel MapToHomeViewModel(this ClienteWithImagesViewModel model)
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
                Indrizzo = string.Format("{0}, {1}, {2}", model.Indirizzo.Indirizzo, model.Indirizzo.PostalCode, model.Indirizzo.Citta),
                NumTelefono = model.NumTelefono,
                OrarioApertura = model.OrarioApertura.MapOrarioApertura(),
                RagioneSociale = model.RagioneSociale
            };
            foreach (var img in model.Immagini)
            {
                result.Images.Add(MapImageViewModel(img));
            }
            return result;
        }

        public static ClienteProfileEditViewModel MapToProfileEditVM(this ClienteWithImagesViewModel model)
        {
            if (model == null) { throw new ArgumentNullException(nameof(model)); }
            var result = new ClienteProfileEditViewModel()
            {
                Indirizzo = model.Indirizzo?.Indirizzo,
                CAP = model.Indirizzo?.PostalCode,
                Citta = model.Indirizzo?.Citta,
                Country = model.Indirizzo?.Country,
                Latitudine = model.Indirizzo?.Coordinate?.Latitudine.ToString(CultureInfo.InvariantCulture),
                Longitudine = model.Indirizzo?.Coordinate?.Longitudine.ToString(CultureInfo.InvariantCulture),
                Descrizione = model.Descrizione,
                Email = model.Email,
                EsitoLookup = 0,
                IdCliente = model.IdCliente,
                ImmagineHome = new ImageViewModel()
                {
                    Alt = model.ImmagineHome?.Alt,
                    Caption = model.ImmagineHome?.Nome,
                    Id = model.ImmagineHome?.Id,
                    Ordinamento = (model.ImmagineHome?.Ordinamento) ?? 0,
                    Url = model.ImmagineHome?.Url
                },
                Nome = model.Nome,
                NumTelefono = model.NumTelefono,
                OrarioAperturaVM = model.OrarioApertura.MapOrarioApertura(),
                RagioneSociale = model.RagioneSociale
            };

            return result;
        }

        public static ClienteProfiloViewModel MapToAPIModel(this ClienteProfileEditViewModel model)
        {
            if (model == null) { throw new ArgumentNullException(nameof(model)); }
            var result = new ClienteProfiloViewModel()
            {
                IdCliente = model.IdCliente,
                Nome = model.Nome,
                RagioneSociale = model.RagioneSociale,
                Email = model.Email,    //Email struttura
                NumTelefono = model.NumTelefono,
                Descrizione = model.Descrizione,
                Indirizzo = new IndirizzoViewModel()
                {
                    Citta = model.Citta,
                    Indirizzo = model.Indirizzo,
                    Coordinate = new CoordinateViewModel()
                    {
                        Latitudine = float.Parse(model.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture),
                        Longitudine = float.Parse(model.Longitudine, NumberStyles.Float, CultureInfo.InvariantCulture)
                    }
                },
                ImmagineHome = new ImmagineViewModel()
                {
                    Alt = model.ImmagineHome?.Alt,
                    Nome = model.ImmagineHome?.Caption,
                    Id = model.ImmagineHome?.Id,
                    Ordinamento = (model.ImmagineHome?.Ordinamento) ?? 0,
                    Url = model.ImmagineHome?.Url
                },
                OrarioApertura = model.OrarioAperturaVM.MapToApiModel()
            };

            return result;
        }

        public static PalestreGoGo.WebAPIModel.OrarioAperturaViewModel MapToApiModel(this Models.OrarioAperturaViewModel vm)
        {
            if (vm == null) { return null; }
            return new PalestreGoGo.WebAPIModel.OrarioAperturaViewModel()
            {
                Lunedi = vm?.LunVen?.MapGiornoOrarioApertura(),
                Martedi = vm?.LunVen?.MapGiornoOrarioApertura(),
                Mercoledi = vm?.LunVen?.MapGiornoOrarioApertura(),
                Giovedi = vm?.LunVen?.MapGiornoOrarioApertura(),
                Venerdi = vm?.LunVen?.MapGiornoOrarioApertura(),
                Sabato = vm?.Sabato?.MapGiornoOrarioApertura(),
                Domenica = vm?.Domenica?.MapGiornoOrarioApertura()
            };
        }

        public static OrarioAperturaViewModel MapOrarioApertura(this PalestreGoGo.WebAPIModel.OrarioAperturaViewModel apiModel)
        {
            if (apiModel == null) return null;
            var result = new OrarioAperturaViewModel();
            result.Domenica = apiModel?.Domenica?.MapGiornoOrarioApertura();
            result.LunVen = apiModel?.Lunedi?.MapGiornoOrarioApertura();
            /*
            result.Martedi = apiModel?.Martedi?.MapGiornoOrarioApertura();
            result.Mercoledi = apiModel?.Mercoledi?.MapGiornoOrarioApertura();
            result.Giovedi = apiModel?.Giovedi?.MapGiornoOrarioApertura();
            result.Venerdi = apiModel?.Venerdi?.MapGiornoOrarioApertura();
            */
            result.Sabato = apiModel?.Sabato?.MapGiornoOrarioApertura();
            return result;
        }

        private static GiornoViewModel MapGiornoOrarioApertura(this PalestreGoGo.WebAPIModel.GiornoViewModel apiModel)
        {
            if (apiModel == null) return null;
            var result = new GiornoViewModel()
            {
                TipoOrario =  (TipoOrarioViewModel)apiModel.TipoOrario
            };
            if (apiModel.Mattina != null)
            {
                result.Mattina = new FasciaOrariaViewmodel()
                {
                    Inizio = apiModel.Mattina.Inizio?.ToString(@"hh\:mm"),
                    Fine = apiModel.Mattina.Fine?.ToString(@"hh\:mm")
                };
            }
            if (apiModel.Pomeriggio != null)
            {
                result.Pomeriggio = new FasciaOrariaViewmodel()
                {
                    Inizio = apiModel.Pomeriggio.Inizio?.ToString(@"hh\:mm"),
                    Fine = apiModel.Pomeriggio.Fine?.ToString(@"hh\:mm")
                };
            }
            return result;
        }

        private static PalestreGoGo.WebAPIModel.GiornoViewModel MapGiornoOrarioApertura(this GiornoViewModel webDayModel)
        {
            if (webDayModel == null) return null;
            var result = new PalestreGoGo.WebAPIModel.GiornoViewModel()
            {
                TipoOrario = (PalestreGoGo.WebAPIModel.TipoOrarioViewModel)webDayModel.TipoOrario                
            };
            if (webDayModel.Mattina != null)
            {
                result.Mattina = new PalestreGoGo.WebAPIModel.FasciaOrariaViewmodel();
                if (!string.IsNullOrEmpty(webDayModel.Mattina.Inizio))
                {
                    var parts = webDayModel.Mattina.Inizio.Split(':');
                    result.Mattina.Inizio = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                }
                if (!string.IsNullOrEmpty(webDayModel.Mattina.Fine))
                {
                    var parts = webDayModel.Mattina.Fine.Split(':');
                    result.Mattina.Fine = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                }
            }
            if (webDayModel.Pomeriggio != null)
            {
                result.Pomeriggio = new PalestreGoGo.WebAPIModel.FasciaOrariaViewmodel();
                if (!string.IsNullOrEmpty(webDayModel.Pomeriggio.Inizio))
                {
                    var parts = webDayModel.Pomeriggio.Inizio.Split(':');
                    result.Pomeriggio.Inizio = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                }
                if (!string.IsNullOrEmpty(webDayModel.Pomeriggio.Fine))
                {
                    var parts = webDayModel.Pomeriggio.Fine.Split(':');
                    result.Pomeriggio.Fine = new TimeSpan(int.Parse(parts[0]), int.Parse(parts[1]), 0);
                }
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


        public static ClienteHeaderViewModel MapToClienteHeaderVM(this ClienteWithImagesViewModel model)
        {
            if (model == null) { throw new ArgumentNullException(nameof(model)); }
            var result = new ClienteHeaderViewModel()
            {
                IdCliente = model.IdCliente,
                ImmagineHome = new ImageViewModel()
                {
                    Alt = model.ImmagineHome?.Alt,
                    Caption = model.ImmagineHome?.Nome,
                    Id = model.ImmagineHome?.Id,
                    Ordinamento = (model.ImmagineHome?.Ordinamento) ?? 0,
                    Url = model.ImmagineHome?.Url
                }
            };

            return result;
        }

    }
}
