using Microsoft.AspNetCore.Mvc;

namespace CRUD_Example.Controllers
{
	public class PersonsController : Controller
	{
		[Route("persons/index")]
		[Route("/")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
