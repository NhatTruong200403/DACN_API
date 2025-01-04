using AutoMapper;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace GoWheels_WebAPI.Controllers.Customer
{
    [Area("User")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        public bool isMB { get; set; }
        private readonly IInvoiceService _invoiceService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        public InvoiceController(IInvoiceService invoiceService, IBookingService bookingService, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet("GetPersonalInvoices")]
        [Authorize(Roles = "User")]
        public ActionResult<OperationResult> GetPersonalInvoices()
        {
            try
            {
                var invoices = _invoiceService.GetPersonalInvoices();
                var invoiceVMs = _mapper.Map<List<InvoiceVM>>(invoices);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: invoiceVMs);
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

        /*        [HttpGet("GetAllByDriver")]//Lấy các hóa đơn cá nhân của tài xế
                [Authorize(Roles = "Driver")]
                public ActionResult<OperationResult> GetAllByDriver()
                {
                    try
                    {
                        var invoices = _invoiceService.GetAllByDriver();
                        var invoiceVMs = _mapper.Map<List<InvoiceVM>>(invoices);
                        return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: invoiceVMs);
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
                }*/



        [HttpPost("MomoPayment/{bookingId}&&{isMobile}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> MomoPayment(int bookingId, bool isMobile)
        {
            try
            {
                var booking = _bookingService.GetById(bookingId);
                if (!booking.OwnerConfirm)
                {
                    return BadRequest("Owner confirm required");
                }
                var responseFromMomo = await _invoiceService.ProcessMomoPayment(booking, isMobile);
                JObject jmessage = JObject.Parse(responseFromMomo);
                var payUrlToken = jmessage.GetValue("payUrl");
                if (payUrlToken != null)
                {
                    string payUrl = payUrlToken.ToString();
                    if (!string.IsNullOrEmpty(payUrl))
                    {
                        return Ok(payUrl);
                    }
                }

                return Ok(responseFromMomo); // Handle failure case
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpGet("ReturnUrl")]
        public ActionResult<OperationResult> ReturnUrl()
        {
            try
            {
                _invoiceService.ProcessReturnUrl(Request.Query);                
                return Redirect("http://localhost:5173/Information/User/HistoryInvoice");
                
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

        /*[HttpGet("CalculateRevenuesByMonth/{year}")]
        [Authorize(Roles = "User")]
        public ActionResult<OperationResult> CalculateRevenuesByMonth(int year)
        {
            try
            {
                var revenues = _invoiceService.CalculateRevenuesByMonth(year);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: revenues);
            }
            catch (Exception ex)
            {
                var exMessage = ex.Message ?? "An error occurred while updating the database.";
                return new OperationResult(false, exMessage, StatusCodes.Status400BadRequest);
            }
        }*/
    }
}
