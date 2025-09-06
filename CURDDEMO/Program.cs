using CURDDEMO.Filters.ActionFilters;
using CURDDEMO.Filters.ResultFilter;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;
var builder = WebApplication.CreateBuilder(args);

//loggiong
//builder.Host.ConfigureLogging(logging =>
//{
//    logging.ClearProviders();
//    logging.AddConsole();
//});

//Serilog           
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider service,LoggerConfiguration loggerConfiguration) =>
{
         loggerConfiguration.ReadFrom.Configuration(context.Configuration)
         .ReadFrom.Services(service);
});

//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(options => {
    //options.Filters.Add<ResponceHeaderActionFilter>(5);

    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

    options.Filters.Add(new ResponseHeaderActionFilter(logger, "My-Key-From-Global", "My-Value-From-Global", 2));
    options.Filters.Add<PersonsListResultFilter>();
});


//(registering di) services into ioc container
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();



builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection"));
});

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =     
    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties 
    | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders;
});
var app = builder.Build();

app.UseSerilogRequestLogging();


app.UseHttpLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (builder.Environment.IsEnvironment("Test")==false)
{

    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotiativa");
}
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }