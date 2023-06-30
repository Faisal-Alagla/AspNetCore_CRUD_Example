using System;
using System.Collections.Generic;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using EntityFrameworkCoreMock;
using Moq;

namespace CRUD_Tests
{
	public class CountriesServiceTest
	{
		private readonly ICountriesService _countriesService;

		public CountriesServiceTest()
		{
			var countriesItitalData = new List<Country>();
			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
				new DbContextOptionsBuilder<ApplicationDbContext>().Options
			);

			ApplicationDbContext dbContext = dbContextMock.Object;
			dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesItitalData);

			_countriesService = new CountriesService(dbContext);
		}

		#region AddCountry
		//When CountryAddRequest is null, it sould throw ArgumentNullException
		[Fact]
		public async Task AddCountry_NullCountry()
		{
			//Arrange
			CountryAddRequest? request = null;

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				//Act
				await _countriesService.AddCountry(request);
			});
		}

		//When CountryName is null, it should throw ArgumentException
		[Fact]
		public async Task AddCountry_CountryNameNull()
		{
			//Arrange
			CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				//Act
				await _countriesService.AddCountry(request);
			});
		}

		//When CountryName is a duplicate, it should throw ArgumentException[Fact]
		[Fact]
		public async Task AddCountry_CountryNameDuplicate()
		{
			//Arrange
			CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "KSA" };
			CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "KSA" };

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				//Act
				await _countriesService.AddCountry(request1);
				await _countriesService.AddCountry(request2);
			});
		}

		//When proper CountryName is supplied, it should be added to the list of countries
		[Fact]
		public async Task AddCountry_ProperCountryDetails()
		{
			//Arrange
			CountryAddRequest? request = new CountryAddRequest() { CountryName = "KSA" };

			//Act
			CountryResponse response = await _countriesService.AddCountry(request);
			List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

			//Assert
			Assert.True(response.CountryID != Guid.Empty);
			Assert.Contains(response, countries_from_GetAllCountries);
		}
		#endregion

		#region GetAllCountries
		//List of countries should be empty by default
		[Fact]
		public async Task GetAllCountries_EmptyList()
		{
			//Acts
			List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

			//Assert
			Assert.Empty(actual_country_response_list);
		}

		//
		[Fact]
		public async Task GetAllCountries_AddFewCountries()
		{
			//Arrange
			List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
			{
				new CountryAddRequest() { CountryName = "KSA"},
				new CountryAddRequest() { CountryName = "DE"},
			};

			//Act
			List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();
			foreach (CountryAddRequest country_request in country_request_list)
			{
				countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
			}

			List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

			//read each element from countries_list_from_add_country
			foreach (CountryResponse expected_country in countries_list_from_add_country)
			{
				Assert.Contains(expected_country, actualCountryResponseList);
			}
		}
		#endregion

		#region GetCountryByCountryID
		//If we supply null as CountryID, it should return null as CountryResponse
		[Fact]
		public async Task GetCountryByCountryID_NullCountryID()
		{
			//Arrange
			Guid? countryID = null;

			//Act
			CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(countryID);

			//Assert
			Assert.Null(country_response_from_get_method);
		}

		//If we supply a valid CountryID, it should return the matching country details as CountryResponse object
		[Fact]
		public async Task GetCountryByCountryID_ValidCountryID()
		{
			//Arrange
			CountryAddRequest country_add_request = new CountryAddRequest()
			{
				CountryName = "KSA"
			};
			CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

			//Act
			CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

			//Assert
			Assert.Equal(country_response_from_add, country_response_from_get);
		}
		#endregion
	}
}
