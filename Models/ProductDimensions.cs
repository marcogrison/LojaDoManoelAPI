namespace LojaDoManoelAPI.Models
{
    public class ProductDimensions
    {
        public double Altura { get; set; }
        public double Largura { get; set; }
        public double Comprimento { get; set; }

        public double Volume => Altura * Largura * Comprimento;

        public ProductDimensions() { }

        public ProductDimensions(double altura, double largura, double comprimento)
        {
            Altura = altura;
            Largura = largura;
            Comprimento = comprimento;
        }

        public IEnumerable<ProductDimensions> GetOrientations()
        {
            yield return new ProductDimensions(Altura, Largura, Comprimento);
            yield return new ProductDimensions(Altura, Comprimento, Largura);
            yield return new ProductDimensions(Largura, Altura, Comprimento);
            yield return new ProductDimensions(Largura, Comprimento, Altura);
            yield return new ProductDimensions(Comprimento, Altura, Largura);
            yield return new ProductDimensions(Comprimento, Largura, Altura);
        }

        public override string ToString()
        {
            return $"A: {Altura}, L: {Largura}, C: {Comprimento}";
        }
    }
}
