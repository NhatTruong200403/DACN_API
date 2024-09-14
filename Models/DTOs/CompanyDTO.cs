using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class CompanyDTO : BaseModelDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? IconImage { get; set; }
        public ICollection<CarTypeDetailDTO> CarTypeDetail { get; set; } = new List<CarTypeDetailDTO>();

    }
}
