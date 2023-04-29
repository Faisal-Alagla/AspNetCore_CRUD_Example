using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Person entity
	/// </summary>
	public interface IPersonsService
	{
		/// <summary>
		/// Adds a person object to the list of persons
		/// </summary>
		/// <param name="personAddRequest">Person object to add</param>
		/// <returns>A person object after adding it, along with newly generated PersonID</returns>
		Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

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
		/// Method for sorting the persons list (ASC / DESC) based on the given property
		/// </summary>
		/// <param name="allPersons">The list of persons</param>
		/// <param name="sortBy">Property to sort based on</param>
		/// <param name="sortOrder">ASC or DESC</param>
		/// <returns>Sorted list of persons</returns>
		Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

		/// <summary>
		/// A method to update a peron's details
		/// </summary>
		/// <param name="personUpdateRequest">Person details to update</param>
		/// <returns>A PersonResponse object after updation</returns>
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

		/// <summary>
		/// Method to delete a person from the list
		/// </summary>
		/// <param name="personID">The PersonID of the person to delete</param>
		/// <returns>true if the deletion is successful, otherwise false</returns>
		Task<bool> DeletePerson(Guid? personID);
	}
}
