using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GoWheels_WebAPI.Models.Entities
{
    public class Report : BaseModel
    {
        public string? Content { get; set; }
        public required string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
