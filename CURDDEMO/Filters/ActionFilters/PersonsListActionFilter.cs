
using CURDDEMO.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CURDDEMO.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    //after logic
        //    //_logger.LogInformation("PersonListActionFilters.OnActionExecuted method");
        //    //assigning dynamic paramter in logging
        //    _logger.LogInformation("{filterName}.{MethidName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuted));
        //    //accessiong and updating view data
        //    PersonsController personsController=(PersonsController) context.Controller;

        //    IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

        //    if (parameters != null && parameters.ContainsKey("searchBy"))
        //    { 
        //         personsController.ViewData["searchBy"] = parameters["searchBy"];
        //        //we can assign value to view bag here and remove assigning code from controllere if we want
        //    }
        //}

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do: add after logic here
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }

            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
             };

        }

        //note  we can access parameter in   OnActionExecuting function but not in  OnActionExecuted
        //and  we can access ViewData  in   OnActionExecuting function but not in  OnActionExecuted
        // and we are using httpcontext for passing parameter values from OnActionExecuting to OnActionExecuted
        public void OnActionExecuting(ActionExecutingContext context)
        {

            context.HttpContext.Items["arguments"] = context.ActionArguments;
            //before logic
            //_logger.LogInformation("PersonListActionFilters.OnActionExecuting method");

            _logger.LogInformation("{filterName}.{MethidName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            //accessing and updating query data 
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var SearchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address),
                    };

                    if (SearchByOptions.Any(temp=>temp==searchBy)==false) 
                    {
                        _logger.LogInformation($"search By actual value is {searchBy}");
                        context.ActionArguments["searchBy"] =nameof(PersonResponse.PersonName);
                        _logger.LogInformation($"search By updated value is {context.ActionArguments["searchBy"]}");
                    }
                }
            }
        }
    }
}