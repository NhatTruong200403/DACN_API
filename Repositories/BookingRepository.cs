using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GoWheels_WebAPI.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Booking booking)
        {
            _context.Add(booking);
            _context.SaveChanges();
        }

        public void Delete(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            booking.IsDeleted = true;
            _context.SaveChanges();
        }

        public List<Booking> GetAll()
            => _context.Bookings.AsNoTracking().Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Where(b => !b.IsDeleted)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllDriverRequireBookings()
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Where(b => !b.IsDeleted
                                                    && b.OwnerConfirm
                                                    && b.Status.Equals("Accept Booking")
                                                    && b.IsRequireDriver
                                                    && !b.HasDriver)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllByDriver(string userId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Where(b => !b.IsDeleted
                                                    && b.OwnerConfirm)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();


        public List<Booking> GetAllByPostId(int postId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Where(b => !b.IsDeleted
                                                        && b.PostId == postId
                                                        && b.OwnerConfirm
                                                        && b.RecieveOn > DateTime.Now)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllByAdmin(string userId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Where(b => !b.IsDeleted
                                                        && b.OwnerConfirm
                                                        && b.RecieveOn > DateTime.Now)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllPersonalBookings(string userId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Where(b => b.UserId == userId)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllCancelRequest()
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.Promotion)
                                        .Include(b => b.User)
                                        .Where(b => b.IsRequest && !b.IsResponse)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllUnRecieveBookingByPostId(int postId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Include(b => b.Promotion)
                                        .Where(b => b.PostId == postId && b.RecieveOn > DateTime.Now && !b.IsResponse)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllWaitingBookingByPostId(int postId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.PostId == postId && b.Status == "Waiting")
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllPendingBookingByUserId(string userId)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Include(b => b.Promotion)
                                        .Where(b => b.UserId == userId && b.Status == "Pending" && !b.IsPay)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();

        public List<Booking> GetAllCompleteBookings()
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.RecieveOn <= DateTime.Now && b.IsPay)
                                        .OrderByDescending(b => b.CreatedOn)
                                        .ToList();


        public Booking GetById(int id)
            => _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.Post)
                                        .ThenInclude(b => b.Images)
                                        .Include(b => b.Promotion)
                                        .Include(b => b.User)
                                        .FirstOrDefault(b => b.Id == id && !b.IsDeleted)
                                        ?? throw new NullReferenceException("Booking not found");

        public void Update(Booking booking)
        {
            var trackedBooking = _context.Bookings.Local.FirstOrDefault(b => b.Id == booking.Id);

            if (trackedBooking != null)
            {
                _context.Entry(trackedBooking).State = EntityState.Detached;
            }

            _context.Entry(booking).State = EntityState.Modified;

            _context.SaveChanges();
        }


    }
}
