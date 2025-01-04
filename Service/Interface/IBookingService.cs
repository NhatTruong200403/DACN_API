using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Service.Interface
{
    public interface IBookingService
    {
        List<DateTime> GetBookedDateByPostId(int postId);
        List<Booking> GetAllPendingBookings();
        List<Booking> GetAll();
        List<Booking> GetAllCompleteBooking();
        List<Booking> GetAllCancelRequest();
        List<Booking> GetPersonalBookings();
        Booking GetById(int id);
        bool CheckBookingValue(BookingDTO bookingDTO, decimal promotionValue);
        void Add(Booking booking);
        void Update(int id, Booking booking);
        Task ConfirmBooking(int id, bool isAccept);
        void UpdateBookingStatus();
        void Delete(int id);
        void RequestCancelBooking(int id);
        Task ExamineCancelBookingRequestAsync(Booking booking, bool isAccept);
        void CancelReportedBookings(Booking booking);

    }
}
