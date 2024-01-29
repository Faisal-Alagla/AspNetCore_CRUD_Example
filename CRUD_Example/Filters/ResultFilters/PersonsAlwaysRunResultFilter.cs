using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters.ResultFilters
{
    //When short circuiting Exception, Authorization and Resource filters, Result Filters won't execute
    //But IAlwaysRunResult Filters would still be executed
    //These can do whatever result filters can do
    //The point of them is ensure running the code regardless of what happens in other scenarios (e.g. clearing cache etc...)
    public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            //Make it so that when the skip filter is applied, this filter is skipped
            if (context.Filters.OfType<SkipFilter>().Any())
            {
                return;
            }
        }
    }
}
