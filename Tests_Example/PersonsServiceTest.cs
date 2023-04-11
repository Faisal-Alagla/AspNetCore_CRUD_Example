using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;

namespace Tests_Example
{
	public class PersonsServiceTest
	{
		private readonly IPersonsService _personService;

		public PersonsServiceTest()
		{
			_personService = new PersonsService();
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
			Assert.True(person_response_from_add.PersonID !=  Guid.Empty);
			Assert.Contains(person_response_from_add, persons_list);
		}
		#endregion
	}
}
