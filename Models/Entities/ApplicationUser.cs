using Microsoft.AspNetCore.Identity;

namespace GoWheels_WebAPI.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? License { get; set; }
        public string? CIC { get; set; }
        public string? Image { get; set; }
        public DateTime? Birthday { get; set; }
        public int? ReportPoint { get; set; } = 0;
        public bool IsSubmitDriver { get; set; }
        public bool isDriver { get; set; } = false!;
        public Driver Driver { get; set; } = null!;
        public ICollection<Notify> Notifications { get; set; } = new List<Notify>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Booking> Booking { get; set; } = new List<Booking>();
        public ICollection<Rating> Rating { get; set; } = new List<Rating>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
