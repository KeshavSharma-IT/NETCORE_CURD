using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository    : IPersonsRepository
    {
        private readonly ApplicationDbContext _context;
        public PersonsRepository(ApplicationDbContext applicationDbContext)
        {
              _context = applicationDbContext;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _context.Persons.Add(person);
          await  _context.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid Id)
        {
            _context.Persons.RemoveRange(_context.Persons.Where(temp => temp.PersonID == Id));
          int rowDeleted=  await _context.SaveChangesAsync();
            return rowDeleted > 0;

        }

        public async Task<List<Person>> GetAllPersons()
        {
           return await _context.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilterPersons(Expression<Func<Person, bool>> predicate)
        {
           return await _context.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid Id)
        {
            return await _context.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == Id); 
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson=await _context.Persons.FirstOrDefaultAsync(temp=>temp.PersonID == person.PersonID);
            if (matchingPerson != null) { 
                matchingPerson.PersonID = person.PersonID;
                matchingPerson.PersonName = person.PersonName;
                matchingPerson.Gender = person.Gender;
                matchingPerson.Address = person.Address;
                matchingPerson.DateOfBirth = person.DateOfBirth;
                matchingPerson.CountryID = person.CountryID;
                matchingPerson.Email = person.Email;
                matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

                int countUpdate= await _context.SaveChangesAsync();
                return matchingPerson;
            }
            return person;
        }
    }
}
