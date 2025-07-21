using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        //private readonly List<Country> _countries;
        private readonly ApplicationDbContext _db;

        //constructor
        public CountriesService(ApplicationDbContext personsDbContext )
        {
            _db = personsDbContext;
            //if (initialize)
            //{

            //    _db.AddRange(new List<Country>() {
            //        new Country()
            //        {
            //            CountryID = Guid.Parse("90EBF025-33E6-4AF2-A441-E02D70E331BF"),
            //            CountryName = "India"
            //        },
            //        new Country()
            //        {
            //            CountryID = Guid.Parse("86CF7B5C-3ABA-4ECD-808A-85BCF43DA0A3"),
            //            CountryName = "United States of America"
            //        },
            //        new Country()
            //        {
            //            CountryID = Guid.Parse("9ED80C93-910E-43D8-8D20-894BD28F6524"),
            //            CountryName = "United Kingdom"
            //        },
            //        new Country()
            //        {
            //            CountryID = Guid.Parse("26F4D3B7-814B-4906-8BC0-7B4A05834EEE"),
            //            CountryName = "Canada"
            //        },
            //        new Country()
            //        {
            //            CountryID = Guid.Parse("41650245-C74D-472C-806D-04DBDAAF1133"),
            //            CountryName = "Australia"
            //        }
            //    });
                
            //}
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {

            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }

        public async Task<int> UplodeCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countryInserted = 0;

            using (ExcelPackage excelPakage = new ExcelPackage(memoryStream))
            {
              ExcelWorksheet excelWorksheet=  excelPakage.Workbook.Worksheets["Countries"];
                int row = excelWorksheet.Dimension.Rows;
                for (int i = 2; i < row; i++) 
                {
                    string? cellValue=Convert.ToString(excelWorksheet.Cells[i, 1].Value);

                    if(!string.IsNullOrEmpty(cellValue))
                    {
                        string countryName = cellValue;

                        if ((_db.Countries.Where(temp => temp.CountryName == countryName).Count()) ==0)
                        {
                            Country country = new Country() { CountryName= countryName };
                             _db.Countries.Add(country);
                            await _db.SaveChangesAsync();

                            countryInserted++;
                        }   
                    }
                }
            }
                return countryInserted;
        }
    }
}
