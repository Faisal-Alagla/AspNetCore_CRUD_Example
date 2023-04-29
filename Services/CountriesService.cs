using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesService : ICountriesService
	{
		//private readonly PersonsDbContext _countries;
		private readonly PersonsDbContext _db;

		//constructor
		public CountriesService(PersonsDbContext personsDbContext)
		{
			_db = personsDbContext;

			//dummy data for testing
			//if (initialize)
			//{
			//	_countries.AddRange(new List<Country>
			//	{
			//	new Country { CountryID = Guid.Parse("9FCBFDF3-CDC0-4819-BD69-6F4664866792"), CountryName = "KSA" },
			//	new Country { CountryID = Guid.Parse("8313AD55-0E9E-4ACE-900F-8E8F14338ECF"), CountryName = "DE" },
			//	new Country { CountryID = Guid.Parse("6D630EF4-73F7-43CA-99A6-E7B3C1E58D2E"), CountryName = "UK" },
			//	new Country { CountryID = Guid.Parse("220E4ED1-E199-4A1F-B1C6-AD2A8569BF00"), CountryName = "USA" },
			//	new Country { CountryID = Guid.Parse("69CC1D32-28CC-42EA-8166-1AEFE5A90244"), CountryName = "JP" },
			//	});
			//}
		}

		public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
		{
			//Validation: countryAddRequest parameter can't be null
			if (countryAddRequest == null)
			{
				throw new ArgumentNullException(nameof(countryAddRequest));
			}

			//Validation: CountryName can't be null
			if (countryAddRequest.CountryName == null)
			{
				throw new ArgumentException(nameof(countryAddRequest.CountryName));
			}

			//Validation: CountryName can't be duplicate
			//if (_db.Countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
			if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
			{
				throw new ArgumentException("Given country name already exists");
			}

			//Convert object from CountryAddRequest to Country type
			Country country = countryAddRequest.ToCountry();

			//generate CountryID
			country.CountryID = Guid.NewGuid();

			//Add country object to _countries list
			_db.Countries.Add(country); // newly added entity objects state "Added" in DbSet
			await _db.SaveChangesAsync(); // to execute the INSERT query on the Added objects against the database

			return country.ToCountryResponse();
		}

		public async Task<List<CountryResponse>> GetAllCountries()
		{
			return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
		}

		public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
		{
			if (countryID == null)
			{
				return null;
			}

			Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == countryID);

			if (country_response_from_list == null)
			{
				return null;
			}

			return country_response_from_list.ToCountryResponse();
		}
	}
}
