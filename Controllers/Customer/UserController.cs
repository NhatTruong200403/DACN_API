using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("GetUserByToken")]
        [Authorize]
        public async Task<ActionResult<OperationResult>> GetUserByTokenAsync()
        {
            try
            {
                var user = await _userService.GetByUserIdAsync();
                var userVM = _mapper.Map<UserVM>(user);
                var userRoles = await _userService.GetUserRolesAsync(user);
                foreach (var role in userRoles)
                {
                    userVM.Roles.Add(role);
                }
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: userVM);
            }
            catch (AutoMapperMappingException mapperEx)
            {
                return new OperationResult(false, mapperEx.Message, StatusCodes.Status422UnprocessableEntity);
            }
            catch (NullReferenceException nullEx)
            {
                return new OperationResult(false, nullEx.Message, StatusCodes.Status204NoContent);

            }
            catch (Exception ex)
            {
                var exMessage = ex.Message ?? "An error occurred while updating the database.";
                return new OperationResult(false, exMessage, StatusCodes.Status400BadRequest);
            }
        }

        [HttpPut("UpdateUserInfo")]
        [Authorize]
        public async Task<ActionResult<OperationResult>> UpdateUserInfoAsync([FromForm] UserDTO userDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _mapper.Map<ApplicationUser>(userDTO);
                    await _userService.UpdateUserInfoAsync(user, userDTO.License!, userDTO.CIC!, userDTO.Image!);
                    return new OperationResult(true, "User information update succesfully", StatusCodes.Status200OK);
                }
                return BadRequest("User data invalid");
            }
            catch (DbUpdateException dbEx)
            {
                return new OperationResult(false, dbEx.Message, StatusCodes.Status500InternalServerError);
            }
            catch (InvalidOperationException operationEx)
            {
                return new OperationResult(false, operationEx.Message, StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message, StatusCodes.Status400BadRequest);
            }
        }
       
    }
}
