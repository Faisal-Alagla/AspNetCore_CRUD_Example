using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace Tests_Example
{
	public class PersonsServiceTest
	{
		private readonly IPersonsService _personService;
		private readonly ICountriesService _countriesService;

		private readonly Mock<IPersonsRepository> _personsRepositoryMock;
		private readonly IPersonsRepository _personsRepository;

		private readonly ITestOutputHelper _testOutputHelper;
		private readonly IFixture _fixture; //auto fixture to create dummy objects

		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{
			_fixture = new Fixture();
			_personsRepositoryMock = new Mock<IPersonsRepository>();
			_personsRepository = _personsRepositoryMock.Object;

			var countriesItitalData = new List<Country>();
			var personsInitialData = new List<Person>();

			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
				new DbContextOptionsBuilder<ApplicationDbContext>().Options
			);

			ApplicationDbContext dbContext = dbContextMock.Object;
			dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesItitalData);
			dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

			_countriesService = new CountriesService(null);
			_personService = new PersonsService(_personsRepository);

			_testOutputHelper = testOutputHelper;
		}

		/*
		 * ###############
		 Note: You can check Fluent Assertions cheat sheet
		 * ###############
		*/

		#region AddPerson
		//Supplying null value as PersonsAddRequest should throw ArgumentNullException
		[Fact]
		public async Task AddPerson_NullPerson_ToBeArgumentNullException()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;

			//Assert
			Func<Task> action = async () =>
			{
				//Act
				await _personService.AddPerson(personAddRequest);
			};

			await action.Should().ThrowAsync<ArgumentNullException>();
		}

		//Supplying null value as PersonName should throw ArgumentException
		[Fact]
		public async Task AddPerson_NullPersonName_ToBeArgumentException()
		{
			//Arrange
			PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.PersonName, null as string)
				.Create();

			Person person = personAddRequest.ToPerson();

			//When personRepository.AddPerson is called, it has to return the same person object
			_personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
				.ReturnsAsync(person);

			//Assert
			Func<Task> action = async () =>
			{
				//Act
				await _personService.AddPerson(personAddRequest);
			};

			await action.Should().ThrowAsync<ArgumentException>();
		}

		//Supplying proper person details, it should be inserted into the persons list
		//and it should return a PersonRespone object with newly generated ID
		[Fact]
		public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
		{
			//Arrange

			/*
			Create() will just generate random values based on the data types.
			It causes an issue with the email though..
			.With helps with customizing the value of chosen fields.
			*/
			//PersonAddRequest? personAddRequest = _fixture.Create<PersonAddRequest>();
			PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "someone@example.com")
				.Create();

			Person person = personAddRequest.ToPerson();
			PersonResponse person_response_expected = person.ToPersonResponse();

			//If we supply any argument value to the AddPerson method, it should return the same return value
			_personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())) //argument data type
				.ReturnsAsync(person); //return data type

			//Act

			PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
			person_response_expected.PersonID = person_response_from_add.PersonID;

			//Assert

			//Assert.True(person_response_from_add.PersonID != Guid.Empty);
			person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
			person_response_from_add.Should().Be(person_response_expected);
		}
		#endregion

		#region GetPersonByPersonID
		//Supplying the method with null PersonID should return null
		[Fact]
		public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
		{
			//Arrange
			Guid? personID = null;

			//Act
			PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

			//Assert
			person_response_from_get.Should().BeNull();
		}

		//Supplying a valid PersonID should return a valid PersonResponse object with person details
		[Fact]
		public async Task GetPersonByPersonID_ValidPersonID_ToBeSuccessful()
		{
			//Arrange
			Person person = _fixture.Build<Person>()
				.With(temp => temp.Email, "email@gmail.com")
				.With(temp => temp.Country, null as Country)
				.Create();

			PersonResponse person_response_expected = person.ToPersonResponse();

			_personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
				.ReturnsAsync(person);

			//Act
			PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person.PersonID);

			//Assert
			//Assert.Equal(person_response_from_add, person_response_from_get);
			person_response_from_get.Should().Be(person_response_expected);
		}
		#endregion

		#region GetAllPersons
		//By default it should return an empty list (before adding any person)
		[Fact]
		public async Task GetAllPersons_EmptyList_ToBeEmpty()
		{
			//Arrange
			_personsRepositoryMock.Setup(temp => temp.GetAllPersons())
				.ReturnsAsync(new List<Person> { });

			//Act
			List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

			//Assert
			persons_from_get.Should().BeEmpty();
		}

		//When adding persons, it should return the same added persons
		[Fact]
		public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
		{
			//Arrange
			List<Person> persons = new List<Person>()
			{
			_fixture.Build<Person>()
				.With(temp => temp.Email, "email1@g.c")
				.With(temp => temp.Country, null as Country)
				.Create(),

			_fixture.Build<Person>()
				.With(temp => temp.Email, "email12@g.c")
				.With(temp => temp.Country, null as Country)
				.Create(),

			_fixture.Build<Person>()
				.With(temp => temp.Email, "email13@g.c")
				.With(temp => temp.Country, null as Country)
				.Create()
			};

			List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response in person_response_list_expected)
			{
				_testOutputHelper.WriteLine(person_response.ToString());
			}

			_personsRepositoryMock.Setup(temp => temp.GetAllPersons())
				.ReturnsAsync(persons);

			//Act
			List<PersonResponse> person_response_list_from_get = await _personService.GetAllPersons();

			//print person_response_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in person_response_list_from_get)
			{
				_testOutputHelper.WriteLine(person_response_from_get.ToString());
			}

			//Assert
			//foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			//{
			//	//Assert.Contains(person_response_from_add, person_response_list_from_get);
			//}

			person_response_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
		}
		#endregion

		#region GetFilteredPersons
		//If the search text is empty and search by is "PersonName", all persons should be returned
		[Fact]
		public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
		{
			//Arrange
			List<Person> persons = new List<Person>()
			{
			_fixture.Build<Person>()
				.With(temp => temp.Email, "email1@g.c")
				.With(temp => temp.Country, null as Country)
				.Create(),

			_fixture.Build<Person>()
				.With(temp => temp.Email, "email12@g.c")
				.With(temp => temp.Country, null as Country)
				.Create(),

			_fixture.Build<Person>()
				.With(temp => temp.Email, "email13@g.c")
				.With(temp => temp.Country, null as Country)
				.Create()
			};

			List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response in person_response_list_expected)
			{
				_testOutputHelper.WriteLine(person_response.ToString());
			}

			_personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
				.ReturnsAsync(persons);

			//Act
			List<PersonResponse> person_response_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

			//print person_response_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_search in person_response_list_from_search)
			{
				_testOutputHelper.WriteLine(person_response_from_search.ToString());
			}

			//Assert
			//foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			//{
			//	Assert.Contains(person_response_from_add, person_response_list_from_search);
			//}

			person_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
		}

		//Searching a name that exists in the list
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
		{
			//Arrange
			List<Person> persons = new List<Person>()
			{
			_fixture.Build<Person>()
				.With(temp => temp.PersonName, "Faisal")
				.With(temp => temp.Email, "email1@g.c")
				.With(temp => temp.Country, null as Country)
				.Create(),

			_fixture.Build<Person>()
			.With(temp => temp.PersonName, "Ahmad")
				.With(temp => temp.Email, "email12@g.c")
				.With(temp => temp.Country, null as Country)
				.Create(),

			_fixture.Build<Person>()
				.With(temp => temp.PersonName, "Khalid")
				.With(temp => temp.Email, "email13@g.c")
				.With(temp => temp.Country, null as Country)
				.Create()
			};

			List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response in person_response_list_expected)
			{
				_testOutputHelper.WriteLine(person_response.ToString());
			}

			_personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
				.ReturnsAsync(persons);

			//Act
			List<PersonResponse> person_response_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

			//print person_response_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_search in person_response_list_from_search)
			{
				_testOutputHelper.WriteLine(person_response_from_search.ToString());
			}

			//Assert
			//foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			//{
			//	Assert.Contains(person_response_from_add, person_response_list_from_search);
			//}

			person_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
		}
		#endregion

		#region GetSortedPersons
		//Sorting based on PersonName in DESC order, should return persons list with names in DESC order
		[Fact]
		public async Task GetSortedPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
			CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

			CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.CountryID, country_response_1.CountryID)
				.With(temp => temp.PersonName, "somename")
				.With(temp => temp.Email, "email1@g.c").Create();

			PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.CountryID, country_response_2.CountryID)
				.With(temp => temp.PersonName, "name")
				.With(temp => temp.Email, "email12@g.c").Create();

			PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.CountryID, country_response_1.CountryID)
				.With(temp => temp.PersonName, "Aaaaaa")
				.With(temp => temp.Email, "email13@g.c").Create();

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{ person_request_1, person_request_2, person_request_3 };

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			List<PersonResponse> allPersons = await _personService.GetAllPersons();

			//print list before sorting
			_testOutputHelper.WriteLine("Before Sorting: ");
			foreach (PersonResponse person in allPersons)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

			//print result after sorting
			_testOutputHelper.WriteLine("Result:");
			foreach (PersonResponse person_response_from_sort in person_response_list_from_sort)
			{
				_testOutputHelper.WriteLine(person_response_from_sort.ToString());
			}

			person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

			//Assert

			//for (int i = 0; i < person_response_list_from_add.Count; i++)
			//{
			//	Assert.Equal(person_response_list_from_add[i], person_response_list_from_sort[i]);
			//}

			//person_response_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);

			person_response_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
		}
		#endregion

		#region UpdatePerson
		//If null value is supplied as PersonUpdateRequest, it should throw ArgumentNullException
		[Fact]
		public async Task UpdatePerson_NullPerson()
		{
			PersonUpdateRequest? personUpdateRequest = null;

			Func<Task> action = async () => await _personService.UpdatePerson(personUpdateRequest);
			await action.Should().ThrowAsync<ArgumentNullException>();
		}

		//If invalid PersonID is supplied, ArgumentException should be thrown
		[Fact]
		public async Task UpdatePerson_InvalidPersonID()
		{
			PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

			Func<Task> action = async () => await _personService.UpdatePerson(personUpdateRequest);
			await action.Should().ThrowAsync<ArgumentException>();
		}

		//If PersonName is null, ArgumentException should be thrown
		[Fact]
		public async Task UpdatePerson_NullPersonName()
		{
			CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

			CountryResponse country_response = await _countriesService.AddCountry(country_request);

			PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.CountryID, country_response.CountryID)
				.With(temp => temp.Email, "email1@g.c")
				.Create();

			PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

			PersonUpdateRequest personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
			personUpdateRequest.PersonName = null;

			Func<Task> action = async () => await _personService.UpdatePerson(personUpdateRequest);
			await action.Should().ThrowAsync<ArgumentException>();
		}

		//Updating a person's name & email
		[Fact]
		public async Task UpdatePerson_ValidUpdation()
		{
			CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

			CountryResponse country_response = await _countriesService.AddCountry(country_request);

			PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.PersonName, "IHaveAName")
				.With(temp => temp.CountryID, country_response.CountryID)
				.With(temp => temp.Email, "email1@g.c")
				.Create();

			PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

			PersonUpdateRequest personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
			personUpdateRequest.PersonName = "YOU (:";
			personUpdateRequest.Email = "you@gmail.com";

			PersonResponse person_response_from_update = await _personService.UpdatePerson(personUpdateRequest);
			PersonResponse person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

			//Assert.Equal(person_response_from_get, person_response_from_update);
			person_response_from_update.Should().Be(person_response_from_get);
		}
		#endregion

		#region DeletePerson
		//Supplying a valid PersonID should return true
		[Fact]
		public async Task DeletePerson_ValidPersonID()
		{
			CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

			CountryResponse country_response = await _countriesService.AddCountry(country_request);

			PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.CountryID, country_response.CountryID)
				.With(temp => temp.Email, "email1@g.c")
				.Create();

			PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

			bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

			//Assert.True(isDeleted);
			isDeleted.Should().BeTrue();
		}

		//Supplying Invalid PersonID should return false
		[Fact]
		public async Task DeletePerson_InvalidPersonID()
		{
			bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

			//Assert.False(isDeleted);
			isDeleted.Should().BeFalse();
		}
		#endregion
	}
}
