using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Service;
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
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public UserController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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



        [HttpPut("SendSubmitDriver")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> SendSubmitDriverAsync()
        {
            try
            {
                await _userService.SendDriverSubmitAsync();
                return new OperationResult(true, "Submit driver sent succesfully", StatusCodes.Status200OK);
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
