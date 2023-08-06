using System;
using System.Collections.Generic;
using AutoFixture;
using Moq;
using ServiceContracts;
//using Xunit;
using FluentAssertions;
using CRUD_Example.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Tests_Example
{
	public class PersonsControllerTest
	{
		private readonly IPersonsService _personsService;
		private readonly ICountriesService _countriesService;

		private readonly Mock<IPersonsService> _peronsServiceMock;
		private readonly Mock<ICountriesService> _countriesServiceMock;

		private readonly Fixture _fixture;

		public PersonsControllerTest()
		{
			_fixture = new Fixture();

			_peronsServiceMock = new Mock<IPersonsService>();
			_countriesServiceMock = new Mock<ICountriesService>();

			_personsService = _peronsServiceMock.Object;
			_countriesService = _countriesServiceMock.Object;
		}

		#region Index

		[Fact]
		public async Task Index_ShouldReturnIndexViewWithPersonsList()
		{
			//Arrange
			List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

			PersonsController personsController = new PersonsController(_personsService, _countriesService);

			_peronsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(persons_response_list);

			_peronsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
				.ReturnsAsync(persons_response_list);

			//Act
			IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

			//Assert
			ViewResult viewResult = Assert.IsType<ViewResult>(result);
			viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
			viewResult.ViewData.Model.Should().Be(persons_response_list);
		}

		#endregion
	}
}
