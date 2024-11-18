using AutoMapper;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class ReportService
    {
        private readonly ReportRepository _reportRepository;
        private readonly BookingService _bookingService;
        private readonly InvoiceService _invoiceService;
        private readonly UserService _userService;  
        private readonly PostService _postService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _userId;

        public ReportService(ReportRepository reportRepository, 
                                BookingService bookingService,
                                InvoiceService invoiceService,
                                UserService userService,
                                PostService postService,
                                IHttpContextAccessor httpContextAccessor, 
                                IMapper mapper)
        {
            _reportRepository = reportRepository;
            _bookingService = bookingService;
            _invoiceService = invoiceService;
            _userService = userService;
            _postService = postService;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                   .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _mapper = mapper;
        }

        public async Task<List<Report>> GetAllAsync()
            => await _reportRepository.GetAllAsync();

        public async Task<Report> GetByIdAsync(int id)
            => await _reportRepository.GetByIdAsync(id);

        public async Task CreateReportAsync(Report report)
        {
            try
            {
                report.IsDeleted = false;
                report.CreatedById = _userId;
                report.CreatedOn = DateTime.Now;
                await _reportRepository.AddAsync(report);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ConfirmReportAsync(int id, bool isAccept)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(id);
                report.IsDeleted = true;
                report.ModifiedById = _userId;
                report.ModifiedOn = DateTime.Now;
                await _reportRepository.UpdateAsync(report);
                if (isAccept)
                {
                    var post = await _postService.GetByIdAsync(report.PostId);
                    if(post.IsDisabled)
                    {
                        return;
                    }    
                    await _postService.DisablePostByIdAsync(post.Id);
                    var bookings = await _bookingService.GetAllUnRecieveBookingsByPostIdAsync(post.Id);
                    foreach (var booking in bookings)
                    {
                        await _bookingService.ExamineCancelBookingRequestAsync(booking, true);
                        if (booking.IsPay)
                        {
                            await _invoiceService.RefundReportedBookingAsync(booking);
                        }     
                    }
                    await _userService.UpdateUserReportPointAsync(report.Post.UserId!, report.ReportType.ReportPoint);
                }

            }
            catch (NullReferenceException nullEx)
            {
                throw new NullReferenceException(nullEx.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
                catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
