using System.Text.Json.Serialization;

namespace LojaDoManoelAPI.Models
{
    public class OrderInput
    {
        [JsonPropertyName("id_pedido")]
        public string IdPedido { get; set; }

        [JsonPropertyName("produtos")]
        public List<ProductInput> Produtos { get; set; }
    }
}
