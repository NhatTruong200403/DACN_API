using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.Entities
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? CreatedById { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public required DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? ModifiedById { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;



    }
}
