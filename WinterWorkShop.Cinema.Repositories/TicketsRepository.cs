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
    public interface ITicketsRepository : IRepository<Ticket> { }

    public class TicketRepository : ITicketsRepository
    {
        private CinemaContext _cinemaContext;
        public TicketRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }
        public Ticket Delete(object id)
        {
            Ticket existing = _cinemaContext.Tickets.Find(id);
            var result = _cinemaContext.Tickets.Remove(existing);
            return result.Entity;
        }

        public async Task<List<Ticket>> GetAll()
        {
            var data = await _cinemaContext.Tickets.ToListAsync();

            return data;
        }

        public async Task<Ticket> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Tickets.FindAsync(id);

            return data;
        }

        public Ticket Insert(Ticket obj)
        {
            var data = _cinemaContext.Tickets.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Ticket Update(Ticket obj)
        {
            var updatedEntry = _cinemaContext.Tickets.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
    }
}
