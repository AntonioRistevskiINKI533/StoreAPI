using StoreAPI.Repositories;
using StoreAPI.Models.Datas;
using StoreAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using StoreAPI.Models;
using BCrypt.Net;

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

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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
            if (request.Email.Contains('@') == false)
            {
                throw new Exception("Invalid email");
            }

            var user = await _userRepository.GetByUsernameOrEmail(request.Username);

            if (user != null)
            {
                throw new Exception("User with same email or username already exists");
            }

            user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)//hash this
            };

            await _userRepository.AddUser(user);

            return;
        }
    }
}
