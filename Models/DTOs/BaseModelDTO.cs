using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.DTOs
{
    public class BaseModelDTO
    {
        public int Id { get; set; }

        [Required]
        public string? CreateById { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime CreateOn { get; set; }

        public string? ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
