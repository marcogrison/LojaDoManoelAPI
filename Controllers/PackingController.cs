using Microsoft.AspNetCore.Mvc;
using LojaDoManoel.API.Models;
using LojaDoManoel.API.Services;
using LojaDoManoel.API.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using LojaDoManoelAPI.Models;

namespace LojaDoManoel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackingController : ControllerBase
    {
        private readonly PackingService _packingService;
        private readonly ApiDbContext _context;

        // DbContext injetado no construtor
        public PackingController(PackingService packingService, ApiDbContext context)
        {
            _packingService = packingService;
            _context = context;
        }

        [HttpPost("process")]
        [ProducesResponseType(typeof(Dictionary<string, OrderOutputPayload>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessPackages([FromBody] List<OrderInput> pedidos)
        {
            // Inicializa o log do job
            var jobLog = new PackingJobLog
            {
                Id = Guid.NewGuid(),
                RequestTimestamp = DateTime.UtcNow,
            };

            try
            {
                jobLog.RequestPayload = JsonSerializer.Serialize(pedidos);
            }
            catch (Exception ex)
            {
                jobLog.RequestPayload = $"Erro ao serializar payload da requisição: {ex.Message}";
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição de entrada.");
            }


            if (pedidos == null || !pedidos.Any())
            {
                jobLog.Success = false;
                jobLog.ResponsePayload = "A lista de pedidos não pode ser vazia.";
                jobLog.ResponseTimestamp = DateTime.UtcNow;
                // Salva o log de erro antes de retornar
                try
                {
                    await _context.PackingJobLogs.AddAsync(jobLog);
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx) { Console.WriteLine($"Erro ao salvar log de erro no BD: {dbEx.Message}"); }
                return BadRequest(jobLog.ResponsePayload);
            }

            if (pedidos.Any(p => p.Produtos == null))
            {
                jobLog.Success = false;
                jobLog.ResponsePayload = "Um ou mais pedidos contêm uma lista de produtos nula.";
                jobLog.ResponseTimestamp = DateTime.UtcNow;
                try
                {
                    await _context.PackingJobLogs.AddAsync(jobLog);
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx) { Console.WriteLine($"Erro ao salvar log de erro no BD: {dbEx.Message}"); }
                return BadRequest(jobLog.ResponsePayload);
            }

            if (pedidos.Any(p => !p.Produtos.Any()))
            {
                jobLog.Success = false;
                jobLog.ResponsePayload = "Todos os pedidos devem conter ao menos um produto.";
                jobLog.ResponseTimestamp = DateTime.UtcNow;
                try
                {
                    await _context.PackingJobLogs.AddAsync(jobLog);
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx) { Console.WriteLine($"Erro ao salvar log de erro no BD: {dbEx.Message}"); }
                return BadRequest(jobLog.ResponsePayload);
            }


            var resultadoFinal = new Dictionary<string, OrderOutputPayload>();
            bool anyErrorInIndividualOrders = false;

            foreach (var pedidoInput in pedidos)
            {
                if (string.IsNullOrEmpty(pedidoInput.IdPedido))
                {
                    jobLog.ResponsePayload = $"Um dos pedidos na lista não possui um id_pedido. O processamento em lote foi interrompido.";
                    anyErrorInIndividualOrders = true;
                    break;
                }
                var orderOutput = _packingService.ProcessOrderPacking(pedidoInput);
                resultadoFinal[pedidoInput.IdPedido] = orderOutput;
            }

            if (anyErrorInIndividualOrders)
            {
                jobLog.Success = false;
                jobLog.ResponseTimestamp = DateTime.UtcNow;
                try
                {
                    await _context.PackingJobLogs.AddAsync(jobLog);
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx) { Console.WriteLine($"Erro ao salvar log de erro no BD: {dbEx.Message}"); }
                return BadRequest(jobLog.ResponsePayload);
            }

            jobLog.Success = true;
            try
            {
                jobLog.ResponsePayload = JsonSerializer.Serialize(resultadoFinal);
            }
            catch (Exception ex)
            {
                jobLog.Success = false;
                jobLog.ResponsePayload = $"Erro ao serializar payload da resposta: {ex.Message}";
                Console.WriteLine($"Erro ao serializar payload da resposta: {ex.Message}");
            }
            jobLog.ResponseTimestamp = DateTime.UtcNow;

            try
            {
                await _context.PackingJobLogs.AddAsync(jobLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar o log principal no banco de dados: {ex.Message}");

                if (!jobLog.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao finalizar o processamento e registrar o resultado.");
                }
            }
            if (!jobLog.Success && jobLog.ResponsePayload.StartsWith("Erro ao serializar payload da resposta"))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao gerar a resposta final.");
            }

            return Ok(resultadoFinal);
        }
    }
}