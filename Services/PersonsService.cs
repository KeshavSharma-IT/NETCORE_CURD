using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private field
        //private readonly List<Person> _persons;
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        
        //constructor
        public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
           
        }


        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //check if PersonAddRequest is not null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            //add person object to persons list
            _db.Persons.Add(person);
            _db.SaveChanges();

            //call stored procedure to insert person
            //_db.sp_InsertPerson(person);

            //convert the Person object into PersonResponse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _db.Persons.ToList().
                Select(temp => ConvertPersonToPersonResponse(temp)).ToList();


            // get with stored procedure
            //return _db.sp_GetAllPersons()
            //    .Select(temp => ConvertPersonToPersonResponse(temp))
            //    .ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilterPersons(string searchby, string? SearchString)
        {
            List<PersonResponse> persons = GetAllPersons();
            List<PersonResponse> filteredPersons = persons;

            if(string.IsNullOrEmpty(searchby) || string.IsNullOrEmpty(SearchString))
            {
                return filteredPersons;
            }

            switch (searchby)
            {
                case nameof(Person.PersonName):
                    filteredPersons = persons.Where(temp => temp.PersonName != null && temp.PersonName.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(Person.Email):
                    filteredPersons = persons.Where(temp => temp.Email != null && temp.Email.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(Person.DateOfBirth):
                    if (DateTime.TryParse(SearchString, out DateTime dateOfBirth))
                    {
                        filteredPersons = persons.Where(temp => temp.DateOfBirth != null && temp.DateOfBirth.Value.Date == dateOfBirth.Date).ToList();
                    }
                    break;
                case nameof(Person.Gender):
                    filteredPersons=persons.Where(temp=>temp.Gender!=null && temp.Gender.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case nameof(Person.CountryID):                    
                        filteredPersons = persons.Where(temp=>(!string.IsNullOrEmpty(temp.Country) ? temp.Country.Contains(SearchString, StringComparison.OrdinalIgnoreCase) : false)).ToList();
                    break;
                case nameof(Person.Address):
                    filteredPersons = persons.Where(temp => temp.Address != null && temp.Address.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                default:
                    filteredPersons = persons ;
                    break;
            }
            return filteredPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allperson, string sortby, SortOrderOptions sortOrder)
        {
            if(string.IsNullOrEmpty(sortby))
            {
                return allperson;
            }
            List<PersonResponse> sortedPersons = (sortby, sortOrder)
                switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.PersonName).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allperson.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allperson.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allperson
            }; 
            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
           
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            //Model validation
            ValidationHelper.ModelValidation(personUpdateRequest);
            //check if person exists
            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
            if (person == null)
            {
                throw new ArgumentException("Person not found with the given PersonID");
            }
            //update person details
            person.PersonName = personUpdateRequest.PersonName;
            person.Email = personUpdateRequest.Email;
            person.DateOfBirth = personUpdateRequest.DateOfBirth;
            person.Address= personUpdateRequest.Address;
            person.Gender = personUpdateRequest.Gender.ToString();
            person.CountryID = personUpdateRequest.CountryID;
            person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            //convert the Person object into PersonResponse type
            //PersonResponse personResponse = ConvertPersonToPersonResponse(person);

            //return the updated person response
            _db.SaveChanges();
            return ConvertPersonToPersonResponse(person);

        }

        public bool DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            //check if person exists
            Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
            {
                return false;
            }
            //remove person from persons list
            _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID) ) ;
            _db.SaveChanges();
            return true;

        }
    }
}