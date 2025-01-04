using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GoWheels_WebAPI.Models.Entities
{
    public class Post : BaseModel
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
        public List<PostImage> Images { get; set; } = new List<PostImage>();
        public string? Description { get; set; }
        public required int Seat { get; set; }
        public required decimal PricePerHour { get; set; }
        public required decimal PricePerDay { get; set; }
        public int RideNumber { get; set; }
        public int CarTypeId { get; set; }
        [ValidateNever]
        public CarType CarType { get; set; } = null!;
        public int CompanyId { get; set; }
        [ValidateNever]
        public Company Company { get; set; } = null!;
        public ICollection<Booking> Booking { get; set; } = new List<Booking>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
