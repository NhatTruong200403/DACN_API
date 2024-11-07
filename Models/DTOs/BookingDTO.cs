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
        [JsonIgnore]
        public string? Status { get; set; }
        [JsonIgnore]
        public bool IsRequest { get; set; }
        [JsonIgnore]
        public bool IsResponse { get; set; }
        [JsonIgnore]
        public bool IsPay {  get; set; }
        public int PostId { get; set; }
        public int? PromotionId { get; set; }
    }
}
