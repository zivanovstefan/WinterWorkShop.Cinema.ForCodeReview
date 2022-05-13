using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// Get all movies by current parameter
        /// </summary>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        Task<IEnumerable<MovieDomainModel>> GetAllMovies(bool? isCurrent);

        /// <summary>
        /// Get a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MovieDomainModel> GetMovieByIdAsync(Guid id);

        /// <summary>
        /// Adds new movie to DB
        /// </summary>
        /// <param name="newMovie"></param>
        /// <returns></returns>
        Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie);

        /// <summary>
        /// Update a movie to DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MovieDomainModel> UpdateMovie(MovieDomainModel updateMovie);

        /// <summary>
        /// Delete a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MovieDomainModel> DeleteMovie(Guid id);

        /// <summary>
        /// Activate/Deactivate movie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActivateMovieModel> ActivateDeactivateMovie(Guid id);

        /// <summary>
        /// Get movies by tags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        ActionResult<List<MovieDomainModel>> SearchMoviesByTags(List<int> tags);

        /// <summary>
        /// Get top 10 movies
        /// </summary>
        /// <returns></returns>
        Task<List<MovieDomainModel>> GetTop10Movies();

        /// <summary>
        /// Get top 10 movies by year
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieDomainModel>> GetTopTenMoviesByYear(int year);
    }
}
