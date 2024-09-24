﻿using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace GoWheels_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = ("Admin, User"))]
    public class SalePromotionController : ControllerBase
    {
        private readonly SalePromotionService _salePromotionService;
        public SalePromotionController(SalePromotionService salePromotionService)
        {
            _salePromotionService = salePromotionService;
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<OperationResult>> GetAllAsync()
        {
            var result = await _salePromotionService.GetAllAsync();
            return result;
        }

        [HttpGet("GetAllByType/{id}")]
        public async Task<ActionResult<OperationResult>> GetAllByPromotionType(int id)
        {
            var result = await _salePromotionService.GetPromotionByUserId(id);
            return result;
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<OperationResult>> GetByIdAsync(int id)
        {
            var result = await _salePromotionService.GetByIdAsync(id);
            return result;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<OperationResult>> AddAsync([FromBody] SalePromotionDTO salePromotionDto)
        {
            if(salePromotionDto == null)
            {
                return BadRequest("Promotion is null");
            }    
            if (ModelState.IsValid)
            {
                var result = await _salePromotionService.AddAsync(salePromotionDto);
                return result;
            }
            return BadRequest("Sale Promotion data invalid");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<OperationResult>> DeleteAsync(int id)
        {
            var result = await _salePromotionService.DeletedByIdAsync(id);
            return result;
        }

        [HttpPost("Update/{id}")]
        public async Task<ActionResult<OperationResult>> UpdateAsync(int id, [FromBody] SalePromotionDTO salePromotionDto)
        {
            if(salePromotionDto == null || id != salePromotionDto.Id)
            {
                return BadRequest("Invalid request");
            }    
            if (ModelState.IsValid)
            {
                var result = await _salePromotionService.UpdateAsync(id, salePromotionDto);
                return result;
            }
            return BadRequest("Sale Promotion data invalid");
        }
    }
}
