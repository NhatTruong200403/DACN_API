using GoWheels_WebAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class PromotionDTO
    {
        public int? Id { get; set; }

        [Required]
        public required string Content { get; set; }
        [Required]
        public decimal DiscountValue { get; set; }
        [Required]
        public required DateTime ExpiredDate { get; set; }
    }
}
