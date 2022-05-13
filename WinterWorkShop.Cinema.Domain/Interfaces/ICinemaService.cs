using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ICinemaService
    {
        Task<IEnumerable<CinemaDomainModel>> GetAllAsync();
        Task<CinemaDomainModel> CreateCinema(CinemaDomainModel newCinema);
        Task<CinemaDomainModel> DeleteCinema(int cinemaId);
    }
}
