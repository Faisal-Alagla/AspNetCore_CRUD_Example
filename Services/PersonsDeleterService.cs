using CsvHelper;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;
using Exceptions;

namespace Services
{
    public class PersonsDeleterService : IPersonsDeleterService
    {
        //private readonly List<Person> _persons;
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnostic)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnostic;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null) { throw new ArgumentNullException(nameof(personID)); }

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);

            if (person == null) { return false; }

            //_persons.RemoveAll(temp => temp.PersonID == personID);
            return await _personsRepository.DeletePersonByPersonID(personID.Value);
        }
    }
}
