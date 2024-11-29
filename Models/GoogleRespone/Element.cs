using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.GoogleRespone
{
    public class Element
    {
        [JsonPropertyName("distance")]
        public Distance Distance { get; set; } = null!;

        [JsonPropertyName("duration")]
        public Duration Duration { get; set; } = null!;

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
