using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.GoogleRespone
{
    public class Distance
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
