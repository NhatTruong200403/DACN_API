using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWheels_WebAPI.Models.Entities
{
    public class CarTypeDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(CarType))]
        public int CarTypeId { get; set; }
        [ValidateNever]
        public CarType CarType { get; set; } = null!;


        [Required]
        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }
        [ValidateNever]
        public Company Company { get; set; } = null!;
        
        [Required]
        public bool IsDeleted { get; set; }
    }
}
