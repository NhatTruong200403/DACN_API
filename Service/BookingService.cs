using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Hubs;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.GoogleRespone;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPostService _postService;
        private readonly ILocatorService _googleApiService;
        private readonly INotifyService _notifyService;
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly string _userId;


        public BookingService(IBookingRepository bookingRepository, 
                                IPostService postService,
                                ILocatorService googleApiService,
                                INotifyService notifyService,
                                IHubContext<NotifyHub> hubContext,
                                IHttpContextAccessor httpContextAccessor,
                                ApplicationDbContext context)
        {
            _bookingRepository = bookingRepository;
            _postService = postService;
            _googleApiService = googleApiService;
            _notifyService = notifyService;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userId = _httpContextAccessor.HttpContext?.User?
                     .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public List<Booking> GetAllUnRecieveBookingsByPostId(int postId)
            => _bookingRepository.GetAllUnRecieveBookingByPostId(postId);


        public List<DateTime> GetBookedDateByPostId(int postId)
        {
            var bookings = _bookingRepository.GetAllByPostId(postId);
            if (bookings.Count == 0)
            {
                return new List<DateTime>();
            }
            //Lấy từng ngày trong từng bookingDTO ra và gắn vào 
            var bookedDates = bookings
                            .SelectMany(b => new List<DateTime> { b.RecieveOn, b.ReturnOn }) // Lấy cả hai ngày
                            .ToList();
            return bookedDates;
        }

        public List<Booking> GetAllByAdmin()
            => _bookingRepository.GetAllByAdmin(_userId);



        public List<Booking> GetAllWaitingBookingsByPostId(int postId)
            => _bookingRepository.GetAllWaitingBookingByPostId(postId);

        public List<Booking> GetAllPendingBookingsByUserId()
            => _bookingRepository.GetAllPendingBookingByUserId(_userId);

        public List<Booking> GetAll()
            => _bookingRepository.GetAll();

        public List<Booking> GetAllCompleteBooking()
            => _bookingRepository.GetAllCompleteBookings();

        public List<Booking> GetAllCancelRequest()
            => _bookingRepository.GetAllCancelRequest();


        public List<Booking> GetPersonalBookings()
            => _bookingRepository.GetAllPersonalBookings(_userId);
        private List<int> FilterBookingsWithinRange(DistanceMatrixRespone distanceMatrixRespone, List<(int bookingId, string location)> bookingLocations)
        {
            var bookingsWithinRange = new List<int>();
            for (var i = 0; i < distanceMatrixRespone.Rows.Count; i++)
            {
                var distance = distanceMatrixRespone.Rows[i].Elements[0].Distance?.Value ?? int.MaxValue;
                if (distance < 7000)
                {
                    bookingsWithinRange.Add(bookingLocations[i].bookingId);
                }
            }
            return bookingsWithinRange;
        }

        public Booking GetById(int id)
            => _bookingRepository.GetById(id);

        public bool CheckBookingValue(BookingDTO bookingDTO, decimal promotionValue)
        {
            var post = _postService.GetById(bookingDTO.PostId);
            var bookingHours = (bookingDTO.ReturnOn - bookingDTO.RecieveOn).TotalHours;
            var bookingDays = (bookingDTO.ReturnOn - bookingDTO.RecieveOn).TotalDays;
            var isPrePaymentValid = bookingDTO.PrePayment == bookingDTO.FinalValue / 2;
            var isFinalValueValid = true;
            if (promotionValue > 1)
            {
                isFinalValueValid = bookingDTO.FinalValue == bookingDTO.Total - promotionValue;
            }
            else
            {
                var value = bookingDTO.Total * (1 - promotionValue);
                isFinalValueValid = bookingDTO.FinalValue == Math.Ceiling(value);
            }
            if (isPrePaymentValid && isFinalValueValid)
            {
                if (bookingHours % 24 != 0)
                {
                    if (bookingDTO.Total == (post.PricePerHour * (decimal)bookingHours))
                    {
                        return true;
                    }
                }
                else
                {
                    if (bookingDTO.Total == (post.PricePerDay * (decimal)bookingDays))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task Add(Booking booking)
        {
            try
            {
                var post = _postService.GetById(booking.PostId);
                if (post.IsDeleted)
                {
                    throw new InvalidOperationException("Post unavailable");
                }
                booking.CreatedById = _userId;
                booking.UserId = _userId;
                booking.CreatedOn = DateTime.Now;
                booking.Status = "Pending";
                _bookingRepository.Add(booking);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }           
        }

        public void Update(int id, Booking booking)
        {
            try
            {
                var existingBooking = _bookingRepository.GetById(id);
                booking.CreatedById = existingBooking.CreatedById;
                booking.CreatedOn = existingBooking.CreatedOn;
                booking.ModifiedById = existingBooking.ModifiedById;
                booking.ModifiedOn = existingBooking.ModifiedOn;
                booking.PrePayment = existingBooking.PrePayment;
                booking.RecieveOn = existingBooking.RecieveOn;
                booking.ReturnOn = existingBooking.ReturnOn;
                booking.FinalValue = existingBooking.FinalValue;
                booking.Total = existingBooking.Total;
                booking.UserId = existingBooking.UserId;
                booking.User = existingBooking.User;
                booking.PostId = existingBooking.PostId;
                booking.Post = existingBooking.Post;
                booking.IsDeleted = existingBooking.IsDeleted;
                var isValueChange = EditHelper<Booking>.HasChanges(booking, existingBooking);
                EditHelper<Booking>.SetModifiedIfNecessary(booking, isValueChange, existingBooking, _userId);
                _bookingRepository.Update(booking);
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

        public async Task ConfirmBooking(int id, bool isAccept)
        {
            try
            {
                var booking = _bookingRepository.GetById(id);
                booking.ModifiedById = _userId;
                booking.ModifiedOn = DateTime.Now;
                booking.Status = isAccept ? "Accept Booking" : "Denied";
                booking.OwnerConfirm = isAccept;
                _bookingRepository.Update(booking);
                var notify = new Notify()
                {
                    BookingId = booking.Id,
                    UserId = booking.UserId,
                    CreateOn = DateTime.Now,
                    IsDeleted = false,
                    IsRead = false
                };
                if (isAccept)
                {
                    notify.Content = "Your booking confirmed by owner";
                }
                else
                {
                    notify.Content = "Your booking has been denied";
                }
                _notifyService.Add(notify);
                if (NotifyHub.userConnectionsDic.TryGetValue(booking.UserId!, out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", notify.Content);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }    
        }

        public void UpdateBookingStatus()
        {
            try
            {
                var bookings = _bookingRepository.GetAll();
                if (bookings.Count == 0)
                {
                    return;
                }
                foreach (var booking in bookings)
                {
                    if (booking.IsPay && booking.Status.Equals("Waiting") && booking.RecieveOn <= DateTime.Now)
                    {
                        booking.Status = "Renting";
                    }
                    else if (booking.IsPay && booking.Status.Equals("Renting") && booking.ReturnOn < DateTime.Now)
                    {
                        booking.Status = "Conplete";
                    }
                    else if (!booking.IsPay && booking.RecieveOn <= DateTime.Now)
                    {
                        booking.Status = "Canceled";
                    }
                    _bookingRepository.Update(booking);
                }
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

        public void Delete(int id)
        {
            try
            {
                var booking = _bookingRepository.GetById(id);
                booking.IsDeleted = true;
                _bookingRepository.Update(booking);
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

        public void RequestCancelBooking(int id)
        {
            try
            {
                var existingBooking = _bookingRepository.GetById(id);
                if (existingBooking.UserId != _userId)
                {
                    throw new UnauthorizedAccessException("Unauthorized");
                }
                existingBooking.ModifiedById = _userId;
                existingBooking.ModifiedOn = DateTime.Now;
                if (existingBooking.IsPay)
                {
                    existingBooking.IsRequest = true;
                    existingBooking.Status = "Processing";
                }
                else
                {
                    existingBooking.IsRequest = true;
                    existingBooking.IsResponse = true;
                    existingBooking.Status = "Canceled";
                }
                _bookingRepository.Update(existingBooking);
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

        public async Task ExamineCancelBookingRequestAsync(Booking booking, bool isAccept)
        {
            try
            {
                var notify = new Notify()
                {
                    BookingId = booking.Id,
                    UserId = booking.UserId,
                    CreateOn = DateTime.Now,
                    IsRead = false,
                    IsDeleted = false
                };
                if (isAccept)
                {
                    if (booking.IsPay)
                    {
                        booking.Status = "Refunded";
                    }
                    else
                    {
                        booking.Status = "Canceled";
                    }
                    notify.Content = "Your cancellation request has been confirmed";
                }
                else
                {
                    notify.Content = "Your cancellation request has been denied";
                }
                booking.IsResponse = true;
                booking.ModifiedById = _userId;
                booking.ModifiedOn = DateTime.Now;
                _bookingRepository.Update(booking);
                _notifyService.Add(notify);
                if (NotifyHub.userConnectionsDic.TryGetValue(booking.UserId!, out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", notify.Content);
                }     
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CancelReportedBookings(Booking booking)
        {
            try
            {
                booking.Status = booking.IsPay ? "Refunded" : "Canceled";
                booking.IsResponse = true;
                _bookingRepository.Update(booking);
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

        public async Task AddDriverToBookingAsync(int bookingId)
        {
            try
            {
                var booking = _bookingRepository.GetById(bookingId);
                booking.FinalValue += 20000 * (decimal)(booking.ReturnOn - booking.RecieveOn).TotalHours;
                booking.PrePayment = booking.FinalValue / 2;
                _bookingRepository.Update(booking);
                var notify = new Notify()
                {
                    BookingId = booking.Id,
                    UserId = booking.UserId,
                    CreateOn = DateTime.Now,
                    IsRead = false,
                    IsDeleted = false,
                    Content = "The driver has accepted your booking"
                };
                _notifyService.Add(notify);
                if (NotifyHub.userConnectionsDic.TryGetValue(booking.UserId!, out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", notify.Content);
                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task RemoveDriverFromBookingAsync(int bookingId)
        {
            try
            {
                var booking = _bookingRepository.GetById(bookingId);
                booking.FinalValue -= 20000 * (decimal)(booking.ReturnOn - booking.RecieveOn).TotalHours;
                booking.PrePayment = booking.FinalValue / 2;
                _bookingRepository.Update(booking);
                var notify = new Notify()
                {
                    BookingId = booking.Id,
                    UserId = booking.UserId,
                    CreateOn = DateTime.Now,
                    IsRead = false,
                    IsDeleted = false,
                    Content = "Your booking driver has cancel"
                };
                _notifyService.Add(notify);
                if (NotifyHub.userConnectionsDic.TryGetValue(booking.UserId!, out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", notify.Content);
                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
