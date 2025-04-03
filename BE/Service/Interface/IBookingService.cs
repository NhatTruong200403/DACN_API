using GoWheels_WebAPI.Models.DTOs;
using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Service.Interface
{
    public interface IBookingService
    {
        List<Booking> GetAllUnRecieveBookingsByPostId(int postId);
        List<DateTime> GetBookedDateByPostId(int postId);
        List<Booking> GetAllWaitingBookingsByPostId(int postId);
        Task<List<Booking>> GetAllDriverRequireBookingsAsync(string latitude, string longitude);
        List<Booking> GetAllPendingBookingsByUserId();
        List<Booking> GetAll();
        List<Booking> GetAllCompleteBooking();
        List<Booking> GetAllCancelRequest();
        List<Booking> GetPersonalBookings();
        List<Booking> GetAllByDriver();
        List<Booking> GetAllByOwner();
        Task<List<Booking>> GetAllBookingsInRange(string latitude, string longitude);
        Booking GetById(int id);
        bool CheckBookingValue(BookingDTO bookingDTO, decimal promotionValue);
        void Add(Booking booking);
        void Update(int id, Booking booking);
        Task AddDriverToBookingAsync(int bookingId);
        Task RemoveDriverFromBookingAsync(int bookingId);
        void UpdateOwnerConfirm(int id, bool isAccept);
        void UpdateBookingStatus();
        void Delete(int id);
        void RequestCancelBooking(int id);
        Task ExamineCancelBookingRequestAsync(Booking booking, bool isAccept);
        void CancelReportedBookings(Booking booking);

    }
}
