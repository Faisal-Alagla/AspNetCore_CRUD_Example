using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ServiceContracts.DTO;

namespace Services.Helpers
{
	public class ValidationHelper
	{
		/// <summary>
		/// Helper method for model validations
		/// </summary>
		/// <param name="obj">The object to validatate</param>
		/// <exception cref="ArgumentException"></exception>
		internal static void ModelValidation(object obj)
		{
			ValidationContext validationContext = new ValidationContext(obj);
			List<ValidationResult> validationResults = new List<ValidationResult>();

			bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
			if (!isValid)
			{
				throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
			}
		}
	}
}
