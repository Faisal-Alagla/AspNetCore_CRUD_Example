using System;
using System.Collections.Generic;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class for adding a new Country
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }

		/// <summary>
		/// Converts the current object of CountryAddRequest into a new Country object
		/// </summary>
		/// <returns>A Country object that has the same name as the current CountryAddRequest object</returns>
		public Country ToCountry()
        {
            return new Country() { CountryName = CountryName };
        }
    }
}
