using AutoMapper;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Repositories;
using GoWheels_WebAPI.Models.Entities;
using System.Security.Claims;
using GoWheels_WebAPI.Utilities;
using GoWheels_WebAPI.Payment;
using Newtonsoft.Json.Linq;
using GoWheels_WebAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GoWheels_WebAPI.Service
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly BookingService _bookingService;
        private readonly PostService _postService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IConfiguration _configuration;

        public InvoiceService(InvoiceRepository invoiceRepository,
                                BookingService bookingService,
                                PostService postService,
                                IMapper mapper,
                                IHttpContextAccessor httpContextAccessor,
                                IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _bookingService = bookingService;
            _postService = postService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                     .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _configuration = configuration;
        }

        public async Task<List<Invoice>> GetAllAsync()
            => await _invoiceRepository.GetAllAsync();

        public async Task<List<Invoice>> GetPersonalInvoicesAsync()
            => await _invoiceRepository.GetAllByUserIdAsync(_userId);

        public async Task<List<Invoice>> GetAllByDriverAsync()
            => await _invoiceRepository.GetAllByDriverAsync(_userId);

        public async Task<Invoice> GetByIdAsync(int id)
            => await _invoiceRepository.GetByIdAsync(id);


        public async Task<Invoice> GetByBookingIdAsync(int bookingId)
            => await _invoiceRepository.GetByBookingIdAsync(bookingId);

        public async Task<Invoice> GetByDriverBookingIdAsync(int driverBookingId)
            => await _invoiceRepository.GetByDriverBookingIdAsync(driverBookingId);

        public async Task<List<Invoice>> GetAllRefundInvoicesAsync()
            => await _invoiceRepository.GetAllRefundInvoicesAsync();

        public async Task CreateInvoiceAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(bookingId);
                var invoice = new Invoice()
                {
                    PrePayment = booking.PrePayment,
                    Total = booking.FinalValue,
                    ReturnOn = booking.RecieveOn.AddDays(2),
                    BookingId = booking.Id,
                    CreatedById = booking.UserId,
                    CreatedOn = DateTime.Now,
                    IsPay = false,
                    RefundInvoice = false,
                };
                await _invoiceRepository.AddAsync(invoice);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddDriverToInvocieAsync(Booking booking, DriverBooking driverBooking)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByBookingIdAsync(booking.Id);
                invoice.DriverBooking = driverBooking;
                invoice.Total += driverBooking.Total;
                await _invoiceRepository.UpdateAsync(invoice);
                booking.HasDriver = true;
                await _bookingService.UpdateAsync(booking.Id, booking);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCancelDriverBookingAsync(Invoice invoice, decimal driverBookingTotal)
        {
            try
            {
                invoice.ModifiedById = _userId;
                invoice.ModifiedOn = DateTime.Now;
                invoice.Total -= driverBookingTotal;
                invoice.DriverBookingId = null!;
                invoice.DriverBooking = null!;
                await _invoiceRepository.UpdateAsync(invoice);
            }
            catch (NullReferenceException nullEx)
            {
                throw new NullReferenceException(nullEx.InnerException!.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            try
            {
                invoice.ModifiedById = _userId;
                invoice.ModifiedOn = DateTime.Now;  
                await _invoiceRepository.UpdateAsync(invoice);
            }
            catch(NullReferenceException nullEx) 
            {
                 throw new NullReferenceException(nullEx.InnerException!.Message);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task RefundAsync(Booking booking, bool isAccept)
        {
            try
            {
                if (isAccept)
                {
                    if(booking.IsPay)
                    {
                        var invoice = await _invoiceRepository.GetByBookingIdAsync(booking.Id);
                        var refundValue = (double)invoice!.PrePayment;
                        var refundHours = (booking.RecieveOn - DateTime.Now).TotalHours;
                        var createHours = (booking.CreatedOn - DateTime.Now).TotalHours;
                        if (refundHours > 168 && createHours > 1)
                        {
                            refundValue = refundValue * 0.7;
                        }
                        else if (refundHours <= 168 && createHours > 1)
                        {
                            refundValue = 0;
                        }
                        else if (createHours == 1)
                        {
                            refundValue = (double)invoice.PrePayment;
                        }
                        invoice.PrePayment = (decimal)refundValue;
                        invoice.ReturnOn = DateTime.Now;
                        invoice.RefundInvoice = true;
                        invoice.ModifiedOn = DateTime.Now;
                        invoice.ModifiedById = _userId;
                        await _invoiceRepository.UpdateAsync(invoice);
                    }                  
                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task RefundReportedBookingAsync(Booking booking)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByBookingIdAsync(booking.Id);
                invoice.ModifiedById = _userId;
                invoice.ModifiedOn = DateTime.Now;
                invoice.DriverBookingId = null!;
                invoice.DriverBooking = null!;
                invoice.RefundInvoice = true!;
                await _invoiceRepository.UpdateAsync(invoice);
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> ProcessMomoPaymentAsync(Invoice  invoice)
        {
            double price = (double)invoice.PrePayment;
            string priceStr = price.ToString();
            string bookingIdStr = invoice.BookingId.ToString();

            string endpoint = _configuration.GetValue<string>("MomoAPI:MomoApiUrl") ?? string.Empty;
            string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey") ?? string.Empty;
            string accessKey = _configuration.GetValue<string>("MomoAPI:AccessKey") ?? string.Empty;
            string returnUrl = _configuration.GetValue<string>("MomoAPI:ReturnUrl") ?? string.Empty;
            string notifyUrl = _configuration.GetValue<string>("MomoAPI:NotifyUrl") ?? string.Empty;
            string partnerCode = _configuration.GetValue<string>("MomoAPI:PartnerCode") ?? string.Empty;

            string orderInfo = _httpContextAccessor.HttpContext?.User?
                     .FindFirstValue(ClaimTypes.Name) ?? "UnknownUser" + " chuyển tiền";
            string amount = priceStr;
            string orderid = DateTime.Now.Ticks.ToString(); // Order ID
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = bookingIdStr;
            // Create the signature
            string rawHash = $"partnerCode={partnerCode}&accessKey={accessKey}&requestId={requestId}&amount={amount}&orderId={orderid}&orderInfo={orderInfo}&returnUrl={returnUrl}&notifyUrl={notifyUrl}&extraData={extraData}";

            MomoSecurity crypto = new();
            string signature = crypto.signSHA256(rawHash, serectkey);

            // Build request JSON
            JObject message = new()
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyUrl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            // Send payment request through the repository
            var jsonResponse = await PaymentRequest.SendPaymentRequestAsync(endpoint, message.ToString());
            return jsonResponse;
        }

        public async Task ProcessReturnUrlAsync(IQueryCollection queryParams)
        {
            try
            {
                string param = "";
                param += "partnerCode=" + queryParams["partnerCode"];
                param += "&accessKey=" + queryParams["accessKey"];
                param += "&requestId=" + queryParams["requestId"];
                param += "&amount=" + queryParams["amount"];
                param += "&orderId=" + queryParams["orderId"];
                param += "&orderInfo=" + queryParams["orderInfo"];
                param += "&orderType=" + queryParams["orderType"];
                param += "&transId=" + queryParams["transId"];
                param += "&message=" + queryParams["message"];
                param += "&localMessage=" + queryParams["localMessage"];
                param += "&responseTime=" + queryParams["responseTime"];
                param += "&errorCode=" + queryParams["errorCode"];
                param += "&payType=" + queryParams["payType"];
                param += "&extraData=" + queryParams["extraData"];

                string bookingIdStr = queryParams["extraData"].ToString() ?? "";
                if (!int.TryParse(bookingIdStr, out int bookingIdNum))
                {
                    throw new InvalidOperationException("Invalid invoice ID"); ;
                }

                var booking = await _bookingService.GetByIdAsync(bookingIdNum);
                MomoSecurity crypto = new();
                string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey") ?? "";
                string signature = crypto.signSHA256(param, serectkey);

                if (signature != queryParams["signature"].ToString())
                {
                    throw new InvalidOperationException("Invalid request signature");
                }

                if (queryParams["errorCode"] != "0" || queryParams["errorCode"] == "1006" || queryParams["message"].ToString().Contains("Transaction denied by user"))
                {
                    throw new InvalidOperationException("Payment failed");
                }
                booking.IsPay = true;
                booking.Status = "Waiting";
                await _bookingService.UpdateAsync(booking.Id, booking);
                var invoice = await _invoiceRepository.GetByBookingIdAsync(bookingIdNum);
                invoice.ModifiedById = _userId;
                invoice.ModifiedOn = DateTime.Now;
                invoice.IsPay = true;
                await _invoiceRepository.UpdateAsync(invoice);
                bool isAvailable = booking.RecieveOn > DateTime.Now;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException(dbEx.InnerException!.Message);
            }
            catch (InvalidOperationException operationEx)
            {
                throw new InvalidOperationException(operationEx.InnerException!.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
