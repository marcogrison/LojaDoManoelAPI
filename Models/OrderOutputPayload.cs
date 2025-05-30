using System.Text.Json.Serialization;

namespace LojaDoManoelAPI.Models
{
    public class OrderOutputPayload
    {
        [JsonPropertyName("caixas")]
        public List<PackedBoxOutput> Caixas { get; set; }

        public OrderOutputPayload()
        {
            Caixas = new List<PackedBoxOutput>();
        }
    }
}