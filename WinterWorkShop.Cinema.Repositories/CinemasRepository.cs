using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface ICinemasRepository : IRepository<CinemaEntity> { }

    public class CinemasRepository : ICinemasRepository
    {
        private CinemaContext _cinemaContext;

        public CinemasRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public CinemaEntity Delete(object id)
        {
            CinemaEntity existing = _cinemaContext.Cinemas.Find(id);
            var result = _cinemaContext.Cinemas.Remove(existing);

            return result.Entity;
        }

        public async Task<List<CinemaEntity>> GetAll()
        {
            var data = await _cinemaContext.Cinemas.ToListAsync();

            return data;
        }

        public async Task<CinemaEntity> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Cinemas.FindAsync(id);

            return data;
        }

        public CinemaEntity Insert(CinemaEntity obj)
        {
            return _cinemaContext.Cinemas.Add(obj).Entity;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public CinemaEntity Update(CinemaEntity obj)
        {
            var updatedEntry = _cinemaContext.Cinemas.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry.Entity;
        }
    }
}
