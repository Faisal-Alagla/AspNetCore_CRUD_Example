using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Method for sorting the persons list (ASC / DESC) based on the given property
        /// </summary>
        /// <param name="allPersons">The list of persons</param>
        /// <param name="sortBy">Property to sort based on</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Sorted list of persons</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    }
}
