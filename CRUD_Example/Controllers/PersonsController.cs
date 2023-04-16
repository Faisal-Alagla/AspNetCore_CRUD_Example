using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_Example.Controllers
{
	[Route("[controller]")] //persons
	public class PersonsController : Controller
	{
		private readonly IPersonsService _personsService;
		private readonly ICountriesService _countriesService;

		public PersonsController(IPersonsService personsService, ICountriesService countriesService)
		{
			_personsService = personsService;
			_countriesService = countriesService;
		}

		[Route("[action]")] //persons/index
		[Route("/")]
		public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
		{
			//search options
			ViewBag.SearchFields = new Dictionary<string, string>()
			{
				{ nameof(PersonResponse.PersonName), "Person Name" },
				{ nameof(PersonResponse.Email), "Email" },
				{ nameof(PersonResponse.DateOfBirth), "Date of Birth" },
				{ nameof(PersonResponse.Gender), "Gender" },
				{ nameof(PersonResponse.CountryID), "Country" },
				{ nameof(PersonResponse.Address), "Address" },
			};

			//search filtering
			List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);
			ViewBag.CurrentSearchBy = searchBy;
			ViewBag.CurrentSearchString = searchString;

			//sorting
			List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
			ViewBag.CurrentSortBy = sortBy;
			ViewBag.CurrentSortOrder = sortOrder;

			return View(sortedPersons);
		}

		[Route("[action]")] //persons/create
		[HttpGet] // receives only GET requests (view the form)
		public IActionResult Create()
		{
			List<CountryResponse> countries = _countriesService.GetAllCountries();
			ViewBag.Countries = countries.Select(temp =>
			new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });
			return View();
		}

		[Route("[action]")] //persons/create
		[HttpPost] // post request for form submission
		public IActionResult Create(PersonAddRequest personAddRequest)
		{
			//In case of errors, return to the view with the error messages
			if (!ModelState.IsValid)
			{
				List<CountryResponse> countries = _countriesService.GetAllCountries();
				ViewBag.Countries = countries.Select(temp =>
				new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

				ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage).ToList();

				return View();
			}

			//Add the person
			PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

			//navigate to Index() action method in PersonsController (another get request to "persons/index")
			return RedirectToAction("Index", "Persons");
		}

		[Route("[action]/{personID}")]
		[HttpGet]
		public IActionResult Edit(Guid? personID)
		{
			PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

			List<CountryResponse> countries = _countriesService.GetAllCountries();
			ViewBag.Countries = countries.Select(temp =>
			new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

			return View(personUpdateRequest);
		}

		[Route("[action]/{personID}")]
		[HttpPost]
		public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
		{
			PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
			if (personResponse == null)
			{
				return RedirectToAction("Index");
			}

			if (ModelState.IsValid)
			{
				PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
				return RedirectToAction("Index");
			}
			else
			{
				List<CountryResponse> countries = _countriesService.GetAllCountries();
				ViewBag.Countries = countries.Select(temp =>
				new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

				ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage).ToList();

				return View(personResponse.ToPersonUpdateRequest());
			}
		}

		[Route("[action]/{personID}")]
		[HttpGet]
		public IActionResult Delete(Guid? personID)
		{
			PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);
			if(personResponse == null)
			{
				return RedirectToAction("Index");
			}

			return View(personResponse);
		}

		[Route("[action]/{personID}")]
		[HttpPost]
		public IActionResult Delete(PersonUpdateRequest personUpdateRequest)
		{
			PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
			if(personResponse == null)
			{
				return RedirectToAction("Index");
			}

			_personsService.DeletePerson(personUpdateRequest.PersonID);

			return RedirectToAction("Index");
		}
	}
}
