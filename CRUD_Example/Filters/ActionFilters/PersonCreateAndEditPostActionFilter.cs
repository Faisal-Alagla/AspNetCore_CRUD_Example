using CRUD_Example.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUD_Example.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before:
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = countries.Select(temp =>
                    new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage).ToList();

                    //getting the passed parameter
                    var personRequest = context.ActionArguments["personRequest"];

                    //when assigning a non-nul value to this Result property, it short circuites the action method
                    //which means the subsequent filters as well as the aciton method won't execute
                    //--> it wil execute the Result
                    //Note: won't short circuite the Result filter
                    context.Result = personsController.View(personRequest);
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
