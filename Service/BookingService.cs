using AutoMapper;
using GoWheels_WebAPI.Hubs;
using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.GoogleRespone;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using GoWheels_WebAPI.Utilities;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly PostService _postService;
        private readonly DriverService _driverService;
        private readonly GoogleApiService _googleApiService;
        private readonly NotifyService _notifyService;
        private readonly IHubContext<NotifyHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;


        public BookingService(BookingRepository bookingRepository, 
                                PostService postService,
                                DriverService driverService,
                                GoogleApiService googleApiService,
                                NotifyService notifyService,
                                IHubContext<NotifyHub> hubContext,
                                IMapper mapper, 
                                IHttpContextAccessor httpContextAccessor)
        {
            _bookingRepository = bookingRepository;
            _postService = postService;
            _driverService = driverService;
            _googleApiService = googleApiService;
            _notifyService = notifyService;
            _hubContext = hubContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                     .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
        }

        public async Task<List<Booking>> GetAllUnRecieveBookingsByPostIdAsync(int postId)
            => await _bookingRepository.GetAllUnRecieveBookingByPostIdAsync(postId);


        public async Task<List<DateTime>> GetBookedDateByPostIdsAsync(int postId)
        {
            var bookings = await _bookingRepository.GetAllByPostIdAsync(postId);
            if (bookings.Count == 0)
            {
                return new List<DateTime>();
            }
            //Lấy từng ngày trong từng bookingDTO ra và gắn vào 
            var bookedDates = bookings
                            .SelectMany(b => new List<DateTime> { b.RecieveOn, b.ReturnOn }) // Lấy cả hai ngày
                            .Distinct() // Loại bỏ các ngày trùng lặp
                            .ToList();
            return bookedDates;
        }

        public async Task<List<Booking>> GetAllWaitingBookingsByPostIdAsync(int postId)
            => await _bookingRepository.GetAllWaitingBookingByPostIdAsync(postId);

        public async Task<List<Booking>> GetAllDriverRequireBookingsAsync()
            => await _bookingRepository.GetAllDriverRequireBookingsAsync();
        public async Task<List<Booking>> GetAllPendingBookingsByUserIdAsync()
            => await _bookingRepository.GetAllPendingBookingByUserIdAsync(_userId);

        public async Task<List<Booking>> GetAllAsync()
            => await _bookingRepository.GetAllAsync();

        public async Task<List<Booking>> GetAllCompleteBookingAsync()
            => await _bookingRepository.GetAllCompleteBookingsAsync();

        public async Task<List<Booking>> GetAllCancelRequestAsync()
            => await _bookingRepository.GetAllCancelRequestAsync();


        public async Task<List<Booking>> GetPersonalBookingsAsync()
            => await _bookingRepository.GetAllPersonalBookingsAsync(_userId);

        public async Task<List<Booking>> GetAllByDriverAsync()
            => await _bookingRepository.GetAllByDriverAsync(_userId);

        public async Task<List<Booking>> GetAllByLocation(string latitude, string longitude)
        {
            try
            {
                var driverLocationString = $"{latitude},{longitude}";
                var bookings = await _bookingRepository.GetAllDriverRequireBookingsAsync();
                var bookingLocations = new List<(int bookingId, string location)>();
                foreach(var booking in bookings)
                {
                    var bookingLocationString = $"{booking.Latitude},{booking.Longitude}";
                    bookingLocations.Add((booking.Id, bookingLocationString));
                }
                var respone = await _googleApiService.GetDistanceAsync(bookingLocations, driverLocationString);
                var bookingsWithinRange = GetBookingsWithinRange(respone, bookingLocations);
                var bookingInRange = bookings.Where(b => bookingsWithinRange.Any(id => id == b.Id)).ToList();
                return bookingInRange;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<int> GetBookingsWithinRange(DistanceMatrixRespone distanceMatrixRespone, List<(int bookingId, string location)> bookingLocations)
        {
            var bookingsWithinRange = new List<int>();
            for (var i = 0; i < distanceMatrixRespone.Rows.Count; i++)
            {
                var distance = distanceMatrixRespone.Rows[i].Elements[0].Distance?.Value ?? int.MaxValue;
                if (distance < 10000)
                {
                    bookingsWithinRange.Add(bookingLocations[i].bookingId);
                }
            }
            return bookingsWithinRange;
        }

        public async Task<Booking> GetByIdAsync(int id) 
            => await _bookingRepository.GetByIdAsync(id);

        public async Task<bool> CheckBookingValue(BookingDTO bookingDTO, decimal promotionValue)
        {
            var post = await _postService.GetByIdAsync(bookingDTO.PostId);
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

        public async Task AddAsync(Booking booking)
        {
            try
            {
                var post = await _postService.GetByIdAsync(booking.PostId);
                if (post.IsDisabled) 
                {
                    throw new InvalidOperationException("Post unavailable");
                }
                booking.CreatedById = _userId;
                booking.UserId = _userId;
                booking.CreatedOn = DateTime.Now;
                booking.Status = "Pending";
                booking.IsDeleted = false;
                booking.IsPay = false;
                booking.IsRequest = false;
                booking.IsResponse = false;
                booking.IsRideCounted = false;
                if((booking.RecieveOn - DateTime.Now).TotalHours >= 72)
                {
                    if (booking.IsRequireDriver)
                    {
                        booking.HasDriver = post.HasDriver;
                    }
                    else
                    {
                        booking.HasDriver = true;
                    }    
                }    
                else
                {
                    booking.HasDriver = true;
                }    
                await _bookingRepository.AddAsync(booking);
                var notify = new Notify()
                {
                    BookingId = booking.Id,
                    UserId = post.UserId,
                    CreateOn = DateTime.Now,
                    IsRead = false,
                    IsDeleted = false,
                    Content = "You have new booking request"
                };
                await _notifyService.AddAsync(notify);
                if(NotifyHub.userConnectionsDic.TryGetValue(post.UserId!, out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "System", "You have new booking request");
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

        public async Task UpdateAsync(int id, Booking booking)
        {
            try
            {
                var existingBooking = await _bookingRepository.GetByIdAsync(id);
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
                await _bookingRepository.UpdateAsync(booking);
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

        public async Task UpdateOwnerConfirmAsync(int id, bool isAccept)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                if (booking.Post.UserId != _userId)
                {
                    throw new UnauthorizedAccessException("Unauthorize");
                }
                booking.ModifiedById = _userId;
                booking.ModifiedOn = DateTime.Now;
                booking.Status = isAccept ? "Accept Booking" : "Denied";
                booking.OwnerConfirm = isAccept;
                await _bookingRepository.UpdateAsync(booking);
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
                    await _driverService.SendNotifyToDrivers(booking);
                    notify.Content = "Your booking confirmed by owner";
                }
                else
                {
                    notify.Content = "Your booking has been denied";
                }
                await _notifyService.AddAsync(notify);
                if (NotifyHub.userConnectionsDic.TryGetValue(booking.UserId!, out var connectionId))
                {
                    //await _hubContext.Groups.AddToGroupAsync(connectionId, booking.UserId!);
                    //await _hubContext.Clients.Client(connectionId).SendAsync("RecieveMessage", "System", isAccept ? "Booking accepted" : "Booking denied");
                    //await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", "Booking");
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", "System", isAccept ? "Your booking confirmed by owner" : "Your booking has been denied");
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

        public async Task UpdateBookingStatus()
        {
            try
            {
                var bookings = await _bookingRepository.GetAllAsync();
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
                    else if(!booking.IsPay && booking.RecieveOn <= DateTime.Now)
                    {
                        booking.Status = "Canceled";
                    }
                    await _bookingRepository.UpdateAsync(booking);
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

        public async Task DeleteAsync(int id)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                booking.IsDeleted = true;
                await _bookingRepository.UpdateAsync(booking);
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

        public async Task RequestCancelBookingAsync(int id)
        {
            try
            {
                var existingBooking = await _bookingRepository.GetByIdAsync(id);
                if (existingBooking.UserId != _userId)
                {
                    throw new UnauthorizedAccessException("Unauthorized");
                }
                existingBooking.ModifiedById = _userId;
                existingBooking.ModifiedOn = DateTime.Now;
                if (existingBooking.HasDriver)
                {
                    existingBooking.IsRequest = true;
                    existingBooking.Status = "Processing";
                }
                else
                {
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
                }    
                await _bookingRepository.UpdateAsync(existingBooking);
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
                    booking.Status = "Request denied";
                    notify.Content = "Your cancellation request has been denied";
                }    
                booking.IsResponse = true;
                booking.ModifiedById = _userId;
                booking.ModifiedOn = DateTime.Now;
                await _bookingRepository.UpdateAsync(booking);
                await _notifyService.AddAsync(notify);
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
        public async Task CancelReportedBookingsAsync(Booking booking)
        {
            try
            {
                booking.Status = booking.IsPay ? "Refunded" : "Canceled";
                booking.IsResponse = true;
                await _bookingRepository.UpdateAsync(booking);
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
