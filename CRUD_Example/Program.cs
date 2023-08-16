using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//logging
builder.Host.ConfigureLogging(loggingProvider =>
{
	loggingProvider.ClearProviders();
	loggingProvider.AddConsole();
	loggingProvider.AddDebug();
	loggingProvider.AddEventLog();
});

//adding services into IoC container
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
	});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

if (!builder.Environment.IsEnvironment("Test"))
{
	Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

//making it partial so that we can access the program class anywhwere in the application (e.g. integration tests)
//+ have to change csproj file so that it's available in other projects within the same solution (edit project file)
public partial class Program { }