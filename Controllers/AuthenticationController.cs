﻿using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoWheels_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenService;

        public AuthenticationController(AuthenticationService service)
        {
            _authenService = service;
        }

        [HttpPost("Login")]
        public async Task<string> Login(LoginVM loginViewModel)
        {
            var result = await _authenService.LoginAsync(loginViewModel);
            return result;
        }

        [HttpPost("SignUp")]
        public async Task<IdentityResult> SignUp(SignUpVM signUpViewModel)
        {
            var result = await _authenService.SignUpAsync(signUpViewModel);
            return result;
        }
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserVM>> GetUser(string token)
        {
            Console.WriteLine($"Token nhận được: {token}");
            var result = await _authenService.GetUserFromToken(token);
            if (result == null)
            {
                return NotFound("Người dùng không tồn tại hoặc token không hợp lệ");
            }
            return Ok(result);
        }
    }
}
