using System.Text.Json.Serialization;

namespace GoWheels_WebAPI.Models.GoogleRespone
{
    public class DistanceMatrixRespone
    {
        [JsonPropertyName("destination_addresses")]
        public List<string> DestinationAddresses { get; set; } = new List<string>();

        [JsonPropertyName("origin_addresses")]
        public List<string> OriginAddresses { get; set; } = new List<string>();

        [JsonPropertyName("rows")]
        public List<Row> Rows { get; set; } = new List<Row>();

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
