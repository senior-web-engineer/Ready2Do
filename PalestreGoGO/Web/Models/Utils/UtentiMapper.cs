using PalestreGoGo.WebAPIModel;
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
                    Nominativo = u.Nominativo,
                    DisplayName = u.DisplayName,
                    IdCliente = u.IdCliente,
                    IdUtente = u.IdUtente
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

            foreach (var item in cfEnum)
            {
                yield return item.MapToClienteFollowed();
            }
        }


        private static string MapToIcon(this TipologiaNotificaApiModel tipo)
        {
            if ((tipo == null) || (tipo.Code == null)) return null;
            switch (tipo.Code.Trim().ToUpper())
            {
                case "ACCOUNT_TO_CONFIRM":
                    return "warning";
                default:
                    return null;
            }
        }

        public static List<NotificaViewModel> MapToViewModel(this IEnumerable<NotificaConTipoApiModel> apiModel)
        {
            if (apiModel == null) { return null; }
            List<NotificaViewModel> result = new List<NotificaViewModel>();
            foreach (var item in apiModel.OrderBy(i => i.Tipo.Priority))
            {
                result.Add(new NotificaViewModel()
                {
                    Id = item.Id.Value,
                    DataCreazione = item.DataCreazione,
                    IsNew = !item.DataPrimaVisualizzazione.HasValue,
                    Testo = item.Testo,
                    Titolo = item.Titolo,
                    IconName = item.Tipo.MapToIcon()
                });
            }
            return result;
        }
    }
}
