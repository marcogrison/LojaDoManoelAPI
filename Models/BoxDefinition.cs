namespace LojaDoManoelAPI.Models
{
    public class BoxDefinition
    {
        public string Nome { get; }
        public ProductDimensions Dimensoes { get; }
        public double Volume => Dimensoes.Volume;

        public BoxDefinition(string nome, double altura, double largura, double comprimento)
        {
            Nome = nome;
            Dimensoes = new ProductDimensions(altura, largura, comprimento);
        }

        public bool CanProductFit(ProductDimensions productDimensions)
        {
            foreach (var orientation in productDimensions.GetOrientations())
            {
                if (orientation.Altura <= Dimensoes.Altura &&
                    orientation.Largura <= Dimensoes.Largura &&
                    orientation.Comprimento <= Dimensoes.Comprimento)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
