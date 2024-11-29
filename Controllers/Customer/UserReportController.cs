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
    public class UserReportController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly IMapper _mapper;
        public UserReportController(ReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpPost("ReportByPostId")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> ReportByPostId([FromBody]ReportDTO reportDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var report = _mapper.Map<Report>(reportDTO);
                    await _reportService.CreateReportAsync(report);
                    return new OperationResult(true, "Report created succesfully", StatusCodes.Status200OK);
                }
                return BadRequest("Report data invalid");
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
