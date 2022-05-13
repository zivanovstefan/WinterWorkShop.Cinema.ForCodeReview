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
    public interface ISeatTicketRepository : IRepository<SeatTicket> { }
    public class SeatTicketRepository : ISeatTicketRepository
    {
        private readonly CinemaContext _cinemaContext;

        public SeatTicketRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }
        public SeatTicket Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SeatTicket>> GetAll()
        {
            var data = await _cinemaContext.SeatTickets.Include(x => x.Seat).Include(x => x.Ticket).ToListAsync();
            return data;
        }

        public Task<SeatTicket> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public SeatTicket Insert(SeatTicket obj)
        {
            var data = _cinemaContext.SeatTickets.Add(obj).Entity;
            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public SeatTicket Update(SeatTicket obj)
        {
            throw new NotImplementedException();
        }
    }
}
