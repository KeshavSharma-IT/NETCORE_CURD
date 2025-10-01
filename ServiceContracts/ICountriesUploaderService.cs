using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity
    /// </summary>
    public interface ICountriesUploaderService
    {
        

        /// <summary>
        /// It will return no of countries inserted
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        Task<int> UplodeCountriesFromExcelFile(IFormFile formFile);
    }
}