﻿using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesService
	{
		/// <summary>
		/// Adds a country object to the list of countries
		/// </summary>
		/// <param name="countryAddRequest">Country object to add</param>
		/// <returns>A country object after adding it (including newly generated id)</returns>
		CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

		/// <summary>
		/// Returns all countries from the list
		/// </summary>
		/// <returns>All countries</returns>
		List<CountryResponse> GetAllCountries();

		/// <summary>
		/// Returns a CountryResponse object based on the given country id
		/// </summary>
		/// <param name="countryID">CountryID (Guid) to search</param>
		/// <returns>Matching country as CountryResponse object, returns null if not found</returns>
		CountryResponse? GetCountryByCountryID(Guid? countryID);
	}
}