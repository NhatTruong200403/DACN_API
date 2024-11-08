using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
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
    public class UserPromotionController : ControllerBase
    {
        private readonly UserPromotionService _promotionService;
        private readonly IMapper _mapper;

        public UserPromotionController(UserPromotionService promotionService, IMapper mapper)
        {
            _promotionService = promotionService;
            _mapper = mapper;
        }

        [HttpGet("GetAllByUserId")]
        public async Task<ActionResult<OperationResult>> GetAllAsync()
        {
            try
            {
                var promotions = await _promotionService.GetAllByUserId();
                var promotionVMs = _mapper.Map<List<PromotionVM>>(promotions);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: promotionVMs);
            }
            catch (NullReferenceException aEx)
            {
                return new OperationResult(false, aEx.Message, StatusCodes.Status204NoContent);
            }
            catch (AutoMapperMappingException mapperEx)
            {
                return new OperationResult(false, mapperEx.Message, StatusCodes.Status422UnprocessableEntity);
            }
            catch (Exception ex)
            {
                var exMessage = ex.Message ?? "An error occurred while updating the database.";
                return new OperationResult(false, exMessage, StatusCodes.Status400BadRequest);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<OperationResult>> GetByIdAsync(int id)
        {
            try
            {
                var promotion = await _promotionService.GetByIdAsync(id);
                var promotionVM = _mapper.Map<PromotionVM>(promotion);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: promotionVM);
            }
            catch (NullReferenceException aEx)
            {
                return new OperationResult(false, aEx.Message, StatusCodes.Status204NoContent);
            }
            catch (AutoMapperMappingException mapperEx)
            {
                return new OperationResult(false, mapperEx.Message, StatusCodes.Status422UnprocessableEntity);
            }
            catch (Exception ex)
            {
                var exMessage = ex.Message ?? "An error occurred while updating the database.";
                return new OperationResult(false, exMessage, StatusCodes.Status400BadRequest);
            }
        }

        [HttpPost("Add")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> AddAsync([FromForm] PromotionDTO promotionDTO)
        {
            try
            {
                if (promotionDTO == null)
                {
                    return BadRequest("Promotion value is null");
                }
                if (ModelState.IsValid)
                {
                    var promotion = _mapper.Map<Promotion>(promotionDTO);
                    await _promotionService.AddAsync(promotion, promotionDTO.PostIds);
                    return new OperationResult(true, "Promotion add succesfully", StatusCodes.Status200OK);
                }
                return BadRequest("Promotion data invalid");
            }
            catch (UnauthorizedAccessException authEx)
            {
                return new OperationResult(false, authEx.Message, StatusCodes.Status401Unauthorized);
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

        [HttpPost("Update/{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> UpdateAsync(int id, [FromForm] PromotionDTO promotionDTO)
        {
            try
            {
                if (promotionDTO == null || id != promotionDTO.Id)
                {
                    return BadRequest("Invalid request");
                }
                if (ModelState.IsValid)
                {
                    var promotion = _mapper.Map<Promotion>(promotionDTO);
                    await _promotionService.UpdateAsync(id, promotion, promotionDTO.PostIds);
                    return new OperationResult(true, "Promotion update succesfully", StatusCodes.Status200OK);

                }
                return BadRequest("Promotion data invalid");
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

        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> DeleteAsync(int id)
        {
            try
            {
                await _promotionService.DeleteByIdAsync(id);
                return new OperationResult(true, "Promotion deleted succesfully", StatusCodes.Status200OK);
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
