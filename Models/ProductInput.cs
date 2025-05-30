using System.Text.Json.Serialization;

namespace LojaDoManoelAPI.Models
{
    public class ProductInput
    {
        [JsonPropertyName("altura")]
        public double Altura { get; set; }

        [JsonPropertyName("largura")]
        public double Largura { get; set; }

        [JsonPropertyName("comprimento")]
        public double Comprimento { get; set; }

        [JsonIgnore]
        public ProductDimensions Dimensions => new ProductDimensions(Altura, Largura, Comprimento);
    }
}
