using GoWheels_WebAPI.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class CarTypeDTO : BaseModelDTO
    {
        [Required]
        public required string Name { get; set; }

        public ICollection<CarTypeDetailDTO> CarTypeDetail { get; set; } = new List<CarTypeDetailDTO>();
    }
}
