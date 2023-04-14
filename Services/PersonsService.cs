using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
	public class PersonsService : IPersonsService
	{
		private readonly List<Person> _persons;
		private readonly ICountriesService _countryService;

		public PersonsService(bool initialize = true)
		{
			_persons = new List<Person>();
			_countryService = new CountriesService();

			//dummy data for testing
			if (initialize)
			{
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("0ED9DEE8-4EA4-4AF0-8B2A-94A9C1B1AA79"),
					PersonName = "Maggy",
					Email = "mdooler0@twitpic.com",
					DateOfBirth = DateTime.Parse("5/10/1972"),
					CountryID = Guid.Parse("9FCBFDF3-CDC0-4819-BD69-6F4664866792"),
					Address = "800 Hudson Parkway",
					Gender = "Female",
					ReceiveNewsLetters = true,
				});
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("9ADA7DEE-AB18-4C05-AAA9-9D10A342A940"),
					PersonName = "Hurley",
					Email = "holuby1@rakuten.co.jp",
					DateOfBirth = DateTime.Parse("10/31/1983"),
					CountryID = Guid.Parse("220E4ED1-E199-4A1F-B1C6-AD2A8569BF00"),
					Address = "28657 Clemons Court",
					Gender = "Male",
					ReceiveNewsLetters = true,
				});
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("9AE058BC-810F-4961-8BF9-0E5640151626"),
					PersonName = "Adda",
					Email = "asecret2@yahoo.co.jp",
					DateOfBirth = DateTime.Parse("12/15/1985"),
					CountryID = Guid.Parse("69CC1D32-28CC-42EA-8166-1AEFE5A90244"),
					Address = "7466 Pawling Plaza",
					Gender = "Female",
					ReceiveNewsLetters = true,
				});
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("D2E9DE3A-3105-4D97-80E2-C4EDEA23C837"),
					PersonName = "Meagan",
					Email = "mvanderlinde3@trellian.com",
					DateOfBirth = DateTime.Parse("7/29/1992"),
					CountryID = Guid.Parse("22BB105B-E877-4788-96DA-746E08F8A365"),
					Address = "95631 Pierstorff Point",
					Gender = "Female",
					ReceiveNewsLetters = true,
				});
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("DD9EE5F7-E25C-45EC-AD5D-D458D7E84684"),
					PersonName = "Odell",
					Email = "ostorrock6@t-online.de",
					DateOfBirth = DateTime.Parse("5/20/1986"),
					CountryID = Guid.Parse("8313AD55-0E9E-4ACE-900F-8E8F14338ECF"),
					Address = "5 Onsgard Point",
					Gender = "Male",
					ReceiveNewsLetters = false,
				});
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("D146FD10-4DB8-4C7E-AE1D-A1FC3F86FFD5"),
					PersonName = "Thia",
					Email = "tmocquer7@goo.gl",
					DateOfBirth = DateTime.Parse("7/4/1992"),
					CountryID = Guid.Parse("8313AD55-0E9E-4ACE-900F-8E8F14338ECF"),
					Address = "9508 Little Fleur Park",
					Gender = "Female",
					ReceiveNewsLetters = false,
				});
				_persons.Add(new Person
				{
					PersonID = Guid.Parse("1A7630CB-7A87-447B-A3BC-BF69BC032014"),
					PersonName = "Chelsae",
					Email = "cbrenstuhl8@umich.edu",
					DateOfBirth = DateTime.Parse("6/30/1984"),
					CountryID = Guid.Parse("6D630EF4-73F7-43CA-99A6-E7B3C1E58D2E"),
					Address = "0739 Valley Edge Avenue",
					Gender = "Female",
					ReceiveNewsLetters = true,
				});
			}
		}

		//Helper method to convert a Person into PersonResponse type
		private PersonResponse ToPersonResponse(Person person)
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

			return ToPersonResponse(person);
		}

		public List<PersonResponse> GetAllPersons()
		{
			return _persons.Select(person => ToPersonResponse(person)).ToList();
		}

		public PersonResponse? GetPersonByPersonID(Guid? personID)
		{
			if (personID == null) { return null; }

			Person? person = _persons.FirstOrDefault(person => person.PersonID == personID);

			if (person == null) { return null; }

			return person.ToPersonResponse();
		}

		public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
		{
			List<PersonResponse> allPersons = GetAllPersons();
			List<PersonResponse> matchingPersons = allPersons;

			if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) { return matchingPersons; }

			//Note: can use reflection instead of this
			switch (searchBy)
			{
				case nameof(Person.PersonName):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.PersonName)) ?
					temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(Person.Email):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Email)) ?
					temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(Person.DateOfBirth):
					matchingPersons = allPersons.Where(temp =>
					(temp.DateOfBirth != null) ?
					temp.DateOfBirth.Value.ToString("dd MM yyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(Person.Gender):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Gender)) ?
					temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(Person.CountryID):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Country)) ?
					temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(Person.Address):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Address)) ?
					temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				default: matchingPersons = allPersons; break;
			}

			return matchingPersons;
		}

		public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
		{
			if (string.IsNullOrEmpty(sortBy)) { return allPersons; }

			//Note: can use reflection instead of this
			List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
			{
				(nameof(Person.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(Person.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(Person.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(Person.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(Person.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
				(nameof(Person.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
				(nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
				(nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
				(nameof(Person.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(Person.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
				(nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),
				_ => allPersons,
			};

			return sortedPersons;
		}

		public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			//Validations:
			if (personUpdateRequest == null) { throw new ArgumentNullException(nameof(personUpdateRequest)); }

			ValidationHelper.ModelValidation(personUpdateRequest);

			//get matching person object to update
			Person matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID) ?? throw new ArgumentException("Given PersonID doesn't exist");

			//update
			matchingPerson.PersonName = personUpdateRequest.PersonName;
			matchingPerson.Email = personUpdateRequest.Email;
			matchingPerson.CountryID = personUpdateRequest.CountryID;
			matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
			matchingPerson.Gender = personUpdateRequest.Gender.ToString();
			matchingPerson.Address = personUpdateRequest.Address;
			matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

			return matchingPerson.ToPersonResponse();
		}

		public bool DeletePerson(Guid? personID)
		{
			if (personID == null) { throw new ArgumentNullException(nameof(personID)); }

			Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);

			if (person == null) { return false; }

			_persons.RemoveAll(temp => temp.PersonID == personID);

			return true;
		}
	}
}
