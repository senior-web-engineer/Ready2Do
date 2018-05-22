using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Utils
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GoogleAdrressValidationAttribute: ValidationAttribute, IClientModelValidator
    {

        private string _esitoLookupAddressPropName;

        public GoogleAdrressValidationAttribute(string esitLookupPropertyName, string errorMessage) : base(errorMessage)
        {
            this._esitoLookupAddressPropName = esitLookupPropertyName;
        }

        //Validazione lato server
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {
                // Using reflection we can get a reference to the other date property, in this example the project start date
                var otherPropertyInfo = validationContext.ObjectType.GetProperty(this._esitoLookupAddressPropName);
                // Let's check that otherProperty is of type DateTime as we expect it to be
                if (otherPropertyInfo.PropertyType.Equals(new bool().GetType()))
                {
                    bool esitoLookup = (bool)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                    if (!esitoLookup)
                    {
                        validationResult = new ValidationResult(ErrorMessageString);
                    }
                }
                else
                {
                    validationResult = new ValidationResult("An error occurred while validating the property. OtherProperty is not of type DateTime");
                }
            }
            catch (Exception ex)
            {
                // Do stuff, i.e. log the exception
                // Let it go through the upper levels, something bad happened
                throw ex;
            }
            return validationResult;
        }

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //     ClientRegistrationInputModel client = (ClientRegistrationInputModel)validationContext.ObjectInstance;

        //    if (string.IsNullOrWhiteSpace(client.Latitudine) || string.IsNullOrWhiteSpace(client.Longitudine))
        //    {
        //        return new ValidationResult(GetErrorMessage());
        //    }

        //    return ValidationResult.Success;
        //}


        //Validazione client
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-googleaddress", this.GetErrorMessage());
            MergeAttribute(context.Attributes, "data-val-googleaddress-esitolookupField", this._esitoLookupAddressPropName);
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
