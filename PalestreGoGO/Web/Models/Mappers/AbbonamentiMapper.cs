using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class AbbonamentiMapper
    {
        #region Tipologie Abbonamenti
        public static TipologiaAbbonamentoViewModel MapToWebViewModel(this TipologieAbbonamentiViewModel model)
        {
            if (model == null) return null;
            return new TipologiaAbbonamentoViewModel()
            {
                Costo = model.Costo,
                DurataMesi = model.DurataMesi,
                Id = model.Id,
                IdCliente = model.IdCliente,
                MaxLivCorsi = model.MaxLivCorsi,
                Nome = model.Nome,
                NumIngressi = model.NumIngressi
            };
        }

        public static TipologieAbbonamentiViewModel MapToAPIModel(this TipologiaAbbonamentoViewModel model)
        {
            if (model == null) return null;
            return new TipologieAbbonamentiViewModel()
            {
                Costo = model.Costo,
                DurataMesi = model.DurataMesi,
                Id = model.Id,
                IdCliente = model.IdCliente,
                MaxLivCorsi = model.MaxLivCorsi,
                Nome = model.Nome,
                NumIngressi = model.NumIngressi
            };
        }

        public static IEnumerable<TipologiaAbbonamentoViewModel> MapToWebViewModel(this IEnumerable<TipologieAbbonamentiViewModel> model)
        {
            if (model == null) return null;
            List<TipologiaAbbonamentoViewModel> result = new List<TipologiaAbbonamentoViewModel>();
            foreach (var i in model)
            {
                result.Add(i.MapToWebViewModel());
            }
            return result;
        }

        public static IEnumerable<TipologieAbbonamentiViewModel> MapToAPIModel(this IEnumerable<TipologiaAbbonamentoViewModel> model)
        {
            if (model == null) return null;
            List<TipologieAbbonamentiViewModel> result = new List<TipologieAbbonamentiViewModel>();
            foreach (var i in model)
            {
                result.Add(i.MapToAPIModel());
            }
            return result;
        }
        #endregion

        #region ABBONAMENTI UTENTI
        public static AbbonamentoUtenteApiModel MapToAPIModel(this AbbonamentoUtenteViewModel model)
        {
            if (model == null) return null;
            var result = new AbbonamentoUtenteApiModel()
            {
                DataInizioValidita = model.DataInizioValidita.Value,
                Id = model.Id,
                IdCliente = model.IdCliente,
                IdTipoAbbonamento = model.IdTipoAbbonamento,
                UserId = model.UserId,
                IngressiIniziali = model.IngressiIniziali,
                IngressiResidui = model.IngressiResidui,
                Scadenza = model.Scadenza,
                TipoAbbonamento = null
            };
            return result;
        }

        public static AbbonamentoUtenteApiModel MapToAPIModel(this AbbonamentoUtenteInputModel model)
        {
            if (model == null) return null;
            var result = new AbbonamentoUtenteApiModel()
            {
                DataInizioValidita = model.DataInizioValidita,
                Id = model.Id,
                IdCliente = model.IdCliente,
                IdTipoAbbonamento = model.IdTipoAbbonamento,
                UserId = model.UserId,
                IngressiResidui = model.IngressiResidui,
                Scadenza = model.Scadenza,
                Importo = model.Importo,
                ImportoPagato = model.ImportoPagato,
                IngressiIniziali = model.IngressiIniziali,
                TipoAbbonamento = null
            };
            return result;
        }

        public static AbbonamentoUtenteViewModel MapToViewModel(this AbbonamentoUtenteApiModel apiModel)
        {
            if (apiModel == null) return null;
            var result = new AbbonamentoUtenteViewModel()
            {
                DataInizioValidita = apiModel.DataInizioValidita,
                Id = apiModel.Id,
                IdCliente = apiModel.IdCliente,
                IdTipoAbbonamento = apiModel.IdTipoAbbonamento,
                UserId = apiModel.UserId,
                IngressiResidui = apiModel.IngressiResidui,
                Scadenza = apiModel.Scadenza,
                Importo = apiModel.Importo,
                ImportoPagato  = apiModel.ImportoPagato,
                IngressiIniziali = apiModel.IngressiIniziali,
                DataCreazione = apiModel.DataCreazione,
                NomeTipoAbbonamento = apiModel.TipoAbbonamento?.Nome                
            };
            return result;
        }

        public static IEnumerable<AbbonamentoUtenteViewModel> MapToViewModel(this IEnumerable<AbbonamentoUtenteApiModel> apiModels) {
            if (apiModels == null) throw new ArgumentNullException(nameof(apiModels));                
            foreach(var item in apiModels)
            {
                yield return item.MapToViewModel();
            }
        }
        #endregion

    }

}