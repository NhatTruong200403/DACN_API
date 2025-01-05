using AutoMapper;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Controllers.Employee
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee, Admin")]
    [ApiController]
    public class BookingRequestController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public BookingRequestController(IBookingService bookingService,
                                        IInvoiceService invoiceService,
                                        IMapper mapper)
        {
            _bookingService = bookingService;
            _invoiceService = invoiceService;
            _mapper = mapper;
        }


        [HttpGet("GetAllCancelRequest")]
        [Authorize(Roles = "Admin, Employee")]
        public ActionResult<OperationResult> GetAllCancelRequest()//Get tất cả các hủy booking 
        {
            try
            {
                var requests = _bookingService.GetAllCancelRequest();
                var requestVMs = _mapper.Map<List<BookingVM>>(requests);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: requestVMs);
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


        [HttpPost("ExamineCancelBooking/{bookingId}&&{isAccept}")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<ActionResult<OperationResult>> ExamineCancelBooking(int bookingId, bool isAccept) // Xác nhận hủy booking của User từ Employee
        {
            try
            {
                var booking = _bookingService.GetById(bookingId);
                await _bookingService.ExamineCancelBookingRequestAsync(booking, isAccept);
                _invoiceService.Refund(booking, isAccept);
                return new OperationResult(true, "Cancellation request processed successfully", StatusCodes.Status200OK);
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

        [HttpGet("GetAllRefundInvoice")]
        public ActionResult<OperationResult> GetAllRefundInvoice() // lấy tất cả các hóa đơn hoàn lại
        {
            try
            {
                var invoices = _invoiceService.GetAllRefundInvoices();
                var invoiceVMs = _mapper.Map<List<InvoiceVM>>(invoices);
                return new OperationResult(true, statusCode: StatusCodes.Status200OK, data: invoiceVMs);
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
    }
}
