using Entities;

namespace RepositoryContracts
{
	/// <summary>
	/// Represents data access logic for managing the Country entity
	/// </summary>
	public interface ICountriesRepository
	{
		/// <summary>
		/// Adds a new country object to the data store
		/// </summary>
		/// <param name="country">Country object to add</param>
		/// <returns>The country object after adding it</returns>
		Task<Country> AddCountry(Country country);

		/// <summary>
		/// Returns all countries in the data store
		/// </summary>
		/// <returns>A list of countries</returns>
		Task<List<Country>> GetAllCountries();

		/// <summary>
		/// Returns the country object based on the given country id
		/// </summary>
		/// <param name="countryID">Country id to search</param>
		/// <returns>The country with the matching id, or null if not found</returns>
		Task<Country?> GetCountryByCountryID(Guid countryID);

		/// <summary>
		/// Returns the country object based on the given country name
		/// </summary>
		/// <param name="countryName">Country name to search</param>
		/// <returns>The country with the matching name, or null if not found</returns>
		Task<Country?> GetCountryByCountryName(string countryName);
	}
}