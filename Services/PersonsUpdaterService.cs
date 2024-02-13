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
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        //private readonly List<Person> _persons;
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnostic)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnostic;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //Validations:
            if (personUpdateRequest == null) { throw new ArgumentNullException(nameof(personUpdateRequest)); }

            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object to update
            Person matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID) ?? throw new InvalidPersonIdException("Given PersonID doesn't exist");

            //update
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            //^^^ state: "Modified" ^^^

            await _personsRepository.UpdatePerson(matchingPerson); //execute UPDATE

            return matchingPerson.ToPersonResponse();
        }
    }
}
