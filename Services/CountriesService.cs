using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesService : ICountriesService
	{
		//this will be replaced later (with EF)
		private readonly List<Country> _countries;

		//constructor
		public CountriesService(bool initialize = true)
		{
			_countries = new List<Country>();

			//dummy data for testing
			if (initialize)
			{
				_countries.AddRange(new List<Country>
				{
				new Country { CountryID = Guid.Parse("9FCBFDF3-CDC0-4819-BD69-6F4664866792"), CountryName = "KSA" },
				new Country { CountryID = Guid.Parse("8313AD55-0E9E-4ACE-900F-8E8F14338ECF"), CountryName = "DE" },
				new Country { CountryID = Guid.Parse("6D630EF4-73F7-43CA-99A6-E7B3C1E58D2E"), CountryName = "UK" },
				new Country { CountryID = Guid.Parse("220E4ED1-E199-4A1F-B1C6-AD2A8569BF00"), CountryName = "USA" },
				new Country { CountryID = Guid.Parse("69CC1D32-28CC-42EA-8166-1AEFE5A90244"), CountryName = "JP" },
				});
			}
		}

		public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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
			if (_countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
			{
				throw new ArgumentException("Given country name already exists");
			}

			//Convert object from CountryAddRequest to Country type
			Country country = countryAddRequest.ToCountry();

			//generate CountryID
			country.CountryID = Guid.NewGuid();

			//Add country object to _countries list
			_countries.Add(country);

			return country.ToCountryResponse();
		}

		public List<CountryResponse> GetAllCountries()
		{
			return _countries.Select(country => country.ToCountryResponse()).ToList();
		}

		public CountryResponse? GetCountryByCountryID(Guid? countryID)
		{
			if (countryID == null)
			{
				return null;
			}

			Country? country_response_from_list = _countries.FirstOrDefault(country => country.CountryID == countryID);

			if (country_response_from_list == null)
			{
				return null;
			}

			return country_response_from_list.ToCountryResponse();
		}
	}
}
