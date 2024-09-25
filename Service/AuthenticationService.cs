using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GoWheels_WebAPI.Service
{
    public class AuthenticationService
    {
        private readonly IUserRepository _autheticationRepository;
        private readonly IConfiguration _config;

        public AuthenticationService(IUserRepository autheticationRepository, IConfiguration config)
        {
            _autheticationRepository = autheticationRepository;
            _config = config;
        }

        public async Task<IdentityResult> SignUpAsync(SignUpVM signUpViewModel)
        {
            var user = new ApplicationUser
            {
                Name = signUpViewModel.UserName,
                Email = signUpViewModel.Email,
                UserName = signUpViewModel.Email
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
            var user = await _autheticationRepository.FindByUserNameAsync(loginViewModel.UserName);
            if (user == null)
            {
                return "Login failed";
            }
            var isPasswordValid = await _autheticationRepository.ValidatePasswordAsync(user, loginViewModel.Password);
            if (!isPasswordValid)
            {
                return "Login failed";
            }
            var token = await GenerateJwtToken(user);

            return token;
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Name", user.Name!),
                new Claim("Email", user.Email!),
                new Claim("Id", user.Id)
            };

            var userRoles = await _autheticationRepository.GetUserRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim("Role", role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]!));
            var token = new JwtSecurityToken
            (
                issuer: _config["JWT:Issuer"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
