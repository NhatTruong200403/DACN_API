using GoWheels_WebAPI.Data;
using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Invoice> GetAll()
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .ThenInclude(b => b.User)
                                        .Include(i => i.DriverBooking)
                                        .ThenInclude(i => i.Driver)
                                        .ThenInclude(i => i.User)
                                        .OrderByDescending(i => i.CreatedOn)
                                        .ToList();

        public List<Invoice> GetAllByUserId(string userId)
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .ThenInclude(i => i.Post)
                                        .ThenInclude(b => b.User)
                                        .Include(i => i.DriverBooking)
                                        .ThenInclude(i => i.Driver)
                                        .ThenInclude(i => i.User)
                                        .Where(i => i.Booking.UserId == userId)
                                        .OrderByDescending(i => i.CreatedOn)
                                        .ToList();

        public List<Invoice> GetAllByDriver(string userId)
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .ThenInclude(b => b.User)
                                        .Include(i => i.DriverBooking)
                                        .ThenInclude(i => i.Driver)
                                        .ThenInclude(i => i.User)
                                        .Where(i => i.DriverBooking.Driver.UserId == userId)
                                        .OrderByDescending(i => i.CreatedOn)
                                        .ToList();

        public List<Invoice> GetAllRefundInvoices()
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .Include(i => i.DriverBooking)
                                        .ThenInclude(i => i.Driver)
                                        .ThenInclude(i => i.User)
                                        .Where(i => i.RefundInvoice)
                                        .OrderByDescending(i => i.CreatedOn)
                                        .ToList();

        public Invoice GetById(int id)
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .ThenInclude(b => b.User)
                                        .Include(i => i.DriverBooking)
                                        .ThenInclude(d => d.Driver)
                                        .ThenInclude(d => d.User)
                                        .FirstOrDefault(i => i.Id == id)
                                        ?? throw new NullReferenceException("Invoice not found");


        public Invoice GetByBookingId(int bookingId)
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .ThenInclude(b => b.User)
                                        .Include(i => i.DriverBooking)
                                        .ThenInclude(d => d.Driver)
                                        .ThenInclude(d => d.User)
                                        .FirstOrDefault(i => i.BookingId == bookingId)
                                        ?? throw new NullReferenceException("Invoice not found");

        public Invoice GetByDriverBookingId(int driverBooking)
            => _context.Invoices.AsNoTracking()
                                        .Include(i => i.Booking)
                                        .ThenInclude(b => b.User)
                                        .Include(i => i.DriverBooking)
                                        .FirstOrDefault(i => i.DriverBookingId == driverBooking)
                                        ?? throw new NullReferenceException("Invoice not found");

        public void Add(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
        }

        public void Update(Invoice invoice)
        {
            var existingInvoice = _context.Invoices.Local.FirstOrDefault(b => b.Id == invoice.Id);

            if (existingInvoice != null)
            {
                _context.Entry(existingInvoice).State = EntityState.Detached;
            }

            // Gán lại trạng thái cho đối tượng là modified và lưu các thay đổi
            _context.Entry(invoice).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Invoice invoice)
        {
            _context.Entry(invoice).State = EntityState.Modified;
            invoice.IsDeleted = true;
            _context.SaveChanges();
        }


    }
}
