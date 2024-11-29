using AutoMapper;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading;

namespace GoWheels_WebAPI.Controllers.Driver
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverBookingController : ControllerBase
    {
        private readonly DriverBookingService _driverBookingService;
        private readonly BookingService _bookingService;
        private readonly InvoiceService _invoiceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IMapper _mapper;

        public DriverBookingController(DriverBookingService driverBookingService,
                                        BookingService bookingService,
                                        InvoiceService invoiceService,
                                        IHttpContextAccessor httpContextAccessor,
                                        IMapper mapper)
        {
            _driverBookingService = driverBookingService;
            _bookingService = bookingService;
            _invoiceService = invoiceService;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _mapper = mapper;
        }

        [HttpGet("GetAllDriverBookings")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<OperationResult>> GetAllDriverBookingsByUserIdAsync()
        {
            try
            {
                var driverBookings = await _driverBookingService.GetAllByUserIdAsync();
                var driverBookingsVMs = _mapper.Map<List<DriverBookingVM>>(driverBookings);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: driverBookingsVMs);
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

        [HttpPost("AddDriverBooking/{bookingId}")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<OperationResult>> AddDriverBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(bookingId);
                if (booking.HasDriver)
                {
                    return new OperationResult(false, "Driver already assigned", StatusCodes.Status409Conflict);
                }
                await _driverBookingService.AddDriverBookingAsync(booking);
                return new OperationResult(true, "Accept booking succesfully", StatusCodes.Status200OK);
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

        [HttpPost("CancelDriverBooking/{driverBookingId}")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<OperationResult>> CancelDriverBookingAsync(int driverBookingId) // hủy tài xế cho đơn
        {
            try
            {
                var driverBooking = await _driverBookingService.GetByIdAsync(driverBookingId);
                if (driverBooking.Driver.UserId != _userId)
                {
                    return new OperationResult(false, "Unauthorized", StatusCodes.Status401Unauthorized);
                }
                driverBooking.IsCancel = true;
                await _driverBookingService.UpdateAsync(driverBooking);
                var invoice = await _invoiceService.GetByDriverBookingIdAsync(driverBookingId);
                await _invoiceService.UpdateCancelDriverBookingAsync(invoice, driverBooking.Total);
                var booking = await _bookingService.GetByIdAsync(invoice.BookingId);
                booking.HasDriver = false;
                booking.IsRequireDriver = true;
                await _bookingService.UpdateAsync(booking.Id, booking);
                return new OperationResult(true, "Cancel driver booking succesfully", StatusCodes.Status200OK);
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
