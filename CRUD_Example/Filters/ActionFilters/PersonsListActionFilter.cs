using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_Example.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = context.HttpContext.Items["arguments"] as IDictionary<string, object?>;

            if (parameters != null)
            {
                if (parameters.ContainsKey("CurrentSearchBy"))
                {
                    personsController.ViewBag["CurrentSearchBy"] = Convert.ToString(parameters["CurrentSearchBy"]);
                }

                if (parameters.ContainsKey("CurrentSearchString"))
                {
                    personsController.ViewBag["CurrentSearchString"] = Convert.ToString(parameters["CurrentSearchString"]);
                }

                if (parameters.ContainsKey("CurrentSortBy"))
                {
                    personsController.ViewBag["CurrentSortBy"] = Convert.ToString(parameters["CurrentSortBy"]);
                }
                else
                {
                    personsController.ViewBag["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }

                if (parameters.ContainsKey("CurrentSortOrder"))
                {
                    personsController.ViewBag["CurrentSortOrder"] = Convert.ToString(parameters["CurrentSortOrder"]);
                }
                else
                {
                    personsController.ViewBag["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }

            personsController.ViewData["SearchFields"] = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };

                    if (!searchByOptions.Any(temp => temp == searchBy))
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);

                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);

                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
