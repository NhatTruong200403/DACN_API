﻿using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoWheels_WebAPI.Service
{
    public class AuthenticationService
    {
        private readonly IUserRepository _autheticationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IConfiguration _config;

        public AuthenticationService(IUserRepository autheticationRepository, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _autheticationRepository = autheticationRepository;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _config = config;
        }

        public async Task<IdentityResult> SignUpAsync(SignUpVM signUpViewModel)
        {
            var user = new ApplicationUser
            {
                Name = signUpViewModel.UserName,
                Email = signUpViewModel.Email,
                UserName = signUpViewModel.Email,
                Image = "https://localhost:7265/images/ImageUser/user.png"
            };
            var result = await _autheticationRepository.CreateUserAsync(user, signUpViewModel.Password);
            if (!result.Succeeded)
            {
                return result;
            }
            await _autheticationRepository.EnsureRoleExistsAsync(ApplicationRole.User);
            await _autheticationRepository.AddUserToRoleAsync(user, ApplicationRole.User);
            return result;
        }

        public async Task<string> LoginAsync(LoginVM loginViewModel)
        {
            var user = await _autheticationRepository.FindByUserNameAsync(loginViewModel.Email);
            if (user == null)
            {
               throw new InvalidOperationException("Wrong email or password");
            }
            var isPasswordValid = await _autheticationRepository.ValidatePasswordAsync(user, loginViewModel.Password);
            if (!isPasswordValid)
            {
                throw new InvalidOperationException("Wrong email or password");
            }
            if (user.LockoutEnabled)
            {
                if (user.LockoutEnd.HasValue)
                {
                    var lockoutDay = (user.LockoutEnd!.Value - DateTime.Now).TotalDays;
                    if (lockoutDay <= 7)
                        return "Account banned until: " + user.LockoutEnd.ToString();
                    return "Account permanently banned";
                }    
            }    
            var token = await GenerateJwtToken(user);

            return token;
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var userRoles = await _autheticationRepository.GetUserRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]!));
            var token = new JwtSecurityToken
            (
                issuer: _config["JWT:Issuer"],
                expires: DateTime.Now.AddDays(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> CheckLockoutStatus()
        {
            var user = await _autheticationRepository.FindByUserIdAsync(_userId);
            return user.LockoutEnabled;
        }

        public async Task<UserVM?> GetUserFromToken(string token)
        {
            // Định nghĩa các hằng số cho Claim Types
            const string ClaimTypeUserId = ClaimTypes.NameIdentifier;
            const string ClaimTypeUserName = ClaimTypes.Name;
            const string ClaimTypeEmail = ClaimTypes.Email;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var claims = jwtToken.Claims;

                var userId = claims.FirstOrDefault(c => c.Type == ClaimTypeUserId)?.Value;
                var userName = claims.FirstOrDefault(c => c.Type == ClaimTypeUserName)?.Value;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypeEmail)?.Value;

                if (userId == null || userName == null || email == null)
                {
                    return null;
                }

                var user = await _autheticationRepository.FindByUserIdAsync(userId);
                if (user == null)
                {
                    return null;
                }

                var userRoles = await _autheticationRepository.GetUserRolesAsync(user);

                return new UserVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    License = user.License,
                    CIC = user.CIC,
                    Image = user.Image,
                    Birthday = user.Birthday,
                    Role = userRoles.FirstOrDefault(),
                };
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi (cân nhắc sử dụng một framework ghi log)
                Console.WriteLine($"Lỗi khi phân tích token: {ex.Message}");
                return null;
            }
        }
    }
}
