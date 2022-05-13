using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<MovieDomainModel>> GetByIdAsync(Guid id)
        {
            MovieDomainModel movie;

            movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                return NotFound(Messages.MOVIE_DOES_NOT_EXIST);
            }

            return Ok(movie);
        }

        /// <summary>
        /// Gets all current movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAsync()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = await _movieService.GetAllMovies(true);

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]MovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Current = movieModel.Current,
                Rating = movieModel.Rating,
                Title = movieModel.Title,
                Year = movieModel.Year,
                HasOscar = movieModel.HasOscar
            };

            MovieDomainModel createMovie;

            try
            {
                createMovie = await _movieService.AddMovie(domainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (createMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("movies//" + createMovie.Id, createMovie);
        }

        /// <summary>
        /// Updates a movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody]MovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel movieToUpdate;

            movieToUpdate = await _movieService.GetMovieByIdAsync(id);

            if (movieToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            movieToUpdate.Title = movieModel.Title;
            movieToUpdate.Current = movieModel.Current;
            movieToUpdate.Year = movieModel.Year;
            movieToUpdate.Rating = movieModel.Rating;

            MovieDomainModel movieDomainModel;
            try
            {
                movieDomainModel = await _movieService.UpdateMovie(movieToUpdate);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Accepted("movies//" + movieDomainModel.Id, movieDomainModel);

        }

        /// <summary>
        /// Delete a movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [AllowAnonymous]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            MovieDomainModel deletedMovie;
            try
            {
                deletedMovie = await _movieService.DeleteMovie(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (deletedMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("movies//" + deletedMovie.Id, deletedMovie);
        }

        /// <summary>
        /// Activate/Deactivate movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<ActivateMovieModel>> ActivateDeactivateMovie(Guid id)
        {
            ActivateMovieModel changedCurrentMovie = new ActivateMovieModel();
            try
            {
                changedCurrentMovie = await _movieService.ActivateDeactivateMovie(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (changedCurrentMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            if (changedCurrentMovie.IsSuccessful == false)
            {
                return BadRequest(changedCurrentMovie.ErrorMessage);
            }
            return Accepted("movies//" + changedCurrentMovie.Movie.Id, changedCurrentMovie.Movie);
        }

        /// <summary>
        /// Get movies by tags
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMoviesByTags")]
        public async Task<ActionResult<List<MovieDomainModel>>> GetMoviesByTags([FromQuery] List<int> tags)
        {
            var moviesByTags = _movieService.SearchMoviesByTags(tags);
            return Ok(moviesByTags);
        }

        /// <summary>
        /// Get top 10 movies
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("top10Movies")]
        public async Task<ActionResult<List<MovieDomainModel>>> GetTop10Movies()
        {
            List<MovieDomainModel> movieDomainModels;
            movieDomainModels = await _movieService.GetTop10Movies();
            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }
            return Ok(movieDomainModels);
        }

        [HttpGet]
        [Route("top10Movies/{year}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetTop10MoviesByYear(int year)
        {
            var result = await _movieService.GetTopTenMoviesByYear(year);
            if (result == null)
            {
                result = new List<MovieDomainModel>();
            }
            return Ok(result);
        }
    }
}
