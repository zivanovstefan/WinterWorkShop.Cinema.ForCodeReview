﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IUsersRepository : IRepository<User> 
    {
        User GetByUserName(string username);
        int AddBonusPoints(Guid UserId, int bonusPoints);
        User GetUserByCredentials(string username, string password);
    }
    public class UsersRepository : IUsersRepository
    {
        private CinemaContext _cinemaContext;

        public UsersRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public User Delete(object id)
        {
            User existing = _cinemaContext.Users.Find(id);
            var result = _cinemaContext.Users.Remove(existing).Entity;

            return result;
        }

        public async Task<List<User>> GetAll()
        {
            var data = await _cinemaContext.Users.ToListAsync();

            return data;
        }

        public async Task<User> GetByIdAsync(object id)
        {
            return await _cinemaContext.Users.FindAsync(id);
        }

        public User GetByUserName(string username)
        {
            var data = _cinemaContext.Users.SingleOrDefault(x => x.UserName == username);

            return data;
        }

        public User Insert(User obj)
        {
            return _cinemaContext.Users.Add(obj).Entity;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public User Update(User obj)
        {
            var updatedEntry = _cinemaContext.Users.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
        public int AddBonusPoints(Guid UserId, int bonusPoints)
        {
            var userToUpdate = _cinemaContext.Users.Where(x => x.Id == UserId).First();
            userToUpdate.BonusPoints += bonusPoints;
            _cinemaContext.Entry(userToUpdate).State = EntityState.Modified;

            return userToUpdate.BonusPoints;
        }

        public User GetUserByCredentials(string username, string password)
        {
            var data = _cinemaContext.Users.SingleOrDefault(o => o.UserName.ToLower() == username && o.Password == password);
            return data;
        }
    }
}
