using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //private field
        //private readonly List<Person> _persons;
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countriesService;

        
        //constructor
        public PersonsService(ApplicationDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
           
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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
            await _db.SaveChangesAsync();

            //call stored procedure to insert person
            //_db.sp_InsertPerson(person);

            //convert the Person object into PersonResponse type
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            //return _db.Persons.ToList().
            //    Select(temp => ConvertPersonToPersonResponse(temp)).ToList();

            //get all persons from persons list inculding country details\
            //-- Inculde is used to eager load the related data
            var persons = await _db.Persons.Include("Country").ToListAsync();
           return  persons.Select(temp => temp.ToPersonResponse()).ToList();

            // get with stored procedure
            //return _db.sp_GetAllPersons()
            //    .Select(temp => ConvertPersonToPersonResponse(temp))
            //    .ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            //Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personID);
            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilterPersons(string searchby, string? SearchString)
        {
            List<PersonResponse> persons = await GetAllPersons();
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

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allperson, string sortby, SortOrderOptions sortOrder)
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

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
           
            if (personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(personUpdateRequest));
            }
            //Model validation
            ValidationHelper.ModelValidation(personUpdateRequest);
            //check if person exists
            Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);
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
            await _db.SaveChangesAsync();
            return person.ToPersonResponse();

        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }
            //check if person exists
            Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personID);
            if (person == null)
            {
                return false;
            }
            //remove person from persons list
            _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID) ) ;
           await _db.SaveChangesAsync();
            return true;

        }

        //public async Task<MemoryStream> GetPersonCSV()
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    StreamWriter streamWriter = new StreamWriter(memoryStream);
        //    CsvWriter csvwriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture,leaveOpen:true);

        //    csvwriter.WriteHeader<PersonResponse>();
        //    csvwriter.NextRecord();
        //    List<PersonResponse> persons  =_db.Persons.Include("Country").Select(temp=>temp.ToPersonResponse()).ToList();
        //    await csvwriter.WriteRecordsAsync(persons);
        //    memoryStream.Position = 0;
        //    return memoryStream;


        //}

        public Task<MemoryStream> GetPersonCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            CsvWriter csvwriter = new CsvWriter(streamWriter,csvConfiguration);

            csvwriter.WriteField(nameof(PersonResponse.PersonName));
            csvwriter.WriteField(nameof(PersonResponse.Email));
            csvwriter.WriteField(nameof(PersonResponse.Age));
            csvwriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvwriter.WriteField(nameof(PersonResponse.Gender));
            csvwriter.WriteField(nameof(PersonResponse.Country));
            csvwriter.WriteField(nameof(PersonResponse.Address));
            csvwriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
            csvwriter.NextRecord();
            List<PersonResponse> persons = _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToList();

            foreach(PersonResponse person in persons)
            {
                csvwriter.WriteField(person.PersonName);
                csvwriter.WriteField(person.Email);
                csvwriter.WriteField(person.Age);
                csvwriter.WriteField(person.Gender);
                csvwriter.WriteField(person.Address);
                if(person.DateOfBirth.HasValue)
                    csvwriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-mm-dd"));
                else
                    csvwriter.WriteField("");

                csvwriter.WriteField(person.Country);
                csvwriter.WriteField(person.ReceiveNewsLetters);
                csvwriter.NextRecord();
                csvwriter.Flush();
                    

            }                
            memoryStream.Position = 0;
            return Task.FromResult(memoryStream);


        }

        public async Task<MemoryStream> GetPersonExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream)) 
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonSheet");
                worksheet.Cells["A1"].Value = "Petrson Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date of Birth";
                worksheet.Cells["D1"].Value = "Age";
                worksheet.Cells["E1"].Value = "Gender";
                worksheet.Cells["F1"].Value = "Country";
                worksheet.Cells["G1"].Value = "Addres";
                worksheet.Cells["H1"].Value = "Receive News Letters";

                int row = 2;

                List<PersonResponse> personResponses=_db.Persons.Include("Country").Select(temp=>temp.ToPersonResponse()).ToList();

                 foreach(PersonResponse person in personResponses)
                {
                    worksheet.Cells[row,1].Value=person.PersonName;
                    worksheet.Cells[row,2].Value = person.Email;
                    if (person.DateOfBirth.HasValue)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-DD");
                    else 
                        worksheet.Cells[row, 3].Value ="";

                    worksheet.Cells[row, 3].Value =person.DateOfBirth;
                    worksheet.Cells[row, 4].Value =person.Age;
                    worksheet.Cells[row, 5].Value =person.Gender;
                    worksheet.Cells[row, 6].Value =person.Country;
                    worksheet.Cells[row, 7].Value =person.Address;
                    worksheet.Cells[row, 8].Value =person.ReceiveNewsLetters;
                    row++;
                }
                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();


            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}