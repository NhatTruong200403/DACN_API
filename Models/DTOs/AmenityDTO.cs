using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class AmenityDTO : BaseModelDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? IconImage { get; set; }
    }
}
