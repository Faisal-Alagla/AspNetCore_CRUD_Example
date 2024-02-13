using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Person getters
	/// </summary>
	public interface IPersonsGetterService
	{
		/// <summary>
		/// List of persons
		/// </summary>
		/// <returns>A list of PersonResponse objects</returns>
		Task<List<PersonResponse>> GetAllPersons();

		/// <summary>
		/// Returns a PersonResponse object based on the given person id
		/// </summary>
		/// <param name="personID">PersonID (Guid) to search</param>
		/// <returns>Matching perosn as PersonResponse object, returns null if not found</returns>
		Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

		/// <summary>
		/// Method for filtered search based on the chosen property
		/// </summary>
		/// <param name="searchBy">Name of the property for the search</param>
		/// <param name="searchString">Actual string value to search</param>
		/// <returns>Returns all matching persons based on the search by and search string</returns>
		Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// A methood to generate the persons as CSV
        /// </summary>
        /// <returns>The memory stream with CSV data</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// A methood to generate the persons as Excel
        /// </summary>
        /// <returns>The memory stream with Excel data</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
