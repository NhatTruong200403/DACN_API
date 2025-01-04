using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Service.Interface
{
    public interface IBookingService
    {
        List<DateTime> GetBookedDateByPostId(int postId);
        List<Booking> GetAllWaitingBookingsByPostId(int postId);
        List<Booking> GetAllPendingBookingsByUserId();
        List<Booking> GetAll();
        List<Booking> GetAllCompleteBooking();
        List<Booking> GetAllCancelRequest();
        List<Booking> GetPersonalBookings();
        List<Booking> GetAllByAdmin();
        Booking GetById(int id);
        bool CheckBookingValue(BookingDTO bookingDTO, decimal promotionValue);
        Task Add(Booking booking);
        void Update(int id, Booking booking);
        Task AddDriverToBookingAsync(int bookingId);
        Task RemoveDriverFromBookingAsync(int bookingId);
        Task ConfirmBooking(int id, bool isAccept);
        void UpdateBookingStatus();
        void Delete(int id);
        void RequestCancelBooking(int id);
        Task ExamineCancelBookingRequestAsync(Booking booking, bool isAccept);
        void CancelReportedBookings(Booking booking);

    }
}
