﻿using Meetups.Domain.Filters;
using Meetups.Domain.Interfaces.Services;
using Meetups.Domain.Models.Entities;
using Meetups.Domain.Models.Requests;
using Meetups.Domain.Models.Responses;
using Meetups.Providers;
using System;
using System.Linq;

namespace Meetups.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public UsersSearchResponse Get(UserSearchFilter filter)
        {
            var q = context.User.Where(x => true);

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
                q = q.Where(x => x.FirstName.Contains(filter.FirstName));

            if (!string.IsNullOrWhiteSpace(filter.LastName))
                q = q.Where(x => x.LastName.Contains(filter.LastName));

            if (!string.IsNullOrWhiteSpace(filter.UserName))
                q = q.Where(x => x.UserName.Contains(filter.UserName));

            return new UsersSearchResponse()
            { 
                Users = q.ToList()
            };
        }

        public int Create(UserRequest request)
        {
            try
            {
                var user = new User
                {
                    FirstName = request.Name,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Password = request.Password
                };

                context.Add(user);
                context.SaveChanges();

                return user.Id;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.Id == id);

                if (user == null)
                    throw new Exception("Not found");

                context.User.Remove(user);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(UserRequest request)
        {
            try
            {
                var user = context.User.First(x => x.Id == request.Id);
                user.FirstName = request.Name;
                user.LastName = request.LastName;
                user.UserName = request.UserName;
                user.Password = request.Password;

                context.Update(user);
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }   
        
        public User ValidateLogin(LoginRequest loginDetails)
        {
            return context.User.FirstOrDefault(x => x.UserName == loginDetails.User && x.Password == loginDetails.Password);
        }
    }
}
