using GoWheels_WebAPI.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GoWheels_WebAPI.Service
{
    public class StartupService : IStartupService   
    {
        private readonly IBookingService _bookingService;
        private readonly IPostService _postService;

        public StartupService(IBookingService bookingService,
                                IPostService postService)
        {
            _bookingService = bookingService;
            _postService = postService;
        }

        public void UpdateBookingsOnStartup()
        {
            try
            {
                _bookingService.UpdateBookingStatus();
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

        private void UpdatePostRideNumber()
        {
            var bookings = _bookingService.GetAllCompleteBooking();
            foreach (var booking in bookings)
            {

                if (!booking.IsRideCounted)
                {
                    _postService.UpdateRideNumber(booking.PostId, 1);
                    booking.IsRideCounted = true;
                    _bookingService.Update(booking.Id, booking);
                }
            }
        }

        public void UpdatePostOnStartup()
        {
            try
            {
                UpdatePostRideNumber();

            }
            catch (InvalidOperationException operationEx)
            {
                Console.WriteLine("An error occurred: " + operationEx.Message);
                Console.WriteLine("Stack Trace: " + operationEx.StackTrace);
            }
            catch (NullReferenceException nullEx)
            {
                Console.WriteLine("An error occurred: " + nullEx.Message);
                Console.WriteLine("Stack Trace: " + nullEx.StackTrace);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("An error occurred: " + dbEx.Message);
                Console.WriteLine("Stack Trace: " + dbEx.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }
    }
}
