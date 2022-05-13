using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMoviesRepository : IRepository<Movie> 
    {
        IEnumerable<Movie> GetCurrentMovies();
        Task<IEnumerable<Movie>> GetTopMovies();
        Task<IEnumerable<Movie>> GetTopTenMoviesByYear(int year);
    }

    public class MoviesRepository : IMoviesRepository
    {
        private CinemaContext _cinemaContext;

        public MoviesRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Movie Delete(object id)
        {
            Movie existing = _cinemaContext.Movies.Find(id);

            if (existing == null)
            {
                return null;
            }

            var result = _cinemaContext.Movies.Remove(existing);

            return result.Entity;
        }

        public async Task<List<Movie>> GetAll()
        {
            return await _cinemaContext.Movies.ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Movies.FindAsync(id);

            return data;
        }

        public IEnumerable<Movie> GetCurrentMovies()
        {
            var data = _cinemaContext.Movies
                .Where(x => x.Current);            

            return data;
        }

        public Movie Insert(Movie obj)
        {
            var data = _cinemaContext.Movies.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Movie Update(Movie obj)
        {
            var updatedEntry = _cinemaContext.Movies.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
        public async Task<IEnumerable<Movie>> GetTopMovies()
        {
            var result = _cinemaContext.Movies.OrderByDescending(x => x.Rating);

            return result;
        }
        public async Task<IEnumerable<Movie>> GetTopTenMoviesByYear(int year)
        {
            var list = (from t in _cinemaContext.Movies
                        where t.Year == year
                        orderby t.Rating descending, t.HasOscar descending
                        select t).Take(10).ToList();
            return list;
        }
    }
}
