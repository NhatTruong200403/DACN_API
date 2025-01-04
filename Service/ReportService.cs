using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Report> _reportRepository;
        private readonly IBookingService _bookingService;
        private readonly IInvoiceService _invoiceService;
        private readonly IUserService _userService;  
        private readonly IPostService _postService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly string _userId;

        public ReportService(IGenericRepository<Report> reportRepository,
                                IBookingService bookingService,
                                IInvoiceService invoiceService,
                                IUserService userService,
                                IPostService postService,
                                IHttpContextAccessor httpContextAccessor,
                                ApplicationDbContext context)
        {
            _reportRepository = reportRepository;
            _bookingService = bookingService;
            _invoiceService = invoiceService;
            _userService = userService;
            _postService = postService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userId = _httpContextAccessor.HttpContext?.User?
                   .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        
    }
}
