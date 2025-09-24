using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private string? Key { get; set; }
        private string? Value { get; set; }
        private int Order { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            Key = key;
            Value = value;
            Order = order;
        }

        //Controller -> FilterFactory -> Filter
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;
            //return filter object
            return filter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Before logic - ResponseHeaderActionFilter");

            await next(); //calls the subsequent filter or action method

            _logger.LogInformation("Before logic - ResponseHeaderActionFilter");

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}



//using Microsoft.AspNetCore.Mvc.Filters;

//namespace CURDDEMO.Filters.ActionFilters
//{
//    //public class ResponseHeaderActionFilter : IActionFilter ,IOrderedFilter
//    //converting IActionFilter to         IAsyncActionFilter
//    //now we can write   OnActionExecuting code before next and         OnActionExecuted after method

//    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
//    {
//        public bool IsReusable => false;

//        private string Key { get; set; }
//        private string Value { get; set; }
//        private int Order { get; set; }
//        public ResponseHeaderFilterFactoryAttribute(string key,string value,int order)
//        {
//            Key = key;
//            Value = value;
//            Order = order;

//        }

//        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
//        {
//            // return filter object
//            var filter= new ResponseHeaderActionFilter(Key, Value, Order);
//            return filter;
//        }
//    }
//    public class ResponseHeaderActionFilter : IAsyncActionFilter , IOrderedFilter
//    //public class ResponseHeaderActionFilter : ActionFilterAttribute
//    {
//        //private readonly ILogger<ResponseHeaderActionFilter> _logger;   we cant inject constructor injection in ActionFilterAttribute 
//        private readonly string Key;
//        private readonly string Value;
//        public int Order { get; set; }
//        public ResponseHeaderActionFilter(string key,string value,int order)
//        {
//           //_logger = logger;
//            Key = key; 
//            Value = value;
//            Order = order;
//        }



//        //after
//        //public void OnActionExecuted(ActionExecutedContext context)
//        //{
//        //    context.HttpContext.Response.Headers[Key]=Value;
//        //    _logger.LogInformation("{filterName}.{MethodName}", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));
//        //}

//        ////before
//        //public void OnActionExecuting(ActionExecutingContext context)
//        //{
//        //    context.HttpContext.Response.Headers[Key] = Value;
//        //    _logger.LogInformation("{filterName}.{MethodName}", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
//        //}

//        // we have to oveerride as      in ActionFilterAttribute  these function is virtual
//        //public override async Task  OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        //{
//        //    context.HttpContext.Response.Headers[Key] = Value;
//        //    //_logger.LogInformation("{filterName}.{MethodName} -before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

//        //    await next();
//        //    context.HttpContext.Response.Headers[Key] = Value;
//        //    //_logger.LogInformation("{filterName}.{MethodName} - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

//        //}

//        public  async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            context.HttpContext.Response.Headers[Key] = Value;
//            //_logger.LogInformation("{filterName}.{MethodName} -before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

//            await next();
//            context.HttpContext.Response.Headers[Key] = Value;
//            //_logger.LogInformation("{filterName}.{MethodName} - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

//        }
//    }
//}
