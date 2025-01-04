namespace GoWheels_WebAPI.Models.ViewModels
{
    public class UserVM
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? License { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? CIC { get; set; }
        public string? Image { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
        public int? ReportPoint {  get; set; }
        public DateTime? Birthday { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
