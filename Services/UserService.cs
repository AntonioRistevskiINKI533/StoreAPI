using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;

namespace StoreAPI.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameOrEmail(request.Username);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (user.PasswordHash != request.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

 /*           if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }*/

            // Generate token (placeholder, replace with JWT)
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            return new LoginResponse
            {
                Token = token
            };
        }

        public async Task AddUser(AddUserRequest request)
        {
            var user = await _userRepository.GetByUsernameOrEmail(request.Username);

            if (user != null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,//hash this
            };

            await _userRepository.AddUser(user);

            return;
        }
    }
}
