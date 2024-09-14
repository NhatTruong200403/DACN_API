using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.Entities
{
    public class Company : BaseModel
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string IconImage { get; set; }
        public ICollection<CarTypeDetail> CarTypeDetail { get; set; } = new List<CarTypeDetail>();
    }
}
