using CRUD_Example.Filters.ResultFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUD_Example
{
    public static class ConfigureServicesExtension
    {
        //extension method to extend the IServiceCollection type
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpLogging(options =>
            {
                //http logging options here
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            //adding services into IoC container
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            //In case of using ServiceFilter instead of TypeFilter
            //Here can decide transient / scoped / singleton.. TypeFilter is Transient by default
            //services.AddTransient<TokenResultFilter>();

            //Have to do this for IFilterFactory (check its file)
            services.AddTransient<PersonsListResultFilter>();

            return services;
        }
    }
}
