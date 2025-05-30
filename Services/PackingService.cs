using LojaDoManoelAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace LojaDoManoel.API.Services
{
    public class PackingService
    {
        private readonly List<BoxDefinition> _availableBoxes;

        public PackingService()
        {

            _availableBoxes = new List<BoxDefinition>
            {
                new BoxDefinition("Caixa 1", 30, 40, 80),
                new BoxDefinition("Caixa 2", 80, 50, 40),
                new BoxDefinition("Caixa 3", 50, 80, 60)
            };

        }

        public OrderOutputPayload ProcessOrderPacking(OrderInput order)
        {
            var orderOutput = new OrderOutputPayload();

            var productsToPack = order.Produtos
                                      .Select(p => p.Dimensions) 
                                      .ToList();

            productsToPack = productsToPack.OrderByDescending(p => p.Volume).ToList();

            while (productsToPack.Any())
            {
                BoxDefinition bestBoxForCurrentIteration = null;
                List<ProductDimensions> productsForThisBox = new List<ProductDimensions>();
                double currentVolumeInThisBox = 0;

                var firstProductToAttempt = productsToPack.First();

                foreach (var boxDef in _availableBoxes.OrderBy(b => b.Volume))
                {
                    if (boxDef.CanProductFit(firstProductToAttempt))
                    {
                        bestBoxForCurrentIteration = boxDef;
                        break;
                    }
                }

                if (bestBoxForCurrentIteration == null)
                {
                    Console.WriteLine($"AVISO: Produto {firstProductToAttempt} (Volume: {firstProductToAttempt.Volume}) não cabe em nenhuma caixa disponível. Pedido: {order.IdPedido}");
                    productsToPack.Remove(firstProductToAttempt);
                    continue;
                }

                productsForThisBox.Add(firstProductToAttempt);
                currentVolumeInThisBox += firstProductToAttempt.Volume;

                var remainingProductsToTry = productsToPack.Skip(1).ToList();
                foreach (var otherProduct in remainingProductsToTry)
                {
                    if (bestBoxForCurrentIteration.CanProductFit(otherProduct) &&
                        (currentVolumeInThisBox + otherProduct.Volume) <= bestBoxForCurrentIteration.Volume)
                    {
                        productsForThisBox.Add(otherProduct);
                        currentVolumeInThisBox += otherProduct.Volume;
                    }
                }

                if (productsForThisBox.Any())
                {
                    var packedBox = new PackedBoxOutput
                    {
                        CaixaUsada = bestBoxForCurrentIteration.Nome,
                        Produtos = new List<ProductDimensions>(productsForThisBox)
                    };
                    orderOutput.Caixas.Add(packedBox);

                    foreach (var packedProduct in productsForThisBox)
                    {
                        productsToPack.Remove(packedProduct);
                    }
                }
                else if (bestBoxForCurrentIteration != null)
                {
                    productsToPack.Remove(firstProductToAttempt);
                }

            } 

            return orderOutput;
        }
    }
}