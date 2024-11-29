using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class DriverBookingRepository : IGenericRepository<DriverBooking>
    {
        private readonly ApplicationDbContext _context;

        public DriverBookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<List<DriverBooking>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<DriverBooking>> GetAllByUserIdAsync(string userId)
            => await _context.DriverBookings.Include(d => d.Driver)
                                            .Include(d => d.Invoices)
                                            .ThenInclude(i => i.Booking)
                                            .ThenInclude(i => i.Post)
                                            .Where(d => !d.IsDeleted && d.Driver.UserId == userId)
                                            .ToListAsync();

        public async Task<DriverBooking> GetByIdAsync(int id)
            => await _context.DriverBookings.Include(d => d.Driver)
                                            .Include(d => d.Invoices)
                                            .ThenInclude(i => i.Booking)
                                            .FirstOrDefaultAsync(d => !d.IsCancel && !d.IsDeleted && d.Id == id) 
                                            ?? throw new NullReferenceException("Driver booking not found");


        public async Task<DriverBooking> GetByBookingIdAsync(int id)
            => await _context.DriverBookings.Include(d => d.Driver)
                                            .Include(d => d.Invoices)
                                            .ThenInclude(i => i.Booking)
                                            .FirstOrDefaultAsync(db => db.Invoices.Any(inv => inv.BookingId == id))
                                            ?? throw new NullReferenceException("Driver booking not found");

        public async Task AddAsync(DriverBooking driverBooking)
        {
            await _context.DriverBookings.AddAsync(driverBooking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DriverBooking driverBooking)
        {
            _context.Entry(driverBooking).State = EntityState.Modified;
            driverBooking.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DriverBooking driverBooking)
        {
            var existingDriverBooking = _context.ChangeTracker.Entries<DriverBooking>()
                                                 .FirstOrDefault(e => e.Entity.Id == driverBooking.Id);

            //detached same id obj
            if (existingDriverBooking != null)
            {
                _context.Entry(existingDriverBooking.Entity).State = EntityState.Detached;
            }

            _context.Entry(driverBooking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            //detached tracking obj after modified
            _context.Entry(driverBooking).State = EntityState.Detached;
        }
    }
}
