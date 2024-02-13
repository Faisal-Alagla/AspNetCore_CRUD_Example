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
    public class PersonsAdderService : IPersonsAdderService
    {
        //private readonly List<Person> _persons;
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnostic)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnostic;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //Validation: personAddRequest parameter can't be null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(personAddRequest);

            //Convert the personAddRequest to Person type
            Person person = personAddRequest.ToPerson();

            //Generate a new PersonID
            person.PersonID = Guid.NewGuid();

            //Add to the list of persons
            await _personsRepository.AddPerson(person);

            return person.ToPersonResponse();
        }
    }
}
