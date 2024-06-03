﻿using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.DB.RepositoriesDB;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class UserAuthenticationServiceAPI : IUserAuthenticationServiceAPI
    {
        private readonly IUserRepositoryDB _userRepository;

        public UserAuthenticationServiceAPI(IUserRepositoryDB userRepository)
        {
            _userRepository = userRepository;
        }

        public UserDB Authenticate(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user != null && user.Password == password)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public IActionResult Create(UserDB user)
        {
            _userRepository.Create(user);
            return new OkResult();
        }

        public UserDB Read(string id)
        {
            return _userRepository.Read(id);
        }

        public IActionResult Update(string id, UserDB user)
        {
            var result = _userRepository.Update(id, user);
            if (result)
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }

        public IActionResult Delete(string id)
        {
            var result = _userRepository.Delete(id);
            if (result)
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
