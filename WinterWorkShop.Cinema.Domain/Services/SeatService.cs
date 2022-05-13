using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.ErrorModels;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatsRepository _seatsRepository;

        public SeatService(ISeatsRepository seatsRepository)
        {
            _seatsRepository = seatsRepository;
        }

        public async Task<IEnumerable<SeatDomainModel>> GetAllAsync()
        {
            var data = await _seatsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<SeatDomainModel> result = new List<SeatDomainModel>();
            SeatDomainModel model;
            foreach (var item in data)
            {
                model = new SeatDomainModel
                {
                    Id = item.Id,
                    AuditoriumId = item.AuditoriumId,
                    Number = item.Number,
                    Row = item.Row
                };
                result.Add(model);
            }

            return result;
        }
        public async Task<SeatDomainModel> AddSeat(SeatDomainModel newSeat)
        {
            Seat seatToCreate = new Seat()
            {
                AuditoriumId =newSeat.AuditoriumId,
                Row = newSeat.Row,
                Number = newSeat.Number
            };
            //check if seat already exists
            var seatExists = await _seatsRepository.GetAll();
            var seat = seatExists.SingleOrDefault(seat => seat.AuditoriumId == seatToCreate.AuditoriumId
                                            && seat.Number == seatToCreate.Number
                                            && seat.Row == seatToCreate.Row);
            if (seat != null)
            {
                var errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = Messages.SEAT_EXISTS_ERROR,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return await Task.FromResult<SeatDomainModel>(null);
                //return null;
            }
            //if seat doesnt exists, create one
            var data = _seatsRepository.Insert(seatToCreate);
            if (data == null)
            {
                return null;
            }
            _seatsRepository.Save();

            SeatDomainModel domainModel = new SeatDomainModel()
            {
                Id = data.Id,
                AuditoriumId=data.AuditoriumId,
                Row=data.Row,
                Number = data.Number

            };
            return domainModel;
        }
        public async Task<SeatDomainModel> GetSeatByIdAsync(Guid id)
        {
            var data = await _seatsRepository.GetByIdAsync(id);

            if (data == null)
            {
                var errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = Messages.SEAT_DOESNT_EXIST_ERROR,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return await Task.FromResult<SeatDomainModel>(null);
            }

            var result = new SeatDomainModel()
            {
                 Id = data.Id,
                 AuditoriumId = data.AuditoriumId,
                 Number = data.Number,
                 Row = data.Row
            };

            return result;
        }
    }
}
