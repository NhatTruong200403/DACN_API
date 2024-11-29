using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GoWheels_WebAPI.Controllers.Customer
{
    [Area("User")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly AdminPromotionService _adminPromotionService;
        private readonly InvoiceService _invoiceService;
        private IMapper _mapper;

        public BookingController(BookingService bookingService, 
                                    AdminPromotionService adminPromotionService, 
                                    InvoiceService invoiceService, 
                                    IMapper mapper)
        {
            _bookingService = bookingService;
            _adminPromotionService = adminPromotionService;
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<ActionResult<OperationResult>> GetById(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                var bookingVM = _mapper.Map<BookingVM>(booking);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookingVM);
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

        [HttpGet("GetPersonalBookings")] // khách hàng
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> GetPersonalBookings()
        {
            try
            {
                var bookings = await _bookingService.GetPersonalBookingsAsync();
                var bookingVMs = _mapper.Map<List<BookingVM>>(bookings);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookingVMs);
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


        [HttpGet("GetAllDriverRequireBookings")] // getall cho tài xế
        public async Task<ActionResult<OperationResult>> GetAllDriverRequireBookingsAsync()
        {
            try
            {
                var bookings = await _bookingService.GetAllDriverRequireBookingsAsync();
                var bookingVMs = _mapper.Map<List<BookingVM>>(bookings);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookingVMs);
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

        [HttpGet("GetAllByDriver")]
        public async Task<ActionResult<OperationResult>> GetAllByDriverAsync() 
        {
            try
            {
                var bookings = await _bookingService.GetAllByDriverAsync();
                var bookingVMs = _mapper.Map<List<BookingVM>>(bookings);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookingVMs);
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

        [HttpGet("GetAllPendingBookingsByUserId")] // chủ xe
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> GetAllPendingBookingsByUserId()
        {
            try
            {
                var bookings = await _bookingService.GetAllPendingBookingsByUserIdAsync();
                var bookingVMs = _mapper.Map<List<BookingVM>>(bookings);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookingVMs);
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

        [HttpGet("GetAllBookedDates/{id}")]
        public async Task<ActionResult<OperationResult>> GetAllBookedDatesByPostId(int id)
        {
            try
            {
                var bookedDates = await _bookingService.GetBookedDateByPostIdsAsync(id);
                return new OperationResult(true,statusCode: StatusCodes.Status200OK, data: bookedDates);
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


        [HttpPost("SendCancelRequest/{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> SendCancelRequestBookingAsync(int id)  //Hủy đơn đjăt xe từ User RỒI NHA
        {
            try
            {
                await _bookingService.RequestCancelBookingAsync(id);
                return new OperationResult(true, "Cancel booking request sent succesfully", StatusCodes.Status200OK);
            }
            catch (NullReferenceException nullEx)
            {
                return new OperationResult(false, nullEx.Message, StatusCodes.Status204NoContent);
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

        [HttpPost("Add")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> AddAsync([FromBody] BookingDTO bookingDTO)
        {
            try
            {
/*                if (bookingDTO.RecieveOn < DateTime.Now || bookingDTO.ReturnOn < bookingDTO.RecieveOn)
                {
                    return BadRequest("return date or recieve date invalid");
                }*/
                if (ModelState.IsValid)
                {
                    var booking = _mapper.Map<Booking>(bookingDTO);  
                    var isBookingValid = await _bookingService.CheckBookingValue(bookingDTO, bookingDTO.DiscountValue);
                    if (isBookingValid)
                    {
                        await _bookingService.AddAsync(booking);
                        return new OperationResult(true, "Booking add succesfully", StatusCodes.Status200OK);
                    }
                    return new OperationResult(false, "Booking values invalid", StatusCodes.Status400BadRequest);
                }
                return BadRequest("Booking data invalid");
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

        [HttpPut("ConfirmBooking")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<OperationResult>> ConfirmBookingAsync([FromForm] int id,[FromForm] bool isAccept)
        {
            try
            {
                await _bookingService.UpdateOwnerConfirmAsync(id, isAccept);
                if(isAccept)
                {
                    await _invoiceService.CreateInvoiceAsync(id);
                }    
                return new OperationResult(true, "Booking confirmed", StatusCodes.Status200OK);
            }
            catch(UnauthorizedAccessException authEx) 
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

    }
}
