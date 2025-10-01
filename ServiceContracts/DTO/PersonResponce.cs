using System;
using Entities;
using ServiceContracts.Enums;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        public string? TaxIdentificationNumber { get; set; }

        /// <summary>
        /// Compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">The PersonResponse Object to compare</param>
        /// <returns>True or false, indicating whether all person details are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;
            return PersonID == person.PersonID && PersonName == person.PersonName && Email == person.Email && DateOfBirth == person.DateOfBirth && Gender == person.Gender && CountryID == person.CountryID && Address == person.Address && ReceiveNewsLetters == person.ReceiveNewsLetters && TaxIdentificationNumber == person.TaxIdentificationNumber;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersonID: {PersonID}, " +
                   $"PersonName: {PersonName}, " +
                   $"Email: {Email}, " +
                   $"DateOfBirth: {DateOfBirth?.ToString("yyyy-MM-dd")}, " +
                   $"Gender: {Gender}, " +
                   $"CountryID: {CountryID}, " +
                   $"Country: {Country}, " +
                   $"Address: {Address}, " +
                   $"ReceiveNewsLetters: {ReceiveNewsLetters}, " +
                   $"Age: {Age}";
        }

       public PersonUpdateRequest ToPersonUpdateRequest()
        {
                  return new PersonUpdateRequest() { PersonID=PersonID,PersonName=PersonName,
                      Gender = (GenderOptions?)Enum.Parse(typeof(GenderOptions), Gender,true),
                      Email = Email,
                      DateOfBirth = DateOfBirth,
                      CountryID = CountryID,
                      Address = Address,
                      ReceiveNewsLetters = ReceiveNewsLetters ,
                      TaxIdentificationNumber = TaxIdentificationNumber,
                  };
        }
    }

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person class into PersonResponse class
        /// </summary>
        /// <param name="person">The Person object to convert</param>
        /// /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            //person => convert => PersonResponse
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Address = person.Address,
                CountryID = person.CountryID,
                Gender = person.Gender,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null    ,
                Country = person.Country?.CountryName, // using navigation property to get country name
            };
        }

        // because of navigation property we dont this anymore because
        // we have country data in our entity
        //private PersonResponse ConvertPersonToPersonResponse(Person person)
        //{
        //    PersonResponse personResponse = person.ToPersonResponse();
        //    //personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
        //    // after adding navigation property in Person entity  we dont need to call countries service
        //    personResponse.Country = person.Country?.CountryName; // using navigation property to get country name
        //    return personResponse;
        //}
    }


}