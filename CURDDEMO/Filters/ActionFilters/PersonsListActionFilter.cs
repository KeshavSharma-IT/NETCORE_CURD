
using CURDDEMO.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CURDDEMO.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //after logic
            //_logger.LogInformation("PersonListActionFilters.OnActionExecuted method");
            //assigning dynamic paramter in logging
            _logger.LogInformation("{filterName}.{MethidName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuted));
            //accessiong and updating view data
            PersonsController personsController=(PersonsController) context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if (parameters != null && parameters.ContainsKey("searchBy"))
            { 
                 personsController.ViewData["searchBy"] = parameters["searchBy"];
                //we can assign value to view bag here and remove assigning code from controllere if we want
            }
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