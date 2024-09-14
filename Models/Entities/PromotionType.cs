using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.Entities
{
    public class PromotionType : BaseModel
    {
        [Required]
        public required string Name { get; set; }
        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
