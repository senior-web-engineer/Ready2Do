using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Utils
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NoPastDateValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        private string GetErrorMessage()
        {
            return $"Non è possibile specificare una data passata.";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            if (date < DateTime.Now) { return new ValidationResult(GetErrorMessage()); }
            return ValidationResult.Success;
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

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-nopastdate", this.GetErrorMessage());
        }
    }
}
