using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class PromotionDTO : BaseModelDTO
    {
        [Required]  
        public string? Content { get; set; }
        [Required]
        [Range(0.01, 1, ErrorMessage = "Discount value must be between 0.01 and 1.")]
        public decimal? DisCountValue { get; set; }
        [Required]
        public int PromotionId { get; set; }
        public string? PromotionName { get; set; }
    }
}
