using CRUD_Example.Filters;
using CRUD_Example.Filters.ActionFilters;
using CRUD_Example.Filters.AuthorizationFilters;
using CRUD_Example.Filters.ExceptionFilters;
using CRUD_Example.Filters.ResourceFilters;
using CRUD_Example.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_Example.Controllers
{
    [Route("[controller]")] //persons
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "Controller-Key", "Controller-Value", 3 }, Order = 3)]
    [ResponseHeaderActionFilter("Controller-Key", "Controller-Value", 3)] //Filter Attribute
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonsService personsService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countriesService = countriesService;
            _logger = logger;
        }

        [Route("[action]")] //persons/index
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
        //Order: to manipulate the order of execution sequence of filters (lec. Custom order of filters)
        //preferred way is to implement IOrderedFilter in filter class and provide it as argument (lec. IOrderedFilter)
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-Custom-Key", "Custom-Value", 1 }, Order = 1)]
        [ResponseHeaderActionFilter("Controller-Key", "Controller-Value", 1)] //Filter Attribute
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter] //check PersonsAlwaysRunResultFilter
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            //search options
            //moved to action filter
            //ViewBag.SearchFields = new Dictionary<string, string>()
            //{
            //    { nameof(PersonResponse.PersonName), "Person Name" },
            //    { nameof(PersonResponse.Email), "Email" },
            //    { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            //    { nameof(PersonResponse.Gender), "Gender" },
            //    { nameof(PersonResponse.CountryID), "Country" },
            //    { nameof(PersonResponse.Address), "Address" },
            //};

            //search filtering
            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            //moved to action filter
            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            //sorting
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            //moved to action filter
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder;

            return View(sortedPersons);
        }

        [Route("[action]")] //persons/create
        [HttpGet] // receives only GET requests (view the form)
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "my-key", "my-value", 4 })]
        [ResponseHeaderActionFilter("Controller-Key", "Controller-Value", 4)] //Filter Attribute
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });
            return View();
        }

        [Route("[action]")] //persons/create
        [HttpPost] // post request for form submission
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            //In case of errors, return to the view with the error messages

            //---The validation is done in the filter---

            //if (!ModelState.IsValid)
            //{
            //    List<CountryResponse> countries = await _countriesService.GetAllCountries();
            //    ViewBag.Countries = countries.Select(temp =>
            //    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            //    ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage).ToList();

            //    return View(personRequest);
            //}

            //Add the person
            PersonResponse personResponse = await _personsService.AddPerson(personRequest);

            //navigate to Index() action method in PersonsController (another get request to "persons/index")
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        //[ServiceFilter(typeof(TokenResultFilter))]
        //ServiceFilter is the same as TypeFilter, except that it can't accept Arguments
        //but it has to be added as a service to the IOC containter (program.cs)
        public async Task<IActionResult> Edit(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personRequest.PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            //if (ModelState.IsValid)
            //{
            PersonResponse updatedPerson = await _personsService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
            //}
            //---The following part is done in the filter---
            //else
            //{
            //    List<CountryResponse> countries = await _countriesService.GetAllCountries();
            //    ViewBag.Countries = countries.Select(temp =>
            //    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            //    ViewBag.Errors = ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage).ToList();

            //    return View(personResponse.ToPersonUpdateRequest());
            //}
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            await _personsService.DeletePerson(personUpdateRequest.PersonID);

            return RedirectToAction("Index");
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            //this class comes from the Rotativa package
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCVS")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsCSV();

            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsExcel();

            //Google -> mime type ->...
            return File(memoryStream, "application/vnd.ms-excel", "persons.xlsx");
        }
    }
}
