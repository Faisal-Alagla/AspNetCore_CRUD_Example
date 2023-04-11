using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
	public class PersonsService : IPersonsService
	{
		private readonly List<Person> _persons;
		private readonly ICountriesService _countryService;

		public PersonsService()
		{
			_persons = new List<Person>();
			_countryService = new CountriesService();
		}

		//Helper method to convert a Person into PersonResponse type
		private PersonResponse ConvertPersonToPersonResponse(Person person)
		{
			PersonResponse personResponse = person.ToPersonResponse();
			personResponse.Country = _countryService.GetCountryByCountryID(person.CountryID)?.CountryName;
			return personResponse;
		}

		public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
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
			_persons.Add(person);

			return ConvertPersonToPersonResponse(person);
		}

		public List<PersonResponse> GetAllPersons()
		{
			throw new NotImplementedException();
		}
	}
}
