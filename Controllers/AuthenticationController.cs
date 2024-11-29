using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Utilities;
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
        public async Task<ActionResult<OperationResult>> Login(LoginVM loginViewModel)
        {
            try
            {
                var result = await _authenService.LoginAsync(loginViewModel);
                //var user = await _authenService.GetUserFromToken(result);
                return new OperationResult(true, result, StatusCodes.Status200OK);
            }
            catch(NullReferenceException nullEx) 
            {
                return new OperationResult(false, nullEx.Message, StatusCodes.Status404NotFound);
            }   
            catch (InvalidOperationException operationEx) 
            {
                return new OperationResult(false, operationEx.Message, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("SignUp")]
        public async Task<IdentityResult> SignUp(SignUpVM signUpViewModel)
        {
            var result = await _authenService.SignUpAsync(signUpViewModel);
            return result;
        }

        [HttpGet("CheckLockoutStatus")]
        public async Task<IActionResult> CheckLockoutStatus()
        {
            try
            {
                var result = await _authenService.CheckLockoutStatus();
                return Ok(result);
            }
            catch (NullReferenceException nullEx)
            {
                return NotFound(nullEx.Message);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<UserVM>> GetUser()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.StartsWith("Bearer ") ? authorizationHeader.Substring("Bearer ".Length) : authorizationHeader;
            var result = await _authenService.GetUserFromToken(token);
            if (result == null)
            {
                return NotFound("Người dùng không tồn tại hoặc token không hợp lệ");
            }
            return Ok(result);
        }
    }
}
