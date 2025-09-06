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
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Serilog;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly Mock<IPersonsRepository> _mockpersonsRepository; 
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;


        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture=new Fixture();
           _mockpersonsRepository=new Mock<IPersonsRepository>();
            _personsRepository= _mockpersonsRepository.Object;
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            var LoggerMock = new Mock<ILogger<PersonsService>>();

            _personService = new PersonsService(_personsRepository, LoggerMock.Object, diagnosticContextMock.Object );

            _testOutputHelper = testOutputHelper;   
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
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
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentNullException()
        {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            Person person=personAddRequest.ToPerson();

            _mockpersonsRepository.Setup(temp=>temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);


            //Act
            Func<Task> action= async() =>
            {
              await  _personService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        
        [Fact]
        public async Task AddPerson_ProperPersonDetails_ToBeSuccessful()
        {
            //Arrange
            //PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "Person name...", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };
            //PersonAddRequest? personAddRequest = _fixture.Create<PersonAddRequest>();    // generating data automatically

            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test@gmail.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponse_expected=person.ToPersonResponse();

            // if we supply any argument value to the AddPerson method, it should return the same   value   
            _mockpersonsRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(new Person());

            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);
            personResponse_expected.PersonID = person_response_from_add.PersonID;

            //List<PersonResponse> persons_list = await _personService.GetAllPersons();

            //Assert
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);

            person_response_from_add.Should().Be(personResponse_expected);


            //Assert.Contains(person_response_from_add, persons_list);
            //persons_list.Should().Contain(person_response_from_add);
        }

        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
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
        public async Task GetPersonByPersonID_WithPersonID_ToBeSucessfull()
        {
            //Arange
            //PersonAddRequest person_request = _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"test@gmail.com").Create();
            Person person = _fixture.Build<Person>().With(temp=>temp.Email,"test@gmail.com").With(temp => temp.Country, null as Country).Create();
            PersonResponse personResponse_expected= person.ToPersonResponse();
            //mocking service method
            _mockpersonsRepository.Setup(TEMP=>TEMP.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse? person_response_from_get =await  _personService.GetPersonByPersonID(person.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            person_response_from_get.Should().Be(personResponse_expected);
        }

        #endregion

        #region      GetAllPersons

        //GetAllPersons should return an empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            var person = new List<Person>();
            _mockpersonsRepository.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(person);

            //Act
            List<PersonResponse> persons_list =await _personService.GetAllPersons();
            //Assert
            //Assert.Empty(persons_list);
            persons_list.Should().BeEmpty();
        }

        // first we add few person and then we call GetAllPersons, it should return the list of persons

        [Fact]
        public async Task GetAllPerson_WithFewPersons_ToBeSuccessful()
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
            //CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            //CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
            //CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest);
            //CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            //PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test@gmail.com").Create();

            //PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test1@gmail.com").Create();

            //PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "test2@gmail.com").Create();

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "test@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test1@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test2@gmail.com").With(temp => temp.Country, null as Country).Create(),

            };

           

            List<PersonResponse> persons_list_add = persons.Select(temp=>temp.ToPersonResponse()).ToList();

            //foreach (var personAddRequest in personAddRequests)
            //{
            //    PersonResponse person_responce =await _personService.AddPerson(personAddRequest);
            //    persons_list_add.Add(person_responce);
            //}
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            _mockpersonsRepository.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(persons);
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
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            // Arrange

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "test@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test1@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test2@gmail.com").With(temp => temp.Country, null as Country).Create(),

            };

            List<PersonResponse> persons_list_add = persons.Select(temp => temp.ToPersonResponse()).ToList();

           
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //mocking
            _mockpersonsRepository.Setup(temp=>temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_Search =await _personService.GetFilteredPersons(nameof(Person.PersonName), "");


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
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            // Arrange

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "test@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test1@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test2@gmail.com").With(temp => temp.Country, null as Country).Create(),

            };

            List<PersonResponse> persons_list_add = persons.Select(temp => temp.ToPersonResponse()).ToList();


            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //mocking
            _mockpersonsRepository.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_Search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ke");


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_Search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert                                

            persons_list_Search.Should().BeEquivalentTo(persons_list_add);              

        }

        #endregion


        #region SotedPersons

        [Fact]
        public async Task GetSortedPersons_ToBeSuccessfull()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp => temp.Email, "test@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test1@gmail.com").With(temp => temp.Country, null as Country).Create(),
                _fixture.Build<Person>().With(temp => temp.Email, "test2@gmail.com").With(temp => temp.Country, null as Country).Create(),

            };

            List<PersonResponse> persons_list_add = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _mockpersonsRepository.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in persons_list_add)
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
            persons_list_add = persons_list_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert             
            persons_list_from_sort.Should().BeInDescendingOrder(temp=>temp.PersonName);


        }
        #endregion


        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
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
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
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
            Person person=_fixture.Build<Person>().With(temp=>temp.PersonName,null as string).With(temp=>temp.Email,"someone@gmail.com").With(temp=>temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();
           
            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();             

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

            Person person = _fixture.Build<Person>()
             //.With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Gender, "Male")
             .With(temp => temp.Country, null as Country)
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            
            _mockpersonsRepository.Setup(temp=>temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _mockpersonsRepository.Setup(temp=>temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            

            //Assert
            //Assert.Equal(person_response_from_get, person_response_from_update);

            person_response_from_update.Should().BeEquivalentTo(person_response_from_add);

        }

        #endregion


        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            

            Person person_add_request = _fixture.Build<Person>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .Create();

            PersonResponse person_response_from_add = person_add_request.ToPersonResponse();

            _mockpersonsRepository.Setup(temp=>temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);
            _mockpersonsRepository.Setup(temp=>temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person_add_request);

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