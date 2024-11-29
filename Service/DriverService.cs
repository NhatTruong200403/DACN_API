using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.GoogleRespone;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using GoWheels_WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace GoWheels_WebAPI.Service
{
    public class DriverService
    {
        private readonly DriverRepository _driverRepository;
        private readonly NotifyService _notifyService;
        private readonly GoogleApiService _googleApiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;

        public DriverService(DriverRepository driverRepository,
                                NotifyService notifyService,
                                IHttpContextAccessor httpContextAccessor,
                                GoogleApiService googleApiService)
        {
            _driverRepository = driverRepository;
            _notifyService = notifyService;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                        .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _googleApiService = googleApiService;
        }

        public async Task<List<Driver>> GetAllAsync()
            => await _driverRepository.GetAllAsync();

        public async Task<Driver> GetByIdAsync(int id)
            => await _driverRepository.GetByIdAsync(id);

        public async Task<Driver> GetByUserIdAsync(string userId)
            => await _driverRepository.GetByUserIdAsync(userId);

        public async Task AddAsync(ApplicationUser user)
        {
            try
            {
                var driver = new Driver()
                {
                    CreatedOn = DateTime.Now,
                    CreatedById = _userId,
                    UserId = user.Id,
                    PricePerHour = 20000,
                    RatingPoint = 0
                };
                await _driverRepository.AddAsync(driver);
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

        public async Task UpdateAsync(int id, Driver driver)
        {
            try
            {
                driver.ModifiedById = _userId;
                driver.ModifiedOn = DateTime.Now;
                await _driverRepository.UpdateAsync(driver);
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

        public async Task DeleteByIdAsync(int id)
        {
            try
            {
                var driver = await _driverRepository.GetByIdAsync(id);
                driver.ModifiedById = _userId;
                driver.ModifiedOn = DateTime.Now;
                driver.IsDeleted = true;
                await _driverRepository.UpdateAsync(driver);
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

        public async Task SendNotifyToDrivers(Booking booking)
        {
            try
            {
                var bookingLocationString = $"{booking.Latitude},{booking.Longitude}";
                var session = _httpContextAccessor.HttpContext!.Session;
                var userIds = session.Keys.ToList();
                var userLocations = new List<(string userId, string location)>();
                foreach (var userId in userIds)
                {
                    var userString = session.GetString(userId);
                    if (!userString.IsNullOrEmpty())
                    {
                        var user = JsonSerializer.Deserialize<UserVM>(userString!)!;
                        var userLocationString = $"{user.Latitude},{user.Longitude}";
                        userLocations.Add((userId, userLocationString));
                    }
                }
                var respone = await _googleApiService.GetDistanceAsync(userLocations, bookingLocationString);
                var usersInRange = GetUserWithinRange(respone, userLocations);
                foreach (var userId in usersInRange)
                {
                    var notify = new Notify()
                    {
                        Content = booking.Id.ToString(),
                        CreateOn = DateTime.Now,
                        Title = "New booking nearby",
                        IsRead = false,
                        UserId = userId,
                    };
                    await _notifyService.AddAsync(notify);
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

        private List<string> GetUserWithinRange(DistanceMatrixRespone distanceMatrixRespone, List<(string userId, string location)> userLocations)
        {
            var usersWithinRange = new List<string>();
            for(var i = 0; i < distanceMatrixRespone.Rows.Count; i++)
            {
                var distance = distanceMatrixRespone.Rows[i].Elements[0].Distance?.Value ?? int.MaxValue;
                if (distance < 10000)
                {
                    usersWithinRange.Add(userLocations[i].userId);
                }
            }    
            return usersWithinRange;
        }
    }
}