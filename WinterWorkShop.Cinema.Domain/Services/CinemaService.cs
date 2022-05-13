using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemasRepository _cinemasRepository;

        public CinemaService(ICinemasRepository cinemasRepository)
        {
            _cinemasRepository = cinemasRepository;
        }

        public async Task<IEnumerable<CinemaDomainModel>> GetAllAsync()
        {
            var data = await _cinemasRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<CinemaDomainModel> result = new List<CinemaDomainModel>();
            CinemaDomainModel model;
            foreach (var item in data)
            {
                model = new CinemaDomainModel
                {
                    Id = item.Id,
                    Name = item.Name
                };
                result.Add(model);
            }
            return result;
        }
        public async Task<CinemaDomainModel> CreateCinema(CinemaDomainModel newCinema)
        {
            CinemaEntity newCinemaModel = new CinemaEntity()
            {
                Name = newCinema.Name
            };
            
            var data = _cinemasRepository.Insert(newCinemaModel);
            if (data == null)
            {
                return null;
            }
            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };
            return domainModel; 
        }

        public async Task<CinemaDomainModel> DeleteCinema(int cinemaId)
        {
            var deletedCinema = _cinemasRepository.Delete(cinemaId);
            if (deletedCinema == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel result = new CinemaDomainModel
            {
                Id = deletedCinema.Id,
                Name = deletedCinema.Name
            };

            return result;
        }
    }

}
