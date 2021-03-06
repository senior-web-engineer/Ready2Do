using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class UtentiMapper
    {
        public static List<ClienteUtenteViewModel> MapToClienteUtenteViewModel(this IEnumerable<UtenteClienteDM> utenti)
        {
            if (utenti == null) return null;
            var result = new List<ClienteUtenteViewModel>();
            foreach (var u in utenti)
            {
                result.Add(u.MapToClienteUtenteViewModel());
            }
            return result;
        }

        public static ClienteUtenteViewModel MapToClienteUtenteViewModel(this UtenteClienteDM utente)
        {
            if (utente == null) return null;
            return new ClienteUtenteViewModel()
            {
                UserInfo = new UserHeaderViewModel()
                {
                    Nome = utente.Nome,
                    Cognome = utente.Cognome,
                    Stato = (ClienteUtenteStatoViewModel)(utente is UtenteClienteDetailsDM ? (int)(((UtenteClienteDetailsDM)utente).Stato) : 0),
                    DisplayName = utente.DisplayName,
                    IdCliente = utente.IdCliente,
                    IdUtente = utente.UserId,
                    DataAssociazione = utente.DataAssociazione.Value
                },
                Abbonamenti = null,
                Certificati = null,
                Appuntamenti = null
            };
        }

        public static ClienteUtenteViewModel MapToClienteUtenteViewModel(this ClienteUtenteDetailsApiModel utente)
        {
            if (utente == null) return null;
            return new ClienteUtenteViewModel()
            {
                UserInfo = new UserHeaderViewModel()
                {
                    Nome = utente.Nome,
                    Cognome = utente.Cognome,
                    Stato = (ClienteUtenteStatoViewModel)((int)utente.Stato),
                    DisplayName = utente.DisplayName,
                    IdCliente = utente.IdCliente,
                    DataAssociazione = utente.DataAssociazione,
                    Email = utente.Email,
                    IdUtente = utente.IdUtente,
                    NumTelefono = utente.TelephoneNumber
                },
                Abbonamenti = null,
                Certificati = null,
                Appuntamenti = null
            };
        }
        
        public static UserHeaderViewModel MapToUserHeaderViewModel(this ClienteUtenteDetailsApiModel utente)
        {
            if (utente == null) return null;
            return new UserHeaderViewModel()
            {
                Nome = utente.Nome,
                Cognome = utente.Cognome,
                Email = utente.Email,
                NumTelefono = utente.TelephoneNumber,
                Stato = (ClienteUtenteStatoViewModel)((int)utente.Stato),
                DisplayName = utente.DisplayName,
                IdCliente = utente.IdCliente,
                IdUtente = utente.IdUtente,
                DataAssociazione = utente.DataAssociazione,
            };
        }

        public static UserHeaderViewModel MapToUserHeaderViewModel(this AssociazioneUtenteClienteDM utente)
        {
            if (utente == null) return null;
            return new UserHeaderViewModel();

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
