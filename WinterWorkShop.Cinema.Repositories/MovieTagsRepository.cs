using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMovieTagsRepository
    {
        List<MovieTag> GetAll();
    }
    public class MovieTagsRepository : IMovieTagsRepository
    {
        private readonly CinemaContext _cinemaContext;

        public MovieTagsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public MovieTag Delete(object id)
        {
            throw new NotImplementedException();
        }

        public List<MovieTag> GetAll()
        {
            var data = _cinemaContext.MovieTags.ToList();
            return data;
        }

        public Task<MovieTag> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public MovieTag Insert(MovieTag obj)
        {
            var data = _cinemaContext.MovieTags.Add(obj).Entity;
            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public MovieTag Update(MovieTag obj)
        {
            throw new NotImplementedException();
        }
    }
}
