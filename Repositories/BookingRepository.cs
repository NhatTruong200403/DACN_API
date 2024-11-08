using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GoWheels_WebAPI.Repositories
{
    public class BookingRepository : IGenericRepository<Booking>
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            booking.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetAllAsync()
            => await _context.Bookings.AsNoTracking().Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Where(b => !b.IsDeleted).ToListAsync();

        public async Task<List<Booking>> GetAllByPostIdAsync(int postId)
                        => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.User)
                                        .Include(b => b.Post)
                                        .Where(b => !b.IsDeleted && b.PostId == postId && b.IsPay && b.OwnerConfirm && b.RecieveOn > DateTime.Now)
                                        .ToListAsync();

        public async Task<List<Booking>> GetAllPersonalBookingsAsync(string userId)
           => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.UserId == userId)
                                        .ToListAsync();

        public async Task<List<Booking>> GetAllCancelRequestAsync()
            => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.IsRequest && !b.IsResponse)
                                        .ToListAsync();

        public async Task<List<Booking>> GetAllUnRecieveBookingByPostIdAsync(int postId)
            => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.PostId == postId && b.RecieveOn > DateTime.Now && !b.IsResponse)
                                        .ToListAsync();

        public async Task<List<Booking>> GetAllWaitingBookingByPostIdAsync(int postId)
            => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.PostId == postId && b.Status == "Waiting")
                                        .ToListAsync();

        public async Task<List<Booking>> GetAllPendingBookingByUserIdAsync(string userId)
            => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .Where(b => b.Post.UserId == userId && b.Status == "Pending" && !b.IsPay)
                                        .ToListAsync();

        public async Task<Booking> GetByIdAsync(int id)
            => await _context.Bookings.AsNoTracking()
                                        .Include(b => b.Post)
                                        .Include(b => b.User)
                                        .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted)
                                        ?? throw new NullReferenceException("Booking not found");

        public async Task UpdateAsync(Booking booking)
        {
            var trackedBooking = _context.Bookings.Local.FirstOrDefault(b => b.Id == booking.Id);

            if (trackedBooking != null)
            {
                _context.Entry(trackedBooking).State = EntityState.Detached;
            }

            _context.Entry(booking).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }


    }
}
