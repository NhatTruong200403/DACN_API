using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWheels_WebAPI.Models.Entities
{
    public class Promotion : BaseModel
    {
        [Required]
        public required string Content { get; set; }
        [Required]
        [Range(0.01, 1, ErrorMessage = "Discount value must be between 0.01 and 1.")]
        public decimal DiscountValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        [Required]
        public required DateTime ExpiredDate { get; set; }

        [Required]
        [ForeignKey(nameof(PromotionType))]
        public int PromotionId { get; set; }

        [ValidateNever]
        public PromotionType PromotionType { get; set; } = null!;
    }
}
