using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Method to delete a person from the list
        /// </summary>
        /// <param name="personID">The PersonID of the person to delete</param>
        /// <returns>true if the deletion is successful, otherwise false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
