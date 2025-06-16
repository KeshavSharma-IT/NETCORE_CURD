using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CURDDEMO.Controllers
{
    [Route("persons")]
    public class PersonsController : Controller
    {
        //private filed for dpendency injection
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsController(IPersonsService personsService,ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("index")]
        [Route("/")]
        public IActionResult Index( string? searchString, string searchBy,string sortBy=nameof(PersonResponse.PersonName),SortOrderOptions sortOrder =SortOrderOptions.ASC)
        {
            //searching
            ViewBag.SearchFields = new Dictionary<string, string>()
          {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryID), "Country" },
            { nameof(PersonResponse.Address), "Address" }
          };
            List<PersonResponse> persons = _personsService.GetFilterPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries  =_countriesService.GetAllCountries();
            //ViewBag.Countries = countries;
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });


            return View();
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(PersonAddRequest? personAddRequest)
        {
            if (ModelState.IsValid)
            {
                PersonResponse person = _personsService.AddPerson(personAddRequest);
                return RedirectToAction("Index");
            }
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries;
            ViewBag.Errors =  ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(personAddRequest);
        }
    }
}
