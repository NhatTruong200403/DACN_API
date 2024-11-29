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
    [Area("User")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class FavoriteController : ControllerBase
    {
        private readonly FavoriteService _favoriteService;
        private readonly IMapper _mapper;

        public FavoriteController(FavoriteService favoriteService, IMapper mapper)
        {
            _favoriteService = favoriteService;
            _mapper = mapper;
        }

        [HttpGet("GetAllFavorite")]
        public async Task<ActionResult<OperationResult>> GetAllAsync()
        {
            try
            {
                var favorites = await _favoriteService.GetAllAsync();
                var favoriteVMs = _mapper.Map<List<FavoriteVM>>(favorites);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: favoriteVMs);
            }
            catch (NullReferenceException nullEx)
            {
                return new OperationResult(false, nullEx.Message, StatusCodes.Status204NoContent);
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

        [HttpPost("AddToFavorite")]
        public async Task<ActionResult<OperationResult>> AddAsync([FromBody] FavoriteDTO favoriteDTO)
        {
            try
            {
                if (favoriteDTO == null)
                {
                    return BadRequest("Invalid request");
                }
                if (ModelState.IsValid)
                {
                    var favorite = _mapper.Map<Favorite>(favoriteDTO);
                    await _favoriteService.AddAsync(favorite);
                    return new OperationResult(true, "Favorite add succesfully", StatusCodes.Status200OK);
                }
                return BadRequest("Favorite data invalid");
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

        [HttpDelete("RemoveFavorite/{id}")]
        public async Task<ActionResult<OperationResult>> RemoveAsync(int id)
        {
            try
            {
                await _favoriteService.DeletedAsync(id);
                return new OperationResult(true, "Favorite deleted succesfully", StatusCodes.Status200OK);
            }
            catch (NullReferenceException nullEx)
            {
                return new OperationResult(false, nullEx.Message, StatusCodes.Status204NoContent);
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
