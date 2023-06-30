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
using Moq;
using AutoFixture;

namespace Tests_Example
{
	public class PersonsServiceTest
	{
		private readonly IPersonsService _personService;
		private readonly ICountriesService _countriesService;
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly IFixture _fixture; //auto fixture to create dummy objects

		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{
			_fixture = new Fixture();

			var countriesItitalData = new List<Country>();
			var personsInitialData = new List<Person>();

			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
				new DbContextOptionsBuilder<ApplicationDbContext>().Options
			);

			ApplicationDbContext dbContext = dbContextMock.Object;
			dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesItitalData);
			dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

			_countriesService = new CountriesService(dbContext);
			_personService = new PersonsService(dbContext, _countriesService);

			_testOutputHelper = testOutputHelper;
		}

		#region AddPerson
		//Supplying null value as PersonsAddRequest should throw ArgumentNullException
		[Fact]
		public async Task AddPerson_NullPerson()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				//Act
				await _personService.AddPerson(personAddRequest);
			});
		}

		//Supplying null value as PersonName should throw ArgumentException
		[Fact]
		public async Task AddPerson_NullPersonName()
		{
			//Arrange
			PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.PersonName, null as string)
				.Create();

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				//Act
				await _personService.AddPerson(personAddRequest);
			});
		}

		//Supplying proper person details, it should be inserted into the persons list
		//and it should return a PersonRespone object with newly generated ID
		[Fact]
		public async Task AddPerson_ProperPersonDetails()
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
				.With(temp => temp.Email, "someoneElse@example.com")
				.Create();

			//Act
			PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
			List<PersonResponse> persons_list = await _personService.GetAllPersons();

			//Assert
			Assert.True(person_response_from_add.PersonID != Guid.Empty);
			Assert.Contains(person_response_from_add, persons_list);
		}
		#endregion

		#region GetPersonByPersonID
		//Supplying the method with null PersonID should return null
		[Fact]
		public async Task GetPersonByPersonID_NullPersonID()
		{
			//Arrange
			Guid? personID = null;

			//Act
			PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

			//Assert
			Assert.Null(person_response_from_get);
		}

		//Supplying a valid PersonID should return a valid PersonResponse object with person details
		[Fact]
		public async Task GetPersonByPersonID_ValidPersonID()
		{
			//Arrange
			CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
			CountryResponse country_response = await _countriesService.AddCountry(country_request);

			//Act
			PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email@gmail.com").Create();

			PersonResponse person_response_from_add = await _personService.AddPerson(person_request);
			PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

			//Assert
			Assert.Equal(person_response_from_add, person_response_from_get);
		}
		#endregion

		#region GetAllPersons
		//By default it should return an empty list (before adding any person)
		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			//Act
			List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

			//Assert
			Assert.Empty(persons_from_get);
		}

		//When adding persons, it should return the same added persons
		[Fact]
		public async Task GetAllPersons_GetAddedPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
			CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

			CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email1@g.c").Create();

			PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email12@g.c").Create();

			PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email13@g.c").Create();

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request_1, person_request_2, person_request_3
			};

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_get = await _personService.GetAllPersons();

			//print person_response_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_get in person_response_list_from_get)
			{
				_testOutputHelper.WriteLine(person_response_from_get.ToString());
			}

			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				Assert.Contains(person_response_from_add, person_response_list_from_get);
			}
		}
		#endregion

		#region GetFilteredPersons
		//If the search text is empty and search by is "PersonName", all persons should be returned
		[Fact]
		public async Task GetFilteredPersons_EmptySearchText()
		{
			//Arrange
			CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
			CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

			CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email1@g.c").Create();

			PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email12@g.c").Create();

			PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
				.With(temp => temp.Email, "email13@g.c").Create();

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request_1, person_request_2, person_request_3
			};

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

			//print person_response_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse person_response_from_search in person_response_list_from_search)
			{
				_testOutputHelper.WriteLine(person_response_from_search.ToString());
			}

			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				Assert.Contains(person_response_from_add, person_response_list_from_search);
			}
		}

		//Searching a name that exists in the list
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName()
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
				.With(temp => temp.PersonName, "some2name")
				.With(temp => temp.Email, "email13@g.c").Create();

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{ person_request_1, person_request_2, person_request_3 };

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("All persons:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ome");

			//print person_response_list_from_get
			_testOutputHelper.WriteLine("Result:");
			foreach (PersonResponse person_response_from_search in person_response_list_from_search)
			{
				_testOutputHelper.WriteLine(person_response_from_search.ToString());
			}

			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				if (person_response_from_add.PersonName != null && person_response_from_add.PersonName.Contains("me2", StringComparison.OrdinalIgnoreCase))
				{
					Assert.Contains(person_response_from_add, person_response_list_from_search);
				}
			}
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
			for (int i = 0; i < person_response_list_from_add.Count; i++)
			{
				Assert.Equal(person_response_list_from_add[i], person_response_list_from_sort[i]);
			}
		}
		#endregion

		#region UpdatePerson
		//If null value is supplied as PersonUpdateRequest, it should throw ArgumentNullException
		[Fact]
		public async Task UpdatePerson_NullPerson()
		{
			PersonUpdateRequest? personUpdateRequest = null;

			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.UpdatePerson(personUpdateRequest));
		}

		//If invalid PersonID is supplied, ArgumentException should be thrown
		[Fact]
		public async Task UpdatePerson_InvalidPersonID()
		{
			PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

			await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePerson(personUpdateRequest));
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

			await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePerson(personUpdateRequest));
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

			Assert.Equal(person_response_from_get, person_response_from_update);
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

			Assert.True(isDeleted);
		}

		//Supplying Invalid PersonID should return false
		[Fact]
		public async Task DeletePerson_InvalidPersonID()
		{
			bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

			Assert.False(isDeleted);
		}
		#endregion
	}
}
