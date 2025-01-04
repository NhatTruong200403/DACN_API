using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public List<IFormFile?> ImagesList { get; set; } = new List<IFormFile?>();
        public string? Description { get; set; }
        [Required]
        public int Seat { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        [Required]
        public int CarTypeId { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }
}
