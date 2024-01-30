using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Example.Controllers
{
    public class HomeController : Controller
    {
        [Route("/Error")]
        public IActionResult Error()
        {
            //to get the exception details (to show different error messages depending on the exception)
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature is not null && exceptionHandlerPathFeature.Error is not null)
            {
                ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            }

            return View();
        }
    }
}
