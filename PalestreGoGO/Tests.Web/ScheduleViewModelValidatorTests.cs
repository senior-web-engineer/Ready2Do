using System;
using Web.Models;
using Web.Models.FluentValidators;
using Xunit;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Tests.Web
{
    public class ScheduleViewModelValidatorTests
    {
        private ScheduleInputViewModelValidator validator;

        private ScheduleEditViewModel GetValidModel()
        {
            return new ScheduleEditViewModel()
            {
                Data = DateTime.Now.AddDays(15).Date,
                DataAperturaIscrizioni = DateTime.Now.AddDays(1).Date,
                OraAperturaIscrizioni = DateTime.Now.AddDays(1).TimeOfDay,
                DataChiusuraIscrizioni = DateTime.Now.AddDays(14).Date,
                OraChiusuraIscrizioni = DateTime.Now.AddDays(14).TimeOfDay,
                TipoSchedule = ready2do.model.common.ScheduleTypeDM.APagamentoRiservatoAbbonati,
                VisibileDalDate = DateTime.Now.Date,
                VisibileDalTime = DateTime.Now.TimeOfDay,
                VisibileFinoAlDate = DateTime.Now.AddDays(20).Date,
                VisibileFinoAlTime = DateTime.Now.AddDays(20).TimeOfDay,
                Title = "Schedule Test",
                OraInizio = DateTime.Now.TimeOfDay,
                IdLocation = 1,
                IdTipoLezione = 2,
                Istruttore = "Isruttore 1",
                Note = "note",
                PostiDisponibili = 10,
                CancellazioneConsentita = true,
                DataCancellazioneMax = DateTime.Now.AddDays(10).Date,
                OraCancellazioneMax = DateTime.Now.AddDays(10).TimeOfDay,
                WaitListDisponibile = true,
            };
        }

        public ScheduleViewModelValidatorTests()
        {
            validator = new ScheduleInputViewModelValidator();
        }

        [Fact]
        public void DataCancellazioneMustBeNullIfNotCancellabile()
        {
            var model = GetValidModel();
            model.CancellazioneConsentita = false;
            model.DataCancellazioneMax = model.DataAperturaIscrizioni.Value.AddDays(-1); //data valida
            validator.ShouldHaveValidationErrorFor(x => x.DataCancellazioneMax, model);
        }

        [Fact]
        public void DatatCancellazioneMustBeLessThanDataInizio()
        {
            var model = GetValidModel();
            model.CancellazioneConsentita = true;
            model.DataCancellazioneMax = model.DataAperturaIscrizioni.Value.AddDays(1); //data non valida
            validator.ShouldHaveValidationErrorFor(x => x.DataCancellazioneMax, model);
        }

        [Fact]
        public void OraCancellazioneSenzaDataNotPossible()
        {
            var model = GetValidModel();
            model.CancellazioneConsentita = true;
            model.OraCancellazioneMax = DateTime.Now.AddMinutes(-30).TimeOfDay;
            model.DataCancellazioneMax = null; //data non valida
            validator.ShouldHaveValidationErrorFor(x => x.DataCancellazioneMax, model);
        }

        [Fact]
        public void OraCancellazioneMustBeLessThanDataOraInizio()
        {
            var model = GetValidModel();
            model.CancellazioneConsentita = true;
            model.DataCancellazioneMax = model.Data;
            model.OraCancellazioneMax = model.OraInizio.Value.Add(new TimeSpan(0, 10, 0));
            validator.ShouldHaveValidationErrorFor(x => x.OraCancellazioneMax, model);
        }

        [Fact]
        public void TitoloTroppoLungo()
        {
            var model = GetValidModel();
            model.Title = new string('T', 101);
            validator.ShouldHaveValidationErrorFor(x => x.Title, model);
        }
    }
}
