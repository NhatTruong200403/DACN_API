using System.ComponentModel.DataAnnotations;

namespace GoWheels_WebAPI.Models.Entities
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string CreateById { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public required DateTime CreateOn { get; set; }

        public string? ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

    }
}
