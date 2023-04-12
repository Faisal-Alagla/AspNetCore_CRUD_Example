using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;

namespace Tests_Example
{
	public class PersonsServiceTest
	{
		private readonly IPersonsService _personService;
		private readonly ICountriesService _countriesService;
		private readonly ITestOutputHelper _testOutputHelper;

		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{
			_personService = new PersonsService();
			_countriesService = new CountriesService();
			_testOutputHelper = testOutputHelper;
		}

		#region AddPerson
		//Supplying null value as PersonsAddRequest should throw ArgumentNullException
		[Fact]
		public void AddPerson_NullPerson()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act
				_personService.AddPerson(personAddRequest);
			});
		}

		//Supplying null value as PersonName should throw ArgumentException
		[Fact]
		public void AddPerson_NullPersonName()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

			//Assert
			Assert.Throws<ArgumentException>(() =>
			{
				//Act
				_personService.AddPerson(personAddRequest);
			});
		}

		//Supplying proper person details, it should be inserted into the persons list
		//and it should return a PersonRespone object with newly generated ID
		[Fact]
		public void AddPerson_ProperPersonDetails()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new PersonAddRequest()
			{
				PersonName = "name",
				Email = "person@gmail.com",
				Address = "address",
				CountryID = Guid.NewGuid(),
				Gender = GenderOptions.Male,
				DateOfBirth = DateTime.Parse("2000-01-01"),
				ReceiveNewsLetters = true,
			};

			//Act
			PersonResponse person_response_from_add = _personService.AddPerson(personAddRequest);
			List<PersonResponse> persons_list = _personService.GetAllPersons();

			//Assert
			Assert.True(person_response_from_add.PersonID != Guid.Empty);
			Assert.Contains(person_response_from_add, persons_list);
		}
		#endregion

		#region GetPersonByPersonID
		//Supplying the method with null PersonID should return null
		[Fact]
		public void GetPersonByPersonID_NullPersonID()
		{
			//Arrange
			Guid? personID = null;

			//Act
			PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(personID);

			//Assert
			Assert.Null(person_response_from_get);
		}

		//Supplying a valid PersonID should return a valid PersonResponse object with person details
		[Fact]
		public void GetPersonByPersonID_ValidPersonID()
		{
			//Arrange
			CountryAddRequest country_request = new CountryAddRequest() { CountryName = "KSA" };
			CountryResponse country_response = _countriesService.AddCountry(country_request);

			//Act
			PersonAddRequest person_request = new PersonAddRequest()
			{
				PersonName = "name",
				Email = "email@gmail.com",
				Address = "address",
				CountryID = country_response.CountryID,
				DateOfBirth = DateTime.Parse("2000-01-01"),
				Gender = GenderOptions.Male,
				ReceiveNewsLetters = true,
			};
			PersonResponse person_response_from_add = _personService.AddPerson(person_request);
			PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(person_response_from_add.PersonID);

			//Assert
			Assert.Equal(person_response_from_add, person_response_from_get);
		}
		#endregion

		#region GetAllPersons
		//By default it should return an empty list (before adding any person)
		[Fact]
		public void GetAllPersons_EmptyList()
		{
			//Act
			List<PersonResponse> persons_from_get = _personService.GetAllPersons();

			//Assert
			Assert.Empty(persons_from_get);
		}

		//When adding persons, it should return the same added persons
		[Fact]
		public void GetAllPersons_GetAddedPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "KSA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "DE" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest()
			{
				PersonName = "name1",
				Email = "email1@g.c",
				Gender = GenderOptions.Female,
				Address = "address",
				CountryID = country_response_1.CountryID,
				ReceiveNewsLetters = true,
			};
			PersonAddRequest person_request_2 = new PersonAddRequest()
			{
				PersonName = "name2",
				Email = "email2@g.c",
				Gender = GenderOptions.Male,
				Address = "address",
				CountryID = country_response_2.CountryID,
				ReceiveNewsLetters = false,
			};

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
			{
				person_request_1, person_request_2
			};

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();
			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_get = _personService.GetAllPersons();

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
	}
}
