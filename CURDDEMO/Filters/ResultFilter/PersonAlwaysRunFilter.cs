using Microsoft.AspNetCore.Mvc.Filters;

namespace CURDDEMO.Filters.ResultFilter
{
    public class PersonAlwaysRunFilter : IAlwaysRunResultFilter
    {
       public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {

            if (context.Filters.OfType<PersonAlwaysRunFilter>().Any())
            {
                return ;
            }
            // before logic
        }
    }
}
