using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        //private readonly List<Country> _countries;
        private readonly PersonsDbContext _db;

        //constructor
        public CountriesService(PersonsDbContext personsDbContext )
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

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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
            if (_db.Countries.Count(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries
            _db.Countries.Add(country);
            _db.SaveChanges();

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list = _db.Countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}
