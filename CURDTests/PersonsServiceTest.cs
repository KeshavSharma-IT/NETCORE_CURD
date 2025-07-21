using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;


        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture=new Fixture();
            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            //Craete mock for DbContext
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
              new DbContextOptionsBuilder<ApplicationDbContext>().Options
             );

            //Access Mock DbContext object
            ApplicationDbContext dbContext = dbContextMock.Object;

            //Create mocks for DbSets'
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            //Create services based on mocked DbContext object
            _countriesService = new CountriesService(dbContext);

            _personService = new PersonsService(dbContext, _countriesService);

            _testOutputHelper = testOutputHelper;   
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            //await   Assert.ThrowsAsync<ArgumentNullException>(async() =>
            //{
            //    await _personService.AddPerson(personAddRequest);
            //});
            Func<Task> action = async () =>
            {
                await _personService.AddPerson(personAddRequest);
            };
             await action.Should().ThrowAsync<ArgumentNullException>();

        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            //Act
            Func<Task> action= async() =>
            {
              await  _personService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "Person name...", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };
            //PersonAddRequest? personAddRequest = _fixture.Create<PersonAddRequest>();    // generating data automatically

            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test@gmail.com").Create();

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

            List<PersonResponse> persons_list = await _personService.GetAllPersons();

            //Assert
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);


            //Assert.Contains(person_response_from_add, persons_list);
            persons_list.Should().Contain(person_response_from_add);
        }



        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get =await _personService.GetPersonByPersonID(personID);

            //Assert
            //Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arange
            //CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            //CountryResponse country_response =await _countriesService.AddCountry(country_request);

            //PersonAddRequest person_request = new PersonAddRequest() { PersonName = "person name...", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };

            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"test@gmail.com").Create();

            PersonResponse person_response_from_add =await _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get =await  _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(person_response_from_add);
        }

        #endregion

        #region      GetAllPersons

        //GetAllPersons should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons_list =await _personService.GetAllPersons();
            //Assert
            //Assert.Empty(persons_list);
            persons_list.Should().BeEmpty();
        }

        // first we add few person and then we call GetAllPersons, it should return the list of persons

        [Fact]
        public async Task GetAllPerson_AddFewPerson()
        {
            // Arrange

            //CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "India" };
            //CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };

            //PersonAddRequest personAddRequest1 = new PersonAddRequest()
            //{
            //    PersonName = "keshv",
            //    Email = "keshav@gmail.com",
            //    Gender = GenderOptions.Male,
            //    Address = "Ghaziabad",
            //    CountryID = countryResponse1
            //.CountryID,
            //    DateOfBirth = DateTime.Parse("01-08-1997"),
            //    ReceiveNewsLetters = true
            //};

            //PersonAddRequest personAddRequest2 = new PersonAddRequest()
            //{
            //    PersonName = "Rohan",
            //    Email = "Rohan@gmail.com",
            //    Gender = GenderOptions.Male,
            //    Address = "Hapur",
            //    CountryID = countryResponse2
            //.CountryID,
            //    DateOfBirth = DateTime.Parse("01-04-1994"),
            //    ReceiveNewsLetters = true
            //};
            //PersonAddRequest personAddRequest3 = new PersonAddRequest()
            //{
            //    PersonName = "Dhiru",
            //    Email = "Dhiru@gmail.com",
            //    Gender = GenderOptions.Male,
            //    Address = "Ghaziabad",
            //    CountryID = countryResponse2.CountryID,
            //    DateOfBirth = DateTime.Parse("20-04-1999"),
            //    ReceiveNewsLetters = true
            //};
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test@gmail.com").Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test1@gmail.com").Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test2@gmail.com").Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce =await _personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //Act
            List<PersonResponse> persons_list_get = await _personService.GetAllPersons();


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_get)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert

            //foreach (var personResponse_add in persons_list_add)
            //{
            //    Assert.Contains(personResponse_add, persons_list_get);
            //}
            persons_list_get.Should().BeEquivalentTo(persons_list_add);


        }
        #endregion

        #region GetFilteredPersons

        //if the serach us empty an search by "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange

            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test@gmail.com").Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test1@gmail.com").Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test2@gmail.com").Create();


            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce =await _personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //Act
            List<PersonResponse> persons_list_Search =await _personService.GetFilterPersons(nameof(Person.PersonName), "");


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_Search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert

            //foreach (var personResponse_add in persons_list_add)
            //{
            //    Assert.Contains(personResponse_add, persons_list_Search);
            //}

            persons_list_Search.Should().BeEquivalentTo(persons_list_add);


        }

        //first we will add few peson and the we will search based on person name
        //with some search string, it should return the list of persons that matches the search string
        //so it shoukd return matching results
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange

            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                 .With(temp => temp.Email, "test@gmail.com")
                 .With(temp => temp.PersonName, "Keshav")
                 .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Rohan")
                .With(temp => temp.Email, "test1@gmail.com").Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, "Mohan").With(temp => temp.Email, "test2@gmail.com").Create();


            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce =await _personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //Act
            List<PersonResponse> persons_list_Search =await _personService.GetFilterPersons(nameof(Person.PersonName), "ke");


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_Search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert

            //foreach (var personResponse_add in persons_list_add)
            //{
            //    if (personResponse_add.PersonName != null)
            //    {
            //        if (personResponse_add.PersonName?.Contains("ke", StringComparison.OrdinalIgnoreCase) == true)
            //        {
            //            // If the person name contains "ke", it should be in the search results
            //            Assert.Contains(personResponse_add, persons_list_Search);
            //        }

            //    }

            //}               

            persons_list_Search.Should().OnlyContain(temp => temp.PersonName.Contains("ke", StringComparison.OrdinalIgnoreCase));

        }

        #endregion


        #region SotedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Smith")
             .With(temp => temp.Email, "someone_1@example.com")
             .With(temp => temp.CountryID, country_response_1.CountryID)
             .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Mary")
             .With(temp => temp.Email, "someone_2@example.com")
             .With(temp => temp.CountryID, country_response_1.CountryID)
             .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone_3@example.com")
             .With(temp => temp.CountryID, country_response_2.CountryID)
             .Create();


            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            //{
            //    Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            //}
            //persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add); // another way down

            persons_list_from_sort.Should().BeInDescendingOrder(temp=>temp.PersonName);


        }
        #endregion


        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            Func<Task> action= async () =>
            {
                //Act
                await _personService.UpdatePerson(person_update_request);
            };
           await action.Should().ThrowAsync<ArgumentNullException>();
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
             .Create();

            //Assert
            Func<Task> action= async () =>
            {
                //Act
                await _personService.UpdatePerson(person_update_request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When PersonName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.CountryID, country_response.CountryID)
             .Create();


            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;


            //Assert
            Func<Task> action= async () =>
            {
                //Act
                await _personService.UpdatePerson(person_update_request);
            };
            await action.Should().ThrowAsync<ArgumentException>();

        }


        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.CountryID, country_response.CountryID)
             .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);

            person_response_from_update.Should().BeEquivalentTo(person_response_from_get);

        }

        #endregion


        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.CountryID, country_response.CountryID)
             .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);


            //Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();

        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}