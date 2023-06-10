using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUD_Example.Controllers
{
	[Route("[controller]")]
	public class CountriesController : Controller
	{
		private readonly ICountriesService _countriesService;

		public CountriesController(ICountriesService countriesService)
		{
			_countriesService = countriesService;
		}

		[Route("UploadFromExcel")]
		public IActionResult UploadFromExcel()
		{
			return View();
		}

		[HttpPost]
		[Route("UploadFromExcel")]
		public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
		{
			if (excelFile == null || excelFile.Length == 0)
			{
				ViewBag.ErrorMessage = "Please Select an Excel file";
				return View();
			}

			if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
			{
				ViewBag.ErrorMessage = "Unsupported file, it must be an xlsx file!";
				return View();
			}

			int countriesCountInserted = await _countriesService.UploadCountriesFromExcelFile(excelFile);

			ViewBag.Message = $"{countriesCountInserted} Countries Uploaded";
			return View();
		}
	}
}
