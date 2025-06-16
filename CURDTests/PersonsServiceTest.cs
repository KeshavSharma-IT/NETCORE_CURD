using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using System.Linq;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;


        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personService = new PersonsService(false);
            _countriesService = new CountriesService(false);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personService.AddPerson(personAddRequest);
            });
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personService.AddPerson(personAddRequest);
            });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            // Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Test Country" };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person name...",
                Email = "person@example.com",
                Address = "sample address",
                CountryID = countryResponse.CountryID, // Use the actual CountryID
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            // Act
            PersonResponse person_response_from_add = _personService.AddPerson(personAddRequest);
            List<PersonResponse> persons_list = _personService.GetAllPersons();

            // Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);
        }


        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(person_response_from_get);
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse country_response = _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = new PersonAddRequest() { PersonName = "person name...", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };

            PersonResponse person_response_from_add = _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion

        #region      GetAllPersons

        //GetAllPersons should return an empty list by default
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons_list = _personService.GetAllPersons();
            //Assert
            Assert.Empty(persons_list);
        }

        // first we add few person and then we call GetAllPersons, it should return the list of persons

        [Fact]
        public void GetAllPerson_AddFewPerson()
        {
            // Arrange

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "keshv",
                Email = "keshav@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse1
            .CountryID,
                DateOfBirth = DateTime.Parse("01-08-1997"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Rohan",
                Email = "Rohan@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Hapur",
                CountryID = countryResponse2
            .CountryID,
                DateOfBirth = DateTime.Parse("01-04-1994"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Dhiru",
                Email = "Dhiru@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("20-04-1999"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce =_personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //Act
            List<PersonResponse> persons_list_get = _personService.GetAllPersons();


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_get)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert

            foreach (var personResponse_add in persons_list_add)
            {
                Assert.Contains(personResponse_add, persons_list_get);
            }



        }
        #endregion

        #region GetFilteredPersons

        //if the serach us empty an search by "PersonName", it should return all persons
        [Fact]
        public void GetFilteredPersons_EmptySearchText()
        {
            // Arrange

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "keshv",
                Email = "keshav@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse1
            .CountryID,
                DateOfBirth = DateTime.Parse("01-08-1997"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Rohan",
                Email = "Rohan@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Hapur",
                CountryID = countryResponse2
            .CountryID,
                DateOfBirth = DateTime.Parse("01-04-1994"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Dhiru",
                Email = "Dhiru@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("20-04-1999"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce = _personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //Act
            List<PersonResponse> persons_list_Search = _personService.GetFilterPersons(nameof(Person.PersonName), "");


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_Search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert

            foreach (var personResponse_add in persons_list_add)
            {
                Assert.Contains(personResponse_add, persons_list_Search);
            }



        }

        //first we will add few peson and the we will search based on person name
        //with some search string, it should return the list of persons that matches the search string
        //so it shoukd return matching results
        [Fact]
        public void GetFilteredPersons_SearchByPersonName()
        {
            // Arrange

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "keshv",
                Email = "keshav@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse1
            .CountryID,
                DateOfBirth = DateTime.Parse("01-08-1997"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Rohan",
                Email = "Rohan@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Hapur",
                CountryID = countryResponse2
            .CountryID,
                DateOfBirth = DateTime.Parse("01-04-1994"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Dhiru",
                Email = "Dhiru@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("20-04-1999"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce = _personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }


            //Act
            List<PersonResponse> persons_list_Search = _personService.GetFilterPersons(nameof(Person.PersonName), "ke");


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_Search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert

            foreach (var personResponse_add in persons_list_add)
            {
                if (personResponse_add.PersonName != null)
                {
                    if (personResponse_add.PersonName?.Contains("ke", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // If the person name contains "ke", it should be in the search results
                        Assert.Contains(personResponse_add, persons_list_Search);
                    }

                }

            }



        }

        #endregion


        #region SotedPersons

        [Fact]
        public void GetSortedPersons()
        {
            // Arrange

            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "keshv",
                Email = "keshav@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse1
            .CountryID,
                DateOfBirth = DateTime.Parse("01-08-1997"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Rohan",
                Email = "Rohan@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Hapur",
                CountryID = countryResponse2
            .CountryID,
                DateOfBirth = DateTime.Parse("01-04-1994"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Dhiru",
                Email = "Dhiru@gmail.com",
                Gender = GenderOptions.Male,
                Address = "Ghaziabad",
                CountryID = countryResponse2.CountryID,
                DateOfBirth = DateTime.Parse("20-04-1999"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                personAddRequest1, personAddRequest2, personAddRequest3
            };

            List<PersonResponse> persons_list_add = new List<PersonResponse>();

            foreach (var personAddRequest in personAddRequests)
            {
                PersonResponse person_responce = _personService.AddPerson(personAddRequest);
                persons_list_add.Add(person_responce);
            }
            //Print
            _testOutputHelper.WriteLine("Persons added: Expected value");
            foreach (var personResponse in persons_list_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> allperson = _personService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_Sort = _personService.GetSortedPersons(allperson,nameof(Person.PersonName),SortOrderOptions.DESC );


            //Print
            _testOutputHelper.WriteLine("Persons added: Actual value");
            foreach (var personResponse in persons_list_Sort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            persons_list_add = persons_list_add.OrderByDescending(temp => temp.PersonName).ToList();
            //Assert

            for(int i = 0; i < persons_list_add.Count; i++)
            {
                Assert.Equal(persons_list_add[i].PersonName, persons_list_Sort[i].PersonName);
            }   



        }

        #endregion


        #region UpdatePerson
        // when we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personService.UpdatePerson(personUpdateRequest);
            });
        }

        // when we supply invalid   PersonID, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            //Assert
            Assert.Throws<ArgumentException>(() => {
                //Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        // when person name is null it should throw ArgumentException
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID,Email="keshav@gmail.com" ,Gender=GenderOptions.Male};
            PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;


            //Assert
            Assert.Throws<ArgumentException>(() => {
                //Act
                _personService.UpdatePerson(person_update_request);
            });

        }


        // when we supply proper person details, it should update the person details and return the updated person details

        // first add person and update it and retuen updated obj
        [Fact]
        public void UpdatePerson_PersonFullDetailsUpdation()
        {
            CountryAddRequest country_add = new CountryAddRequest() { CountryName = "Test Country" };
            CountryResponse countryResponse = _countriesService.AddCountry(country_add);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "keshav", CountryID = countryResponse.CountryID,Email="keshav@gmail.com",Gender=GenderOptions.Male,Address="ghaziabad",ReceiveNewsLetters=true,DateOfBirth=DateTime.Parse("1997-08-01"), };
            PersonResponse personResponse_add = _personService.AddPerson(personAddRequest);

            //Arrange
            PersonUpdateRequest? personUpdateRequest = personResponse_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Rohan";
            personUpdateRequest.Email = "Rohan@gmail.com";
            //Act

            PersonResponse personResponse_update = _personService.UpdatePerson(personUpdateRequest);

           PersonResponse personResponse_from_get= _personService.GetPersonByPersonID(personResponse_add.PersonID);

            //Assert

            Assert.Equal(personResponse_update, personResponse_from_get);
        }


        #endregion

        #region DeletePerson

        // when we supply valid as PersonID, it should return true

        [Fact]
        public void DeletePerson_validPersonID()
        {
            CountryAddRequest country_add = new CountryAddRequest() { CountryName = "Test Country" };
            CountryResponse countryResponse = _countriesService.AddCountry(country_add);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "keshav", CountryID = countryResponse.CountryID, Email = "keshav@gmail.com", Gender = GenderOptions.Male, Address = "ghaziabad", ReceiveNewsLetters = true, DateOfBirth = DateTime.Parse("1997-08-01"), };
            PersonResponse personResponse_add = _personService.AddPerson(personAddRequest);

            //ACT
            bool isDeleted = _personService.DeletePerson(personResponse_add.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        // invalid person id, it should return false
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {   
            //ACT
            bool isDeleted = _personService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}