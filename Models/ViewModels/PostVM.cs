using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GoWheels_WebAPI.Models.ViewModels
{
    public class PostVM : BaseModelVM
    {
        public required string Name { get; set; }
        public required string Image { get; set; }
        public List<PostImageVM> Images { get; set; } = new List<PostImageVM>();
        public string? Description { get; set; }
        public required int Seat { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public int RideNumber { get; set; }
        public string? CarTypeName { get; set; }
        public string? CompanyName { get; set; }
        public UserVM User { get; set; } = null!;
    }
}
