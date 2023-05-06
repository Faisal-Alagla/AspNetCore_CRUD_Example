using System;
using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace Services
{
	public class PersonsService : IPersonsService
	{
		//private readonly List<Person> _persons;
		private readonly PersonsDbContext _db;
		private readonly ICountriesService _countryService;

		public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
		{
			_db = personsDbContext;
			_countryService = countriesService;

			//dummy data for testing
			//if (initialize)
			//{
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("0ED9DEE8-4EA4-4AF0-8B2A-94A9C1B1AA79"),
			//		PersonName = "Maggy",
			//		Email = "mdooler0@twitpic.com",
			//		DateOfBirth = DateTime.Parse("5/10/1972"),
			//		CountryID = Guid.Parse("9FCBFDF3-CDC0-4819-BD69-6F4664866792"),
			//		Address = "800 Hudson Parkway",
			//		Gender = "Female",
			//		ReceiveNewsLetters = true,
			//	});
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("9ADA7DEE-AB18-4C05-AAA9-9D10A342A940"),
			//		PersonName = "Hurley",
			//		Email = "holuby1@rakuten.co.jp",
			//		DateOfBirth = DateTime.Parse("10/31/1983"),
			//		CountryID = Guid.Parse("220E4ED1-E199-4A1F-B1C6-AD2A8569BF00"),
			//		Address = "28657 Clemons Court",
			//		Gender = "Male",
			//		ReceiveNewsLetters = true,
			//	});
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("9AE058BC-810F-4961-8BF9-0E5640151626"),
			//		PersonName = "Adda",
			//		Email = "asecret2@yahoo.co.jp",
			//		DateOfBirth = DateTime.Parse("12/15/1985"),
			//		CountryID = Guid.Parse("69CC1D32-28CC-42EA-8166-1AEFE5A90244"),
			//		Address = "7466 Pawling Plaza",
			//		Gender = "Female",
			//		ReceiveNewsLetters = true,
			//	});
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("D2E9DE3A-3105-4D97-80E2-C4EDEA23C837"),
			//		PersonName = "Meagan",
			//		Email = "mvanderlinde3@trellian.com",
			//		DateOfBirth = DateTime.Parse("7/29/1992"),
			//		CountryID = Guid.Parse("69CC1D32-28CC-42EA-8166-1AEFE5A90244"),
			//		Address = "95631 Pierstorff Point",
			//		Gender = "Female",
			//		ReceiveNewsLetters = true,
			//	});
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("DD9EE5F7-E25C-45EC-AD5D-D458D7E84684"),
			//		PersonName = "Odell",
			//		Email = "ostorrock6@t-online.de",
			//		DateOfBirth = DateTime.Parse("5/20/1986"),
			//		CountryID = Guid.Parse("8313AD55-0E9E-4ACE-900F-8E8F14338ECF"),
			//		Address = "5 Onsgard Point",
			//		Gender = "Male",
			//		ReceiveNewsLetters = false,
			//	});
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("D146FD10-4DB8-4C7E-AE1D-A1FC3F86FFD5"),
			//		PersonName = "Thia",
			//		Email = "tmocquer7@goo.gl",
			//		DateOfBirth = DateTime.Parse("7/4/1992"),
			//		CountryID = Guid.Parse("8313AD55-0E9E-4ACE-900F-8E8F14338ECF"),
			//		Address = "9508 Little Fleur Park",
			//		Gender = "Female",
			//		ReceiveNewsLetters = false,
			//	});
			//	_persons.Add(new Person
			//	{
			//		PersonID = Guid.Parse("1A7630CB-7A87-447B-A3BC-BF69BC032014"),
			//		PersonName = "Chelsae",
			//		Email = "cbrenstuhl8@umich.edu",
			//		DateOfBirth = DateTime.Parse("6/30/1984"),
			//		CountryID = Guid.Parse("6D630EF4-73F7-43CA-99A6-E7B3C1E58D2E"),
			//		Address = "0739 Valley Edge Avenue",
			//		Gender = "Female",
			//		ReceiveNewsLetters = true,
			//	});
			//}
		}

		public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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
			_db.Persons.Add(person);
			await _db.SaveChangesAsync();
			//_db.sp_InsertPerson(person);

			return person.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetAllPersons()
		{
			//return _persons.Select(person => ConvertPersonToPersonResponse(person)).ToList();

			//Include -> navigation property (instead of joints)
			List<Person> persons = await _db.Persons.Include("Country").ToListAsync(); // SELECT * from persons (loading from DB to memory)
			return persons.Select(person => person.ToPersonResponse()).ToList();

			//using a stored procedure
			//return _db.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
			//^^^ To use this: need to update the stored procedure to include the new TIN column.. lec.209 ^^^
		}

		public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
		{
			if (personID == null) { return null; }

			Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.PersonID == personID);
			//now it's possible to access this person's country object
			//person.Country.CountryName;

			if (person == null) { return null; }

			return person.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
		{
			List<PersonResponse> allPersons = await GetAllPersons();
			List<PersonResponse> matchingPersons = allPersons;

			if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString)) { return matchingPersons; }

			//Note: can use reflection instead of this
			switch (searchBy)
			{
				case nameof(PersonResponse.PersonName):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.PersonName)) ?
					temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.Email):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Email)) ?
					temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.DateOfBirth):
					matchingPersons = allPersons.Where(temp =>
					(temp.DateOfBirth != null) ?
					temp.DateOfBirth.Value.ToString("dd MM yyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.Gender):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Gender)) ?
					temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.CountryID):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Country)) ?
					temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.Address):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Address)) ?
					temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				default: matchingPersons = allPersons; break;
			}

			return matchingPersons;
		}

		public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
		{
			if (string.IsNullOrEmpty(sortBy)) { return allPersons; }

			//Note: can use reflection instead of this
			List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
			{
				(nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
				(nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
				(nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
				(nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
				(nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
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

		public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			//Validations:
			if (personUpdateRequest == null) { throw new ArgumentNullException(nameof(personUpdateRequest)); }

			ValidationHelper.ModelValidation(personUpdateRequest);

			//get matching person object to update
			Person matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID) ?? throw new ArgumentException("Given PersonID doesn't exist");

			//update
			matchingPerson.PersonName = personUpdateRequest.PersonName;
			matchingPerson.Email = personUpdateRequest.Email;
			matchingPerson.CountryID = personUpdateRequest.CountryID;
			matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
			matchingPerson.Gender = personUpdateRequest.Gender.ToString();
			matchingPerson.Address = personUpdateRequest.Address;
			matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
			//^^^ state: "Modified" ^^^

			await _db.SaveChangesAsync(); //execute UPDATE

			return matchingPerson.ToPersonResponse();
		}

		public async Task<bool> DeletePerson(Guid? personID)
		{
			if (personID == null) { throw new ArgumentNullException(nameof(personID)); }

			Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personID);

			if (person == null) { return false; }

			//_persons.RemoveAll(temp => temp.PersonID == personID);
			_db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID)); //state: "Removed"
			await _db.SaveChangesAsync(); //execute DELETE

			return true;
		}

		public async Task<MemoryStream> GetPersonsCSV()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);

			//problem with below commented code is that we can't control which properties to include (all properties)
			//this will help us customize the csv file
			CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
			CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

			//PersonName,Email,DateOfBirth,Country
			csvWriter.WriteField(nameof(PersonResponse.PersonName));
			csvWriter.WriteField(nameof(PersonResponse.Email));
			csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
			csvWriter.WriteField(nameof(PersonResponse.Country));

			//CsvWriter comes form CsvHelper package
			//lec. 217... 218 = more methods
			//CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

			//csvWriter.WriteHeader<PersonResponse>(); //PersonID,PersonName,...
			csvWriter.NextRecord(); // \n

			List<PersonResponse> persons = _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();

			foreach (PersonResponse person in persons)
			{
				csvWriter.WriteField(person.PersonName);
				csvWriter.WriteField(person.Email);
				if (person.DateOfBirth.HasValue) { csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd")); }
				csvWriter.WriteField(person.Country);

				csvWriter.NextRecord();
				csvWriter.Flush();
			}
			//await csvWriter.WriteRecordsAsync(persons); //1,abc,...

			memoryStream.Position = 0;
			return memoryStream;
		}

		public async Task<MemoryStream> GetPersonsExcel()
		{
			MemoryStream memoryStream = new MemoryStream();
			//from EPPlus package..
			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
				workSheet.Cells["A1"].Value = "Person Name";
				workSheet.Cells["B1"].Value = "Person Email";
				workSheet.Cells["C1"].Value = "Date of Birth";
				workSheet.Cells["D1"].Value = "Country";

				int row = 2;
				List<PersonResponse> persons = _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();

				foreach (PersonResponse person in persons)
				{
					//[row, column]
					workSheet.Cells[row, 1].Value = person.PersonName;
					workSheet.Cells[row, 2].Value = person.Email;
					if (person.DateOfBirth != null) { workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd"); }
					workSheet.Cells[row, 4].Value = person.Country;

					//formatting header cells
					using (ExcelRange headerCells = workSheet.Cells["A1,D1"])
					{
						headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
						headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
						headerCells.Style.Font.Bold = true;
					}

					row++;
				}
				//### google epplus for documentation (more features etc...) ###

				workSheet.Cells[$"A1:D{row}"].AutoFitColumns();

				await excelPackage.SaveAsync();
			}

			memoryStream.Position = 0;
			return memoryStream;
		}
	}
}
