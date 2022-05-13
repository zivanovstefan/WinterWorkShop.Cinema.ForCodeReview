using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<IEnumerable<UserDomainModel>> GetAllAsync()
        {
            var data = await _usersRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<UserDomainModel> result = new List<UserDomainModel>();
            UserDomainModel model;
            foreach (var item in data)
            {
                model = new UserDomainModel
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserName = item.UserName,
                    BonusPoints = item.BonusPoints
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<UserDomainModel> GetUserByIdAsync(Guid id)
        {
            var data = await _usersRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                BonusPoints= data.BonusPoints
            };

            return domainModel;
        }

        public async Task<UserDomainModel> GetUserByUserName(string username)
        {
            var data = _usersRepository.GetByUserName(username);

            if (data == null)
            {
                return null;
            }

            UserDomainModel domainModel = new UserDomainModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                BonusPoints = data.BonusPoints,
                Role = data.Role
            };

            return domainModel;
        }
        public async Task<UserDomainModel> CreateUser(UserDomainModel newUser)
        {
            User userToCreate = new User()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                UserName = newUser.UserName,
                BonusPoints = newUser.BonusPoints,
                Password = newUser.Password,
                Role = newUser.Role
            };
            
            var data = _usersRepository.Insert(userToCreate);
            if (data == null)
            {
                return null;
            }
            _usersRepository.Save();

            UserDomainModel domainModel = new UserDomainModel()
            {
                Id=data.Id,
                FirstName=data.FirstName,
                LastName=data.LastName,
                UserName=data.UserName,
                BonusPoints =data.BonusPoints,
                Role = data.Role
            };
            return domainModel;
        }
        public async Task<int> AddBonusPoints(Guid userId, int bonusPoints)
        {
            var bonusPointCount = _usersRepository.AddBonusPoints(userId, bonusPoints);
            if (bonusPointCount == null || bonusPointCount < 0)
            {
                return -1;
            }
            _usersRepository.Save();

            return bonusPointCount;
        }
        public UserModel Authenticate(UserCred userCred)
        {
            var currentUser = _usersRepository.GetUserByCredentials(userCred.Username, userCred.Password);
            if (currentUser != null)
            {
                var resultUser = new UserModel()
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Password = currentUser.Password,
                    UserName = currentUser.UserName,
                    Role = currentUser.Role
                };
                return resultUser;
            }
            return null;
        }
    }
}
