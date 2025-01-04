using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Diagnostics.CodeAnalysis;

namespace GoWheels_WebAPI.Models.Entities
{
    public class Booking : BaseModel
    {
        public required decimal PrePayment { get; set; }
        public required decimal Total { get; set; }
        public required decimal FinalValue { get; set; }
        public required DateTime RecieveOn { get; set; }
        public required DateTime ReturnOn { get; set; }
        public required string Status { get; set; }
        public required bool OwnerConfirm { get; set; }
        public bool IsPay { get; set; } = false;
        public bool IsRequest { get; set; } = false;
        public bool IsResponse { get; set; } = false;
        public bool IsRideCounted { get; set; } = false;
        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; } = null!;
        public string? UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; } = null!;
        [AllowNull]
        public int? PromotionId { get; set; }
        [ValidateNever]
        public Promotion? Promotion { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
