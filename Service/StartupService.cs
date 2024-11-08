using AutoMapper;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GoWheels_WebAPI.Service
{
    public class StartupService
    {
        private readonly BookingService _bookingService;
        private readonly PostService _postService;
        private readonly ILogger<StartupService> _logger;
        private readonly IMapper _mapper;

        public StartupService(BookingService bookingService, PostService postService, ILogger<StartupService> logger, IMapper mapper)
        {
            _bookingService = bookingService;
            _postService = postService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task UpdateBookingsOnStartup()
        {
            try
            {
                await _bookingService.UpdateBookingStatus();
            }
            catch (InvalidOperationException operationEx)
            {
                Debug.WriteLine("An error occurred: " + operationEx.Message);
                Debug.WriteLine("Stack Trace: " + operationEx.StackTrace);
            }
            catch (NullReferenceException nullEx)
            {
                Debug.WriteLine("An error occurred: " + nullEx.Message);
                Debug.WriteLine("Stack Trace: " + nullEx.StackTrace);
            }
            catch (DbUpdateException dbEx)
            {
                Debug.WriteLine("An error occurred: " + dbEx.Message);
                Debug.WriteLine("Stack Trace: " + dbEx.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred: " + ex.Message);
                Debug.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }

        public async Task UpdatePostOnStartup()
        {
            var bookings = await _bookingService.GetAllCompleteBookingAsync();
            foreach (var booking in bookings)
            {
                try
                {
                    if (!booking.IsRideCounted)
                    {
                        await _postService.UpdateRideNumberAsync(booking.PostId, 1);
                        booking.IsRideCounted = true;
                        await _bookingService.UpdateAsync(booking.Id, booking);
                    }
                }
                catch (InvalidOperationException operationEx)
                {
                    Debug.WriteLine("An error occurred: " + operationEx.Message);
                    Debug.WriteLine("Stack Trace: " + operationEx.StackTrace);
                }
                catch (NullReferenceException nullEx)
                {
                    Debug.WriteLine("An error occurred: " + nullEx.Message);
                    Debug.WriteLine("Stack Trace: " + nullEx.StackTrace);
                }
                catch (DbUpdateException dbEx)
                {
                    Debug.WriteLine("An error occurred: " + dbEx.Message);
                    Debug.WriteLine("Stack Trace: " + dbEx.StackTrace);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred: " + ex.Message);
                    Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                }
            }
            
        }
    }
}
