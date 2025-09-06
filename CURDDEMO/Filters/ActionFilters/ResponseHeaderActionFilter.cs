using Microsoft.AspNetCore.Mvc.Filters;

namespace CURDDEMO.Filters.ActionFilters
{
    //public class ResponseHeaderActionFilter : IActionFilter ,IOrderedFilter
    //converting IActionFilter to         IAsyncActionFilter
    //now we can write   OnActionExecuting code before next and         OnActionExecuted after method
    public class ResponseHeaderActionFilter : IAsyncActionFilter ,IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string Key;
        private readonly string Value;
        public int Order { get; set; }
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger,string key,string value,int order)
        {
           _logger = logger;
            Key = key; 
            Value = value;
            Order = order;
        }

        

        //after
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    context.HttpContext.Response.Headers[Key]=Value;
        //    _logger.LogInformation("{filterName}.{MethodName}", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));
        //}

        ////before
        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    context.HttpContext.Response.Headers[Key] = Value;
        //    _logger.LogInformation("{filterName}.{MethodName}", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
        //}

       public async Task  OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Response.Headers[Key] = Value;
            _logger.LogInformation("{filterName}.{MethodName} -before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            await next();
            context.HttpContext.Response.Headers[Key] = Value;
            _logger.LogInformation("{filterName}.{MethodName} - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

        }
    }
}
