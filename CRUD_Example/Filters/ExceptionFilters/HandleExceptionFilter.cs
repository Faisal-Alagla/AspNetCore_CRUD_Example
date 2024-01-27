using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters.ExceptionFilters
{
    //Note: Exception filters are for handling exceptions in modelbinding, action filter and action method
    //To handle other filters / any other middleware -> use exception handling middleware (broader)
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HandleExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            //To short circuite -> have to do one of the folowing
            //context.Result = some action result...
            //context.ExceptionHandled = true;

            //for IAsyncExceptionFilter:
            //return Task.CompletedTask;

            _logger.LogError("Exception filter {FilterName}.{MethodName}\n{ExceptionType}\n{ExceptionMessage}", nameof(HandleExceptionFilter), nameof(OnException), context.Exception.GetType().ToString(), context.Exception.Message);

            if (_hostEnvironment.IsDevelopment())
            {
                context.Result = new ContentResult()
                {
                    Content = context.Exception.Message,
                    StatusCode = 500
                };
            }
        }
    }
}
