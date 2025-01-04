using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public decimal PrePayment { get; set; }
        public decimal Total { get; set; }
        public decimal FinalValue { get; set; }
        public DateTime RecieveOn { get; set; }
        public DateTime ReturnOn { get; set; }
        public int PostId { get; set; }

        [AllowNull]
        public int? PromotionId { get; set; }
        public decimal DiscountValue { get; set; } 
    }
}
