using System.Text.Json.Serialization;

namespace LojaDoManoelAPI.Models
{
    public class PackedBoxOutput
    {
        [JsonPropertyName("caixa_usada")]
        public string CaixaUsada { get; set; }

        [JsonPropertyName("produtos")]
        public List<ProductDimensions> Produtos { get; set; }

        public PackedBoxOutput()
        {
            Produtos = new List<ProductDimensions>();
        }
    }
}
