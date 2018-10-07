﻿using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Utils
{
    public static class AbbonamentiMapper
    {
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

        public static AbbonamentoUtenteApiModel MapToAPIModel(this AbbonamentoUtenteViewModel model)
        {
            if (model == null) return null;
            var result = new AbbonamentoUtenteApiModel()
            {
                DataInizioValidita = model.DataInizioValidita,
                Id = model.Id,
                IdCliente = model.IdCliente,
                IdTipoAbbonamento = model.TipoAbbonamento.Id,
                UserId = model.UserId,
                IngressiResidui = model.IngressiResidui,
                Scadenza = model.Scadenza,
                ScadenzaCertificato = model.ScadenzaCertificato,
                StatoPagamento = model.StatoPagamento,
                TipoAbbonamento = null
            };
            return result;
        }

        public static AbbonamentoUtenteApiModel MapToAPIModel(this EditAbbonamentoUtenteInputModel model)
        {
            if (model == null) return null;
            var result = new AbbonamentoUtenteApiModel()
            {
                DataInizioValidita = model.DataInizioValidita,
                Id = model.Id,
                IdCliente = model.IdCliente,
                IdTipoAbbonamento = model.IdTipologiaAbbonamento,
                UserId = model.IdUtente,
                IngressiResidui = model.IngressiResidui,
                Scadenza = model.Scadenza,
                ScadenzaCertificato = model.ScadenzaCertificato,
                StatoPagamento = model.Pagato ? (byte)1 : (byte)0,
                TipoAbbonamento = null
            };
            return result;
        }

        
    }

}