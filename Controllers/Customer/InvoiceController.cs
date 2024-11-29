using AutoMapper;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GoWheels_WebAPI.Controllers.Customer
{
    [Area("User")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;
        private readonly BookingService _bookingService;
        private readonly IMapper _mapper;
        public InvoiceController(InvoiceService invoiceService, BookingService bookingService, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet("GetPersonalInvoices")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> GetPersonalInvoicesAsync()  //tất cả invoice của User đang đăng nhập
        {
            try
            {
                var invoices = await _invoiceService.GetPersonalInvoicesAsync();
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

        [HttpGet("GetAllByDriver")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> GetAllByDriverAsync()  // dành cho driver
        {
            try
            {
                var invoices = await _invoiceService.GetAllByDriverAsync();
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

        public bool isMB { get; set; }

        [HttpPost("MomoPayment/{bookingId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> MomoPayment(int bookingId,[FromForm] bool isMobile)
        {
            try
            {
                if(isMobile)
                {
                    isMB = true;
                }
                var booking = await _bookingService.GetByIdAsync(bookingId);
                if(!booking.OwnerConfirm)
                {
                    return BadRequest("Owner confirm required");
                }    
                var invoice = await _invoiceService.GetByBookingIdAsync(bookingId);
                var responseFromMomo = await _invoiceService.ProcessMomoPaymentAsync(invoice);
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
        public async Task<ActionResult<OperationResult>> ReturnUrl()
        {
            try
            {
                await _invoiceService.ProcessReturnUrlAsync(Request.Query);
                if (isMB)
                {
                    return new OperationResult(true, "Transaction successfully", StatusCodes.Status200OK);
                }
                else
                {
                    return Redirect("http://192.168.1.7:5173/Information/User/HistoryBooking");
                }
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
