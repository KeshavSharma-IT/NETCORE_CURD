using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using Services;

namespace CURDDEMO.Controllers
{
    //[Route("[controller]")]
    [Route("Countries")]
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
            if (excelFile == null || excelFile.Length==0)
            {
                ViewBag.ErrorMessage = "Please select an excel file";
                return View();
            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file.Please select an excel file";
                return View();
            }

            int CountriesCount= await _countriesService.UplodeCountriesFromExcelFile(excelFile);
            ViewBag.Message = $"{CountriesCount} countries uploded";
            return View();
        }
    }
}
