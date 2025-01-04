using GoWheels_WebAPI.Models.Entities;
using GoWheels_WebAPI.Models.ViewModels;
using GoWheels_WebAPI.Payment;
using GoWheels_WebAPI.Repositories.Interface;
using GoWheels_WebAPI.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace GoWheels_WebAPI.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBookingService _bookingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userId;
        private readonly IConfiguration _configuration;
        public InvoiceService(IInvoiceRepository invoiceRepository,
                                IBookingService bookingService,
                                IHttpContextAccessor httpContextAccessor,
                                IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _bookingService = bookingService;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext?.User?
                     .FindFirstValue(ClaimTypes.NameIdentifier) ?? "UnknownUser";
            _configuration = configuration;
        }

        public List<Invoice> GetAll()
            => _invoiceRepository.GetAll();

        public List<Invoice> GetPersonalInvoices()
            => _invoiceRepository.GetAllByUserId(_userId);

        public Invoice GetById(int id)
            => _invoiceRepository.GetById(id);


        public Invoice GetByBookingId(int bookingId)
            => _invoiceRepository.GetByBookingId(bookingId);


        public List<Invoice> GetAllRefundInvoices()
            => _invoiceRepository.GetAllRefundInvoices();

        public void CreateInvoice(int bookingId)
        {
            try
            {
                var booking = _bookingService.GetById(bookingId);
                var invoice = new Invoice()
                {
                    PrePayment = booking.PrePayment,
                    Total = booking.FinalValue,
                    ReturnOn = booking.RecieveOn.AddDays(2),
                    BookingId = booking.Id,
                    CreatedById = booking.UserId,
                    CreatedOn = DateTime.Now,
                    RefundInvoice = false,
                };
                _invoiceRepository.Add(invoice);
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

        public void Update(Invoice invoice)
        {
            try
            {
                invoice.ModifiedById = _userId;
                invoice.ModifiedOn = DateTime.Now;
                _invoiceRepository.Update(invoice);
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

        public void Refund(Booking booking, bool isAccept)
        {
            try
            {
                if (isAccept)
                {
                    if (booking.IsPay)
                    {
                        var invoice = _invoiceRepository.GetByBookingId(booking.Id);
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
                        _invoiceRepository.Update(invoice);
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

        public void RefundReportedBooking(Booking booking)
        {
            try
            {
                var invoice = _invoiceRepository.GetByBookingId(booking.Id);
                invoice.ModifiedById = _userId;
                invoice.ModifiedOn = DateTime.Now;
                invoice.RefundInvoice = true!;
                _invoiceRepository.Update(invoice);
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

        public async Task<string> ProcessMomoPayment(Booking booking, bool isMobile)
        {
            long price = (long)booking.PrePayment;
            string priceStr = price.ToString();
            string bookingIdStr = booking.Id.ToString();

            string endpoint = _configuration.GetValue<string>("MomoAPI:MomoApiUrl") ?? string.Empty;
            string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey") ?? string.Empty;
            string accessKey = _configuration.GetValue<string>("MomoAPI:AccessKey") ?? string.Empty;
            string returnUrl = _configuration.GetValue<string>("MomoAPI:ReturnUrl") ?? string.Empty;
            if (isMobile)
            {
                returnUrl = _configuration.GetValue<string>("MomoAPI:MobileReturnUrl") ?? string.Empty;
            }
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

            //string momoUrl = $"momo://app?action=pay" +
            //         $"&partnerCode={partnerCode}" +
            //         $"&accessKey={accessKey}" +
            //         $"&requestId={requestId}" +
            //         $"&amount={amount}" +
            //         $"&orderId={orderid}" +
            //         $"&orderInfo={Uri.EscapeDataString(orderInfo)}" +
            //         $"&returnUrl={Uri.EscapeDataString(returnUrl)}" +
            //         $"&notifyUrl={Uri.EscapeDataString(notifyUrl)}" +
            //         $"&extraData={Uri.EscapeDataString(extraData)}" +
            //         $"&signature={Uri.EscapeDataString(signature)}";
            //return momoUrl;

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

        public void ProcessReturnUrl(IQueryCollection queryParams)
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

                var booking = _bookingService.GetById(bookingIdNum);
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
                _bookingService.Update(booking.Id, booking);

                string amountStr = queryParams["amount"].ToString();
                if (!decimal.TryParse(amountStr, out decimal amount))
                {
                    throw new InvalidOperationException("Invalid invoice ID"); ;
                }
                var invoice = new Invoice() 
                { 
                    CreatedById = _userId,
                    CreatedOn = DateTime.Now,
                    PrePayment = amount,
                    Total = amount,
                    ReturnOn = booking.ReturnOn.AddDays(1),
                    BookingId = booking.Id,
                };
                _invoiceRepository.Add(invoice);
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

        /*public List<MonthlyRevenueVM> CalculateRevenuesByMonth(int year)
        {
            var invoices = _invoiceRepository.GetAllByPostOwner(_userId);
            var monthsRevenues = new List<MonthlyRevenueVM>();
            for (int i = 0; i <= 11; i++)
            {
                var monthRevenue = invoices.Where(inv =>inv.CreatedOn.Year == year && inv.CreatedOn.Month == (i+1)).Sum(i => i.Total);
                monthsRevenues.Add(new MonthlyRevenueVM()
                {
                    Month = i + 1,
                    Revenue = monthRevenue
                });
            }
            return monthsRevenues;
        }*/
    }
}
