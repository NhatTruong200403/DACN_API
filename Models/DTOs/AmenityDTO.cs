using GoWheels_WebAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class AmenityDTO
    {
        public int? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public IFormFile? IconImage { get; set; } = null!;
    }
}
