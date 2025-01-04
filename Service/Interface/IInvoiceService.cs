using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;

namespace GoWheels_WebAPI.Service.Interface
{
    public interface IInvoiceService
    {
        List<Invoice> GetAll();
        List<Invoice> GetPersonalInvoices();
        //List<Invoice> GetAllByDriver();
        Invoice GetById(int id);
        Invoice GetByBookingId(int bookingId);
        List<Invoice> GetAllRefundInvoices();
        void CreateInvoice(int bookingId);
        void Update(Invoice invoice);
        void Refund(Booking booking, bool isAccept);
        void RefundReportedBooking(Booking booking);
        Task<string> ProcessMomoPayment(Booking booking, bool isMobile);
        void ProcessReturnUrl(IQueryCollection queryParams);
        //Task CalculateRevenuesByYears
        //List<MonthlyRevenueVM> CalculateRevenuesByMonth(int year);
    }
}
