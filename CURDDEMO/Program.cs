using CRUDExample.Filters.ActionFilters;
using CURDDEMO.Filters.ActionFilters;
using CURDDEMO.Filters.ResultFilter;
using CURDDEMO.StartupExtension;
using CURDDEMO.Middleware;
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

builder.Services.ConfigurationServices(builder.Configuration);
// we can call  ConfigurationServices class without parameter but need builder.
// Configuration  there for calling DB Connection so that why we are taking parameter from here


var app = builder.Build();

app.UseSerilogRequestLogging();


app.UseHttpLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandlingMiddleware();
}

if (builder.Environment.IsEnvironment("Test") == false)
{

    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotiativa");
}
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
public partial class Program { }
