using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly IMovieTagsRepository _movieTagsRepository;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository, IMovieTagsRepository movieTagsRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
            _movieTagsRepository = movieTagsRepository;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetAllMovies(bool? isCurrent)
        {
            var data = _moviesRepository.GetCurrentMovies();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    HasOscar = item.HasOscar
                };
                result.Add(model);
            }

            return result;

        }

        public async Task<MovieDomainModel> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year,
                HasOscar = data.HasOscar
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie)
        {
            Movie movieToCreate = new Movie()
            {
                Title = newMovie.Title,
                Current = newMovie.Current,
                Year = newMovie.Year,
                Rating = newMovie.Rating,
                HasOscar = newMovie.HasOscar
            };

            var data = _moviesRepository.Insert(movieToCreate);
            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> UpdateMovie(MovieDomainModel updateMovie) {

            Movie movie = new Movie()
            {
                Id = updateMovie.Id,
                Title = updateMovie.Title,
                Current = updateMovie.Current,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating,
                HasOscar = updateMovie.HasOscar
            };

            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return null;
            }
            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating,
                HasOscar = data.HasOscar

            };

            return domainModel;
        }

        public async Task<MovieDomainModel> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating,
                HasOscar = data.HasOscar

            };

            return domainModel;
        }
        public async Task<ActivateMovieModel> ActivateDeactivateMovie(Guid id)
        {
            var movie = await _moviesRepository.GetByIdAsync(id);
            var checkProjections = _projectionsRepository.GetByMovieId(id);

            foreach (var item in checkProjections)
            {
                //check if movie has projections in future
                if (item.DateTime > DateTime.Now)
                {
                    return new ActivateMovieModel
                    {
                        IsSuccessful = false,
                        ErrorMessage = Messages.MOVIE_HAS_FUTURE_PROJECTIONS
                    };
                }
            }
            if (movie == null)
            {
                return new ActivateMovieModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST
                };
            }
            movie.Current = !movie.Current;

            _moviesRepository.Update(movie);
            _moviesRepository.Save();
            ActivateMovieModel movieDomain = new ActivateMovieModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Movie = new MovieDomainModel
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Current = movie.Current,
                    Rating = movie.Rating,
                    Year = movie.Year,
                    HasOscar = movie.HasOscar
                }
            };
            return movieDomain;
        }
        public ActionResult<List<MovieDomainModel>> SearchMoviesByTags(List<int> tagIds)
        {
            //initialization
            List<Guid> MovieGuids = new List<Guid>();
            List<MovieTag> MovieTags = new List<MovieTag>();
            //finding movies with first tag
            for (int i = 0; i < tagIds.Count; i++)
            {
                List<MovieTag> moviesTagsWithFirstTag = _movieTagsRepository.GetAll().Where(x => x.TagId == tagIds[i]).ToList();
                MovieTags.AddRange(moviesTagsWithFirstTag);
                foreach (var movie in moviesTagsWithFirstTag)
                {
                    MovieGuids.Add(movie.MovieId);
                }
            }
            List<Guid> resultMovieIds = new List<Guid>();

            for (int i = 0; i < MovieGuids.Count; i++)
            {
                bool contains = false;
                //finding movies that contains tags from input
                for (int j = 0; j < tagIds.Count; j++)
                {
                    if (MovieTags.FirstOrDefault(x => x.TagId == tagIds[j] && x.MovieId == MovieGuids[i]) != null)
                    {
                        contains = true;
                    }
                    else
                    {
                        contains = false;
                        break;
                    }
                }
                if (contains == true)
                {
                    resultMovieIds.Add(MovieGuids[i]);
                }
            }
            List<Movie> Movies = new List<Movie>();
            List<MovieDomainModel> resultMovies = new List<MovieDomainModel>();

            foreach (Guid movieId in resultMovieIds)
            {
                var movie = _moviesRepository.GetByIdAsync(movieId).Result;
                if (movie != null && Movies.FirstOrDefault(x => x.Id == movieId) == null)
                {
                    Movies.Add(movie);
                }
            }
            foreach (Movie movie in Movies)
            {
                var model = new MovieDomainModel()
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Current = movie.Current,
                    Rating = movie.Rating,
                    Year = movie.Year,
                    HasOscar = movie.HasOscar,
                };
                resultMovies.Add(model);
            }
            if (resultMovies.Count == 0)
            {
                var responseModel = new ErrorResponseModel()
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return new BadRequestObjectResult(responseModel);
            }
            return resultMovies;
        }
        public async Task<List<MovieDomainModel>> GetTop10Movies()
        {
            var movies = await _moviesRepository.GetTopMovies();
            if (movies == null)
            {
                return null;
            }
            var top10Movies = movies.Take(10).ToList();
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in top10Movies)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating,
                    HasOscar = item.HasOscar,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);
            }
            return result;
        }
        public async Task<IEnumerable<MovieDomainModel>> GetTopTenMoviesByYear(int year)
        {
            var data = await _moviesRepository.GetTopTenMoviesByYear(year);
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                result.Add(new MovieDomainModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Current = item.Current,
                    HasOscar = item.HasOscar,
                    Rating = item.Rating,
                    Year = item.Year,
                });
            }
            return result;
        }
    }
}