﻿using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Utils
{
    public static class UtentiMapper
    {
        public static List<ClienteUtenteViewModel> MapToClienteUtenteViewModel(this IEnumerable<ClienteUtenteWithAbbonamentoApiModel> utenti)
        {
            if (utenti == null) return null;
            var result = new List<ClienteUtenteViewModel>();
            ClienteUtenteViewModel item;
            foreach (var u in utenti)
            {
                item = new ClienteUtenteViewModel()
                {
                    Cognome = u.Cognome,
                    Email = u.Email,
                    IdCliente = u.IdCliente,
                    IdUtente = u.IdUtente,
                    Nome = u.Nome
                };
                result.Add(item);
                if (u.Abbonamento == null) { continue; }

                item.Abbonamento = new AbbonamentoUtenteViewModel()
                {
                    DataInizioValidita = u.Abbonamento.DataInizioValidita,
                    Id = u.Abbonamento.Id,
                    IngressiResidui = u.Abbonamento.IngressiResidui,
                    Scadenza = u.Abbonamento.Scadenza,
                    ScadenzaCertificato = u.Abbonamento.ScadenzaCertificato,
                    StatoPagamento = u.Abbonamento.StatoPagamento,
                    TipoAbbonamento = u.Abbonamento.TipoAbbonamento.MapToWebViewModel()
                };

            }
            return result;
        }

        public static ClienteFollowed MapToClienteFollowed(this ClienteFollowedApiModel cf)
        {
            if (cf == null) return null;
            return new ClienteFollowed()
            {
                IdCliente = cf.IdCliente,
                NomeCliente = cf.Nome,
                DataFollowing = cf.DataFollowing,
                RagioneSociale = cf.RagioneSociale,
                HasAbbonamentoValido = cf.AbbonamentoValido
            };
        }
        public static IEnumerable<ClienteFollowed> MapToEnumerableClienteFollowed(this IEnumerable<ClienteFollowedApiModel> cfEnum)
        {
            if (cfEnum == null) throw new ArgumentNullException(nameof(cfEnum));

            foreach(var item in cfEnum) {
                yield return item.MapToClienteFollowed(); 
            }
        }


    }
}