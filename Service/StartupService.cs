using AutoMapper;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Service
{
    public class StartupService
    {
        private readonly BookingService _bookingService;
        private readonly PostService _postService;
        private readonly IMapper _mapper;

        public StartupService(BookingService bookingService, PostService postService, IMapper mapper)
        {
            _bookingService = bookingService;
            _postService = postService;
            _mapper = mapper;
        }

        public async Task UpdateBookingsOnStartup()
        {
            try
            {
                await _bookingService.UpdateBookingStatus();
            }
            catch(NullReferenceException nullEx)
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

        public async Task UpdatePostOnStartup()
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync();
                foreach (var booking in bookings)
                {
                    if (booking.RecieveOn <= DateTime.Now && booking.IsPay && !booking.IsRideCounted)
                    {
                        await _postService.UpdateRideNumberAsync(booking.PostId, 1);
                        booking.IsRideCounted = true;
                        await _bookingService.UpdateAsync(booking.Id, booking);
                    }
                }
            }
            catch (NullReferenceException)
            {
                return;
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
