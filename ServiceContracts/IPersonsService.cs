using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Perosn entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Addds a new person into the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns the same person details, along with newly generated PersonID</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);


        /// <summary>
        /// Returns all persons
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        List<PersonResponse> GetAllPersons();

        /// <summary>
        /// Returns the person object based on the given person id
        /// </summary>
        /// <param name="personID">Person id to search</param>
        /// <returns>Returns matching person object</returns>
        PersonResponse? GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns a list of persons based on the given filter criteria
        /// </summary>
        /// <param name="serachby"></param>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        List<PersonResponse> GetFilterPersons(string serachby, string? SearchString);
        /// <summary>
        /// Sorts the list of persons based on the given sort criteria
        /// </summary>
        /// <param name="allperson"></param>
        /// <param name="sortby"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allperson,string sortby,SortOrderOptions sortOrder);

        /// <summary>
        /// Updates the person details based on the given person update request
        /// </summary>
        /// <param name="personUpdateRequest"></param>
        /// <returns>it return person object</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        /// <summary>
        ///            Deletes the person based on the given person id
        /// </summary>
        /// <param name="personID"></param>
        /// <returns>it return true and false</returns>
        bool DeletePerson(Guid? personID);
    }
}