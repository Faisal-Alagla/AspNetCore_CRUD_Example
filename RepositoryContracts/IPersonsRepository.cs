using System.Linq.Expressions;
using Entities;

namespace RepositoryContracts
{
	/// <summary>
	/// Represents data access logic for managing the Person entity
	/// </summary>
	public interface IPersonsRepository
	{
		/// <summary>
		/// Adds a new person object to the data store
		/// </summary>
		/// <param name="person">Person object to add</param>
		/// <returns>The person object after adding it</returns>
		Task<Person> AddPerson(Person person);

		/// <summary>
		/// Returns all persons in the data store
		/// </summary>
		/// <returns>A list of persons</returns>
		Task<List<Person>> GetAllPersons();

		/// <summary>
		/// Returns the person object based on the given country id
		/// </summary>
		/// <param name="personId">Person id to search</param>
		/// <returns>The person with the matching id, or null if not found</returns>
		Task<Person?> GetPersonByPersonID(Guid personId);

		/// <summary>
		/// Returns all person objects based on the given expression
		/// </summary>
		/// <param name="predicate">Linq expression to check</param>
		/// <returns>All matching persons based on the given condition</returns>
		Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

		/// <summary>
		/// Deletes a person object with the matching ID
		/// </summary>
		/// <param name="personId">Person ID</param>
		/// <returns>Returns true if deletion is successful, false otherwise</returns>
		Task<bool> DeletePersonByPersonID(Guid personId);

		/// <summary>
		/// Update person object based on the passed person's ID
		/// </summary>
		/// <param name="person">Person object to update</param>
		/// <returns>The updated person object</returns>
		Task<Person> UpdatePerson(Person person);
	}
}