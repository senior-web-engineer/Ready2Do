using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Utils
{
    public class GoogleAdrressValidationAttribute: ValidationAttribute, IClientModelValidator
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
             ClientRegistrationInputModel client = (ClientRegistrationInputModel)validationContext.ObjectInstance;

            if (string.IsNullOrWhiteSpace(client.Latitudine) || string.IsNullOrWhiteSpace(client.Longitudine))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }


        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-googleaddress", GetErrorMessage());

        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }

        private string GetErrorMessage()
        {
            return $"E' necessario seleziona un indirizzo tra quelli proposti.";
        }
    }
}
