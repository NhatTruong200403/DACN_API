using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Controllers.Customer
{
    [Area("User")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private IMapper _mapper;

        public BookingController(IBookingService bookingService,
                                    IInvoiceService invoiceService, 
                                    IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public ActionResult<OperationResult> GetById(int id)
        {
            try
            {
                var booking = _bookingService.GetById(id);
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
        public ActionResult<OperationResult> GetPersonalBookings()
        {
            try
            {
                var bookings = _bookingService.GetPersonalBookings();
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


        [HttpGet("GetAllByAdmin")]
        public ActionResult<OperationResult> GetAllByAdmin()
        {
            try
            {
                var bookings = _bookingService.GetAllByAdmin();
                var bookingVMs = _mapper.Map<List<BookingVM>>(bookings);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookingVMs);
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
        public ActionResult<OperationResult> GetAllPendingBookingsByUserId()
        {
            try
            {
                var bookings = _bookingService.GetAllPendingBookingsByUserId();
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
        public ActionResult<OperationResult> GetAllBookedDatesByPostId(int id)
        {
            try
            {
                var bookedDates = _bookingService.GetBookedDateByPostId(id);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: bookedDates);
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
        public ActionResult<OperationResult> SendCancelRequestBooking(int id)  //Hủy đơn đặt xe từ Khách hàng
        {
            try
            {
                _bookingService.RequestCancelBooking(id);
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
        public ActionResult<OperationResult> Add([FromBody] BookingDTO bookingDTO)
        {
            try
            {
                if (bookingDTO.RecieveOn < DateTime.Now || bookingDTO.ReturnOn < bookingDTO.RecieveOn)
                {
                    return BadRequest("return date or recieve date invalid");
                }
                if (ModelState.IsValid)
                {
                    var booking = _mapper.Map<Booking>(bookingDTO);
                    var isBookingValid = _bookingService.CheckBookingValue(bookingDTO, bookingDTO.DiscountValue);
                    if (isBookingValid)
                    {
                        _bookingService.Add(booking);
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

        [HttpPut("ConfirmBooking")]//Chủ xe xác nhận đơn đặt từ khách hàng
        [Authorize(Roles = "Admin, Employee")]
        public async Task<ActionResult<OperationResult>> ConfirmBookingAsync([FromForm] int id,[FromForm] bool isAccept)
        {
            try
            {
                await _bookingService.ConfirmBooking(id, isAccept);
/*                
                if(isAccept)
                {
                    _invoiceService.CreateInvoice(id);
                }   */ 
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
