using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
	/// <summary>
	/// Represents business logic for manipulating Person entity
	/// </summary>
	public interface IPersonsUpdaterService
	{
		/// <summary>
		/// A method to update a peron's details
		/// </summary>
		/// <param name="personUpdateRequest">Person details to update</param>
		/// <returns>A PersonResponse object after updation</returns>
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
	}
}
