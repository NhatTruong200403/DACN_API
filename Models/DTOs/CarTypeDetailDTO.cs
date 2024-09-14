using GoWheels_WebAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class CarTypeDetailDTO
    {
        [Required]
        public int CarTypeId { get; set; }
        public string? CarTypeName { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
