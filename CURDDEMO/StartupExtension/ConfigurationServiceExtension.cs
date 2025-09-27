using CRUDExample.Filters.ActionFilters;
using CURDDEMO.Filters.ActionFilters;
using CURDDEMO.Filters.ResultFilter;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;
using System.Configuration;

namespace CURDDEMO.StartupExtension
{
    public static class ConfigurationServiceExtension
    {
        public static IServiceCollection ConfigurationServices(this IServiceCollection services,IConfiguration configuration)
        {
            //builder.Services.AddControllersWithViews();
           services.AddControllersWithViews(options => {
                //options.Filters.Add<ResponceHeaderActionFilter>(5);

                var logger =services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger) { Key = "My-Key-From-Global", Value = "My-Value-From-Global", Order = 2 });
                options.Filters.Add<PersonsListResultFilter>();
            });


            //(registering di) services into ioc container
           services.AddScoped<ICountriesRepository, CountriesRepository>();
           services.AddScoped<IPersonsRepository, PersonsRepository>();
           services.AddTransient<PersonsListActionFilter>();
           services.AddTransient<ResponseHeaderActionFilter>();



            services.AddScoped<ICountriesGetterService, CountriesGetterService>();
            services.AddScoped<ICountriesAdderService, CountriesAdderService>();
            services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();

            //services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();
            services.AddScoped<PersonsGetterService, PersonsGetterService>();



            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
           services.AddScoped<IPersonsAdderService, PersonsAdderService>();
           services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
           services.AddScoped<IPersonsSorterService, PersonsSorterService>();
           services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Connection"));
            });

           services.AddHttpLogging(options =>
            {
                options.LoggingFields =
                Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders;
            });

            return services;
        }
    }
}
