using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.FluentValidators
{
    public class ScheduleInputViewModelValidator : AbstractValidator<ScheduleInputViewModel>
    {
        public ScheduleInputViewModelValidator()
        {
            DateTime now = DateTime.Now;
            RuleFor(s => s.Title).NotEmpty().MaximumLength(100);
            RuleFor(s => s.IdTipoLezione).NotNull();
            RuleFor(s => s.Data).GreaterThanOrEqualTo(now.Date);
            RuleFor(s => s.OraInizio)
                    .GreaterThanOrEqualTo(now.TimeOfDay)
                        .WithMessage("Non è possibile creare un evento passato.")
                    .When(m=>(m.Data.HasValue && m.Data.Value.Date.Equals(now.Date))); //L'ora conta solo se la data è oggi
            RuleFor(s => s.Istruttore).MaximumLength(150);
            RuleFor(s => s.PostiDisponibili).GreaterThan(0);
            RuleFor(s => s.DataCancellazioneMax)
                //Se Cancellabile, o nno è specificata la DataCancellazioneMax oppure questa non è successiva alla DataOra inizio evento
                .Must((s, dtCanc) => { return s.CancellazioneConsentita ? !dtCanc.HasValue || (dtCanc <= (new DateTime(s.Data.Value.Ticks)).Add(s.OraInizio.Value)) : true; })
                .WithMessage("La Data di Cancellazione non può essere successiva alla Data ed Ora di inizio dell'evento.");
            RuleFor(s => s.DataCancellazioneMax)
                .Null()
                .When(m => !m.CancellazioneConsentita)
                .WithMessage("Non è possibile specificare una data massima di cancellazione se la cancellazione non è consentita");

            RuleFor(s => s.DataCancellazioneMax)
                .Must((s, dtCanc) =>
                {
                    if (!dtCanc.HasValue && s.OraCancellazioneMax.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche una Data.");

            RuleFor(s => s.OraCancellazioneMax)
                .NotNull()
                    .WithMessage("E' necessario specificare anche l'ora")
                .When(m => m.DataCancellazioneMax.HasValue);

            RuleFor(s => s.OraCancellazioneMax)
                .Must((s, oraCanc) =>
                {
                    if (!oraCanc.HasValue || !s.DataCancellazioneMax.HasValue) return true; //Se l'ora non è valorizzata questa regola non si applica
                    if (!s.Data.HasValue || !s.OraInizio.HasValue) return true; //Se non valorizzata la data inizio non si applica la regola
                    if (s.Data.Value.Equals(s.DataCancellazioneMax.Value) && (oraCanc.Value > s.OraInizio.Value)) return false; //La cancellazione max < DataInizio
                    return true;
                })
                .WithMessage("Il termine per la cancellazione non può essere successivo all'inizio dell'evento.");

            RuleFor(s => s.DataAperturaIscrizioni)
                .Must((s, dtApertura) =>
                {
                    if ((!s.Data.HasValue) || (!s.OraInizio.HasValue)) return true; //se non valorizzate la DataOraInizio evento questa proprietà non possiamo validarla
                    if (!dtApertura.HasValue) return true;
                    var dtInizio = (new DateTime(s.Data.Value.Ticks)).Add(s.OraInizio.Value);
                    if (dtApertura.Value >= dtInizio) return false;
                    if (s.DataChiusuraIscrizioni.HasValue)
                        return dtApertura < s.DataChiusuraIscrizioni.Value;
                    return true;
                }).WithMessage("La Data Apertura Iscrizioni non può essere successiva alla Data Chiusura Iscrizioni se specificata o alla Data ed Ora di inizio dell'evento.");
            RuleFor(s => s.DataAperturaIscrizioni)
                .Must((s, dtApertura) =>
                {
                    if (!dtApertura.HasValue && s.OraAperturaIscrizioni.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche la data se specificata l'ora");
            RuleFor(s => s.OraAperturaIscrizioni)
               .Must((s, oraApertura) =>
               {
                   if (s.DataAperturaIscrizioni.HasValue && !oraApertura.HasValue) return false;
                   return true;
               })
               .WithMessage("E' necessario specificare anche l'ora");
            RuleFor(s => s.OraAperturaIscrizioni)
                .Must((s, oraApertura) =>
                {
                    if ((!s.Data.HasValue) || (!s.OraInizio.HasValue)) return true; //se non valorizzate la DataOraInizio evento questa proprietà non possiamo validarla
                    if (!s.DataAperturaIscrizioni.HasValue) return true; //non possiamo validare l'ora senza la data
                    //L'ora ha senso validarla solo a parità di data
                    if (oraApertura.HasValue && s.Data.Equals(s.DataAperturaIscrizioni.Value) && oraApertura.Value > s.OraInizio.Value) return false;
                    return true;
                })
                .WithMessage("Non è possibile specificare un'orario successivo all'inizio dell'evento");

            RuleFor(s => s.DataChiusuraIscrizioni)
            .Must((s, dtChiusura) =>
            {
                if ((!s.Data.HasValue) || (!s.OraInizio.HasValue)) return true; //se non valorizzate la DataOraInizio evento questa proprietà non possiamo validarla
                if (!dtChiusura.HasValue) return true;
                var dtInizio = (new DateTime(s.Data.Value.Ticks)).Add(s.OraInizio.Value);
                //Consideriamo solo la data qui, l'ora la validiamo a parte
                if (dtChiusura.Value >= dtInizio) return false;
                return true;
            }).WithMessage("La Data Chiusura Iscrizioni non può essere successiva alla Data ed Ora di inizio dell'evento.");
            RuleFor(s => s.DataChiusuraIscrizioni)
                .Must((s, dtChiusura) =>
                {
                    if (s.OraChiusuraIscrizioni.HasValue && !dtChiusura.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche la data se specificata l'ora");
            RuleFor(s => s.OraChiusuraIscrizioni)
                .Must((s, oraChiusura) =>
                {
                    if (s.DataChiusuraIscrizioni.HasValue && !oraChiusura.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche l'ora");
            RuleFor(s => s.OraChiusuraIscrizioni)
                .Must((s, oraChiusura) =>
                {
                    if ((!s.Data.HasValue) || (!s.OraInizio.HasValue)) return true; //se non valorizzate la DataOraInizio evento questa proprietà non possiamo validarla
                    if (!s.DataChiusuraIscrizioni.HasValue) return true; //non possiamo validare l'ora senza la data
                    //L'ora ha senso validarla solo a parità di data
                    if (oraChiusura.HasValue && s.Data.Equals(s.DataChiusuraIscrizioni.Value) && oraChiusura.Value > s.OraInizio.Value) return false;
                    return true;
                })
                .WithMessage("Non è possibile specificare un'orario successivo all'inizio dell'evento");

            RuleFor(s => s.WaitListDisponibile)
                .Must((s, wl) =>
                {
                    if (!s.CancellazioneConsentita) return wl == false; //Se non sono previste cancellazioni non è possibile abilitare la Wait List
                    return true;
                });
            /*Date Visibilita
              Regole:
              - DataInizioVisibilita non può essere antecedente DataFineVisibilità
              - DataInizioVisibilita deve essere antecedente DataOraInizio
              - DataFineVisibilita non può essere successiva DataFineVisibilità (può essere antecedente DataOraInizio anche se non ha molto senso)

            */
            RuleFor(s => s.VisibileDalDate)
                 .Must((s, dtVisibDal) =>
                 {
                     if ((!s.Data.HasValue) || (!s.OraInizio.HasValue)) return true; //se non valorizzate la DataOraInizio evento questa proprietà non possiamo validarla
                     if (!dtVisibDal.HasValue) return true;
                     var dtInizio = (new DateTime(s.Data.Value.Ticks)).Add(s.OraInizio.Value);
                     //Consideriamo solo la data qui, l'ora la validiamo a parte
                     if (dtVisibDal.Value >= dtInizio) return false;
                     return true;
                 }).WithMessage("La Data Inizio Visibilità non può essere successiva alla Data ed Ora di inizio dell'evento.");
            RuleFor(s => s.VisibileDalDate)
                 .Must((s, dtVisibDal) =>
                 {
                     if (!dtVisibDal.HasValue) return true;
                     if ((!s.VisibileFinoAlDate.HasValue) || (!s.VisibileFinoAlTime.HasValue)) return true; //se non valorizzate la VisibileFinoAlDate questa validazione non si applica
                     var dtFineVis = (new DateTime(s.VisibileFinoAlDate.Value.Ticks)).Add(s.VisibileFinoAlTime.Value);
                     //Consideriamo solo la data qui, l'ora la validiamo a parte
                     if (dtVisibDal.Value >= dtFineVis) return false;
                     return true;
                 }).WithMessage("La Data Inizio Visibilità non può essere successiva alla Data ed Ora di Fine Visibilità.");
            RuleFor(s => s.VisibileDalDate)
                .Must((s, dtVisibDal) =>
                {
                    if (s.VisibileDalTime.HasValue && !dtVisibDal.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche la data se specificata l'ora");

            RuleFor(s => s.VisibileDalTime)
                .Must((s, oraFineVis) =>
                {
                    if (s.VisibileDalDate.HasValue && !oraFineVis.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche l'ora");
            RuleFor(s => s.VisibileDalTime)
                .Must((s, oraInizioVis) =>
                {
                    if (!oraInizioVis.HasValue) return true;
                    if (!s.VisibileDalDate.HasValue) return true;
                    if ((!s.Data.HasValue) || (!s.OraInizio.HasValue)) return true; //se non valorizzate la DataOraInizio evento questa regola non possiamo validarla
                    if (s.VisibileDalDate.Value.Equals(s.Data.Value) && (oraInizioVis.Value > s.OraInizio.Value)) return false;
                    return true;
                })
                .WithMessage("Non è possibile specificare un'inizio visibilità successiva all'inizio dell'evento");
            RuleFor(s => s.VisibileDalTime)
                .Must((s, oraInizioVis) =>
                {
                    if (!oraInizioVis.HasValue) return true;
                    if (!s.VisibileDalDate.HasValue) return true;
                    if ((!s.VisibileFinoAlDate.HasValue) || (!s.VisibileFinoAlTime.HasValue)) return true; //se non valorizzat VisibileFinoAl questa regola non possiamo validarla
                    if (s.VisibileDalDate.Value.Equals(s.VisibileFinoAlDate.Value) && (oraInizioVis.Value > s.VisibileFinoAlTime.Value)) return false;
                    return true;
                })
                .WithMessage("Non è possibile specificare un'inizio visibilità successiva alla fine visibilità");


            RuleFor(s => s.VisibileFinoAlDate)
                .Must((s, dtFineVis) =>
                {
                    if (dtFineVis.HasValue) return true;
                    if ((!s.VisibileDalDate.HasValue) || (!s.VisibileDalTime.HasValue)) return true; //se non valorizzat VisibileFinoAl questa regola non possiamo validarla
                    if (dtFineVis.Value < s.VisibileDalDate.Value) return false;
                    return true;
                })
                .WithMessage("La data fine visibilità non può essere antecedente quella di inizio visibilità");
            RuleFor(s => s.VisibileFinoAlDate)
                .Must((s, dtFineVis) =>
                {
                    if (!dtFineVis.HasValue && s.VisibileFinoAlTime.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche la data se specificata l'ora");
            RuleFor(s => s.VisibileFinoAlTime)
                .Must((s, oraFineVis) =>
                {
                    if (s.VisibileFinoAlDate.HasValue && !oraFineVis.HasValue) return false;
                    return true;
                })
                .WithMessage("E' necessario specificare anche l'ora");
            RuleFor(s => s.VisibileFinoAlTime)
               .Must((s, oraFineVis) =>
               {
                   if (!oraFineVis.HasValue) return true;
                   if (!s.VisibileFinoAlDate.HasValue) return true;
                   if ((!s.VisibileDalDate.HasValue) || (!s.VisibileDalTime.HasValue)) return true; //se non valorizzat VisibileFinoAl questa regola non possiamo validarla
                   if (s.VisibileFinoAlDate.Value.Equals(s.VisibileDalDate.Value) && (oraFineVis.Value < s.VisibileDalTime.Value)) return false;
                   return true;
               })
               .WithMessage("Non è possibile specificare una fine visibilità antecedente l'inizio della visibilità stessa");
        }
    }
}
