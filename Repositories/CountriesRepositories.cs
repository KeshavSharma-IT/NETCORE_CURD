using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CountriesRepositories : ICountriesRepository
    {
        private readonly ApplicationDbContext _context;
        public CountriesRepositories(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Country> AddCountry(Country country)
        {
             _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountry()
        {
            return await _context.Countries.ToListAsync();
             
        }

        public async Task<Country?> GetCountryByCountryID(Guid id)
        {
         return  await  _context.Countries.FirstOrDefaultAsync(temp=>temp.CountryID== id);
        }

        public async Task<Country?> GetCountryByCountryName(string CountryName)
        {
               return await _context.Countries.FirstOrDefaultAsync(temp=>temp.CountryName== CountryName);
        }
    }
}
