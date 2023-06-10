using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

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
		Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

		/// <summary>
		/// Returns all countries from the list
		/// </summary>
		/// <returns>All countries</returns>
		Task<List<CountryResponse>> GetAllCountries();

		/// <summary>
		/// Returns a CountryResponse object based on the given country id
		/// </summary>
		/// <param name="countryID">CountryID (Guid) to search</param>
		/// <returns>Matching country as CountryResponse object, returns null if not found</returns>
		Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);

		/// <summary>
		/// Upload countries from Excel file
		/// </summary>
		/// <param name="formFile">Excel file with list of countries</param>
		/// <returns>Number of countries added</returns>
		Task<int> UploadCountriesFromExcelFile(IFormFile formFile); //IEnumerable<IFromFile> if we wanna receive multiple files
	}
}