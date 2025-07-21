using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
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
        public async Task<IActionResult> Index( string? searchString, string searchBy,string sortBy=nameof(PersonResponse.PersonName),SortOrderOptions sortOrder =SortOrderOptions.ASC)
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
            List<PersonResponse> persons =await _personsService.GetFilterPersons(searchBy, searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
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
        public async Task<IActionResult> Create(PersonAddRequest? personAddRequest)
        {
            if (ModelState.IsValid)
            {
                PersonResponse person = await _personsService.AddPerson(personAddRequest);
                return RedirectToAction("Index");
            }
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries;
            ViewBag.Errors =  ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(personAddRequest);
        }

        [Route("[action]/{personId}")]
        [HttpGet]
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
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid personId, PersonUpdateRequest personUpdateRequest)
        {

            if (personId != null)
            {
                bool delete=await _personsService.DeletePerson(personId);
                if (delete)
                {
                    return RedirectToAction("Index");
                }
            }
            
            return View(personUpdateRequest); // Return the view with the model to show validation errors
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
          MemoryStream memoryStream =await _personsService.GetPersonCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv"); 
             

        }

        [Route("PersonExcel")]
        public async Task<IActionResult> PersonExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlxs");


        }
    }
}
