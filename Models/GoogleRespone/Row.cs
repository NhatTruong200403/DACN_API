using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.GoogleRespone
{
    public class Row
    {
        [JsonPropertyName("elements")]
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}
