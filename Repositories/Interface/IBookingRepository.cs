using GoWheels_WebAPI.Models.Entities;

namespace GoWheels_WebAPI.Repositories.Interface
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        List<Booking> GetAllDriverRequireBookings();
        List<Booking> GetAllByDriver(string userId);
        List<Booking> GetAllByPostId(int postId);
        List<Booking> GetAllPersonalBookings(string userId);
        List<Booking> GetAllCancelRequest();
        List<Booking> GetAllUnRecieveBookingByPostId(int postId);
        List<Booking> GetAllWaitingBookingByPostId(int postId);
        List<Booking> GetAllPendingBookingByUserId(string userId);
        List<Booking> GetAllCompleteBookings();

    }
}
