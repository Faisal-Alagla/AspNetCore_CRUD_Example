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

		#region GetFilteredPersons
		//If the search text is empty and search by is "PersonName", all persons should be returned
		[Fact]
		public void GetFilteredPersons_EmptySearchText()
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
			List<PersonResponse> person_response_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "");

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
		public void GetFilteredPersons_SearchByPersonName()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "KSA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "DE" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest()
			{
				PersonName = "somename",
				Email = "email1@g.c",
				Gender = GenderOptions.Female,
				Address = "address",
				CountryID = country_response_1.CountryID,
				ReceiveNewsLetters = true,
			};

			PersonAddRequest person_request_2 = new PersonAddRequest()
			{
				PersonName = "anyname",
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
			_testOutputHelper.WriteLine("All persons:");
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(person_response_from_add.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "ome");

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
		public void GetSortedPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "KSA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "DE" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest()
			{
				PersonName = "anyname",
				Email = "email1@g.c",
				Gender = GenderOptions.Female,
				Address = "address",
				CountryID = country_response_1.CountryID,
				ReceiveNewsLetters = true,
			};

			PersonAddRequest person_request_2 = new PersonAddRequest()
			{
				PersonName = "somename",
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

			List<PersonResponse> allPersons = _personService.GetAllPersons();

			//print list before sorting
			_testOutputHelper.WriteLine("Before Sorting: ");
			foreach (PersonResponse person in allPersons)
			{
				_testOutputHelper.WriteLine(person.ToString());
			}

			//Act
			List<PersonResponse> person_response_list_from_sort = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

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
		public void UpdatePerson_NullPerson()
		{
			PersonUpdateRequest? personUpdateRequest = null;

			Assert.Throws<ArgumentNullException>(() => _personService.UpdatePerson(personUpdateRequest));
		}

		//If invalid PersonID is supplied, ArgumentException should be thrown
		[Fact]
		public void UpdatePerson_InvalidPersonID()
		{
			PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

			Assert.Throws<ArgumentException>(() => _personService.UpdatePerson(personUpdateRequest));
		}

		//If PersonName is null, ArgumentException should be thrown
		[Fact]
		public void UpdatePerson_NullPersonName()
		{
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "KSA" };
			CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest()
			{
				PersonName = "ME :)",
				Email = "me@gmail.com",
				CountryID = country_response_from_add.CountryID,
				Gender = GenderOptions.Male,
			};
			PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

			PersonUpdateRequest personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
			personUpdateRequest.PersonName = null;

			Assert.Throws<ArgumentException>(() => _personService.UpdatePerson(personUpdateRequest));
		}

		//Updating a person's name & email
		[Fact]
		public void UpdatePerson_ValidUpdation()
		{
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "KSA" };
			CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest()
			{
				PersonName = "ME :)",
				Email = "me@gmail.com",
				CountryID = country_response_from_add.CountryID,
				Address = "my address :)",
				Gender = GenderOptions.Male
			};
			PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

			PersonUpdateRequest personUpdateRequest = person_response_from_add.ToPersonUpdateRequest();
			personUpdateRequest.PersonName = "YOU (:";
			personUpdateRequest.Email = "you@gmail.com";

			PersonResponse person_response_from_update = _personService.UpdatePerson(personUpdateRequest);
			PersonResponse person_response_from_get = _personService.GetPersonByPersonID(person_response_from_add.PersonID);

			Assert.Equal(person_response_from_get, person_response_from_update);
		}
		#endregion

		#region DeletePerson
		//Supplying a valid PersonID should return true
		[Fact]
		public void DeletePerson_ValidPersonID()
		{
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "KSA" };
			CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest()
			{
				PersonName = "ME :)",
				Email = "myemail@g.c",
				Address = "my address",
				CountryID = country_response_from_add.CountryID,
				Gender = GenderOptions.Male,
				ReceiveNewsLetters = true,
			};
			PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

			bool isDeleted = _personService.DeletePerson(person_response_from_add.PersonID);

			Assert.True(isDeleted);
		}

		//Supplying Invalid PersonID should return false
		[Fact]
		public void DeletePerson_InvalidPersonID()
		{
			bool isDeleted = _personService.DeletePerson(Guid.NewGuid());

			Assert.False(isDeleted);
		}
		#endregion
	}
}
