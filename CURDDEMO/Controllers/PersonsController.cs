using CRUDExample.Filters.ActionFilters;
using CURDDEMO.Filters;
using CURDDEMO.Filters.ActionFilters;
using CURDDEMO.Filters.AuthorizationFilters;
using CURDDEMO.Filters.ExceptionFilters;
using CURDDEMO.Filters.ResourseFilter;
using CURDDEMO.Filters.ResultFilter;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CURDDEMO.Controllers
{
    //[Route("persons")]
    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-Key-From-Controller", "My-Value-From-Controller", 3 }, Order = 3)]
    //[ResponseHeaderActionFilter("My-Key-From-Controller", "My-Value-From-Controller", 3)]
    [ResponseHeaderFilterFactoryAttribute("My-Key-From-Controller", "My-Value-From-Controller", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunFilter))]
    public class PersonsController : Controller
    {
        //private filed for dpendency injection
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        //constructor
        public PersonsController(IPersonsService personsService,ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countriesService = countriesService;
            _logger = logger;
        }

        //[Route("index")]
        //[Route("[action]")]
        //[Route("/")]
        //[TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "MyKey-FromAction", "MyValue-From-Action", 1 }, Order = 1)]
        //[TypeFilter(typeof(PersonsListResultFilter))]
        //public async Task<IActionResult> Index( string? searchString, string searchBy,string sortBy=nameof(PersonResponse.PersonName),SortOrderOptions sortOrder =SortOrderOptions.ASC)
        //{
        //    _logger.LogInformation("Index Action Method of personController");

        //    _logger.LogDebug($"searchBy: {searchBy}, serachstring : {searchString} , sortby:{sortBy}, SortOrder : {sortOrder}");

        //    //searching
        //    ViewBag.SearchFields = new Dictionary<string, string>()
        //  {
        //    { nameof(PersonResponse.PersonName), "Person Name" },
        //    { nameof(PersonResponse.Email), "Email" },
        //    { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
        //    { nameof(PersonResponse.Gender), "Gender" },
        //    { nameof(PersonResponse.CountryID), "Country" },
        //    { nameof(PersonResponse.Address), "Address" }
        //  };
        //    List<PersonResponse> persons =await _personsService.GetFilterPersons(searchBy, searchString);
        //    ViewBag.CurrentSearchBy = searchBy;
        //    ViewBag.CurrentSearchString = searchString;

        //    //Sort
        //    List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        //    ViewBag.CurrentSortBy = sortBy;
        //    ViewBag.CurrentSortOrder = sortOrder.ToString();

        //    return View(sortedPersons);
        //}


        [Route("[action]")]
        [Route("/")]
        //[TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]
        [TypeFilter(typeof(ResponseHeaderFilterFactoryAttribute), Arguments = new object[] { "MyKey-FromAction", "MyValue-From-Action", 1 }, Order = 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [TypeFilter(typeof(SkipFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
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
            _logger.LogInformation("Index action method of PersonsController");

            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");


            //Search
            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries  =await _countriesService.GetAllCountries();
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
        [TypeFilter(typeof(PersonCreateandEditPostFilter))]

        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments = new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest? personRequest)
        {
            //if (ModelState.IsValid)
            //{
            //    PersonResponse person = await _personsService.AddPerson(personRequest);
            //    return RedirectToAction("Index");
            //}
            //List<CountryResponse> countries = await _countriesService.GetAllCountries();
            //ViewBag.Countries = countries;
            //ViewBag.Errors =  ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            PersonResponse personResponse = await _personsService.AddPerson(personRequest);

            return View(personRequest);
        }

        [Route("[action]/{personId}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        
        public async Task<IActionResult> Edit(Guid personId) { 
            
           PersonResponse personResponse=await _personsService.GetPersonByPersonID(personId);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            List<CountryResponse> countries = await _countriesService.GetAllCountries();  
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });
            return View(personUpdateRequest);
            
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(TokenAuthFilter))]
        public async Task<IActionResult> Edit(Guid personId, PersonUpdateRequest personUpdateRequest) { 
            
            if(ModelState.IsValid)
            {
                PersonResponse personResponse = await _personsService.UpdatePerson(personUpdateRequest);
                if (personResponse != null)
                {
                    return RedirectToAction("Index");
                }                 
            }
            List<CountryResponse> countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });
            return View(personUpdateRequest); // Return the view with the model to show validation errors
        }


        [Route("[action]/{personId}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personId)
        {

            PersonResponse personResponse =await _personsService.GetPersonByPersonID(personId);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();           
            return View(personUpdateRequest);

        }

        
        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }

        [Route("PersonPdf")]
        public async Task<IActionResult> PersonPdf()
        {
           List<PersonResponse> personResponses=await _personsService.GetAllPersons();
            return new ViewAsPdf("PersonPdf", personResponses, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };

        }


        [Route("PersonCsv")]
        public async Task<IActionResult> PersonCsv()
        {
          MemoryStream memoryStream =await _personsService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv"); 
             

        }

        [Route("PersonExcel")]
        public async Task<IActionResult> PersonExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlxs");


        }
    }
}
