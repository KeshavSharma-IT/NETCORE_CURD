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
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsService(bool initialize= true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _persons.AddRange(new List<Person>()
                {
                    new Person() { PersonID=Guid.Parse("F27C0F98-1FEA-4D36-A9D9-F3FC39A9F4B3"),
                        PersonName="Edmon",Email="eexcell0@jigsy.com",Address="PIAZZA FILIPPO MEDA 4",
                        DateOfBirth=DateTime.Parse("2017-02-08"),Gender="Male",ReceiveNewsLetters=true,
                        CountryID=Guid.Parse("90EBF025-33E6-4AF2-A441-E02D70E331BF")},
                    
                    new Person()
                    {
                        PersonID = Guid.Parse("93B0FB5A-F3CB-44D1-B1A2-B8088B570335"),
                        PersonName = "Kathryne",
                        Email = "kgann1@elpais.com",
                        Address = "P.-B.-Rodlbergerstraße",
                        DateOfBirth = DateTime.Parse("2008-11-29"),
                        Gender = "Female",
                        ReceiveNewsLetters = false,
                        CountryID = Guid.Parse("86CF7B5C-3ABA-4ECD-808A-85BCF43DA0A3")
                    },
                    new Person()
                    {
                        PersonID = Guid.Parse("63A9DE31-B82A-4E76-AACA-D9D30B9771D2"),
                        PersonName = "Vin",
                        Email = "vhankins2@newyorker.com",
                        Address = "PIAZZA FILIPPO MEDA 4",
                        DateOfBirth = DateTime.Parse("2001-12-18"),
                        Gender = "Male",
                        ReceiveNewsLetters = false,
                        CountryID = Guid.Parse("9ED80C93-910E-43D8-8D20-894BD28F6524")
                    },

                    new Person()
                    {
                        PersonID = Guid.Parse("ACF1C7EF-677B-48F7-9B64-85A2AE64B63D"),
                        PersonName = "Mirabel",
                        Email = "msurphliss3@simplemachines.org",
                        Address = "PO BOX 32282",
                        DateOfBirth = DateTime.Parse("1999-08-20"),
                        Gender = "Feale",
                        ReceiveNewsLetters = false,
                        CountryID = Guid.Parse("26F4D3B7-814B-4906-8BC0-7B4A05834EEE")
                    },

                    new Person()
                    {
                        PersonID = Guid.Parse("412EC5CB-4990-41A1-B63B-F6FB2A8F96BD"),
                        PersonName = "Melvyn",
                        Email = "mhowson4@skyrock.com",
                        Address = "Kölnische Straße 8",
                        DateOfBirth = DateTime.Parse("2014-09-17"),
                        Gender = "Female",
                        ReceiveNewsLetters = false,
                        CountryID = Guid.Parse("90EBF025-33E6-4AF2-A441-E02D70E331BF")
                    },

                    new Person()
                    {
                        PersonID = Guid.Parse("B2D76D1F-E4BE-412B-ADE8-DA7AFBDDE194"),
                        PersonName = "Dukey",
                        Email = "dtapsfield5@about.com",
                        Address = "CORSO PERTICARI 25/27",
                        DateOfBirth = DateTime.Parse("2017-05-14"),
                        Gender = "Male",
                        ReceiveNewsLetters = true,
                        CountryID = Guid.Parse("41650245-C74D-472C-806D-04DBDAAF1133")
                    }
                  
                });
            }
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
            _persons.Add(person);

            //convert the Person object into PersonResponse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(temp=>ConvertPersonToPersonResponse(temp)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
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
            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
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
            return ConvertPersonToPersonResponse(person);

        }

        public bool DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            //check if person exists
            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);
            if (person == null)
            {
                return false;
            }
            //remove person from persons list
            return _persons.Remove(person);

        }
    }
}