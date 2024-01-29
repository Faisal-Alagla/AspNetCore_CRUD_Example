using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters.ResultFilters
{
    //Note: The regular TypeFilter / ServiceFilters AND IFilterFactory are the best ways to implement filters (best practices)... try to avoid Attribute filters (not recommended) due to not being able to DI
    public class PersonsListFilterFactoryAttribute : Attribute, IFilterFactory
    {
        //default is false, in case of true -> means the filter object can be accessed across multiple requests in the app
        public bool IsReusable => false;
        private string SomeProperty { get; set; }
        private int Order { get; set; }

        public PersonsListFilterFactoryAttribute(string someProperty, int order)
        {
            SomeProperty = someProperty;
            Order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //return the filter object here
            //return new PersonsListResultFilter(Order, SomeProperty);

            //must be added to the IOC container
            var filter = serviceProvider.GetRequiredService<PersonsListResultFilter>();

            filter.SomeProperty = SomeProperty;
            filter.Order = Order;

            return filter;
        }
    }

    public class PersonsListResultFilter : IAsyncResultFilter, IOrderedFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;
        //private readonly string _someProperty;
        public string SomeProperty { get; set; }

        public int Order { get; set; }

        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger/*, int order, string someProperty*/)
        {
            _logger = logger;
            //Order = order;
            //_someProperty = someProperty;
        }


        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            await next();

            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

            //this failed a test case (lec.287)
            //context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
