using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Repositories.Interface
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        List<Booking> GetAllByPostId(int postId);
        List<Booking> GetAllPersonalBookings(string userId);
        List<Booking> GetAllCancelRequest();
        List<Booking> GetAllPendingBooking();
        List<Booking> GetAllCompleteBookings();

    }
}
