using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Web.Models;
using Web.Models.FluentValidators;

namespace Web.Tests
{
    [TestClass]
    public class ClientiControllerTests
    {
        //CheckUrl

        public ClientiControllerTests()
        {

        }

        [TestMethod]
        public void DataCancellazioneMustBeNullIfNotCancellabile()
        {
            //var model = GetValidModel();
            //model.CancellazioneConsentita = false;
            //model.DataCancellazioneMax = model.DataAperturaIscrizioni.Value.AddDays(-1); //data valida
            //validator.ShouldHaveValidationErrorFor(x => x.DataCancellazioneMax, model);
        }
    }
}
