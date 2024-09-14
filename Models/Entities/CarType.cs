using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.Entities
{
    public class CarType : BaseModel
    {
        [Required]
        public required string Name { get; set; }
        public ICollection<CarTypeDetail> CarTypeDetail { get; set; } = new List<CarTypeDetail>();
    }
}
