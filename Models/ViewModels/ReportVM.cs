using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GoWheels_WebAPI.Models.ViewModels
{
    public class ReportVM : BaseModelVM
    {
        public string? Content { get; set; }
        public required string UserId { get; set; }
        public UserVM User { get; set; } = null!;
    }
}
