using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class UserAuthenticationServiceAPI : IUserAuthenticationServiceAPI
    {
        private readonly IUserRepositoryDB _userRepository;

        public UserAuthenticationServiceAPI(
            IUserRepositoryDB userRepository)
        {
            _userRepository = userRepository;
        }

        public UserDB? Authenticate(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null || string.IsNullOrWhiteSpace(user.Password))
            {
                return null;
            }

            if (BCryptNet.Verify(password, user.Password))
            {
                return user;
            }

            return null;
        }

        public IActionResult Create(UserDB user)
        {
            _userRepository.Create(user);

            return new OkResult();
        }

        public UserDB? Read(string id) =>
            _userRepository.Read(id);

        public IActionResult Update(string id, UserDB user)
        {
            var result = _userRepository.Update(id, user);

            return result
                ? new OkResult()
                : new NotFoundResult();
        }

        public IActionResult Delete(string id)
        {
            var result = _userRepository.Delete(id);

            return result
                ? new OkResult()
                : new NotFoundResult();
        }
    }
}
