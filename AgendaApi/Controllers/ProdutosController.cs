using AgendaApi.Application.Services;
using AgendaApi.Extensions.DtoMapper;
using AgendaApi.Models;
using AgendaShared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _service;
        public ProdutosController(IProdutoService service) => _service = service;

        [HttpPost("{agendamentoId:int}/produtos")]
        public async Task<ActionResult<AgendamentoProdutoDto>> AdicionarProduto(int agendamentoId,
        [FromBody] AgendamentoProdutoCreateDto dto)
        {
            if (dto.ProdutoId <= 0 || dto.Quantidade <= 0)
            {
                return BadRequest("ProdutoId e Quantidade são obrigatórios.");
            }

            try
            {
                // 3. Chama o serviço (próximo passo)
                var produtoAdicionadoDto = await _service.AdicionarProdutoAsync(agendamentoId, dto);

                // Retorna 201 Created com o objeto criado
                return Ok(produtoAdicionadoDto);
            }
            catch (KeyNotFoundException ex) // Exemplo: se o produto não for encontrado
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AgendamentoProdutoDto>> GetById(int id)
        {
            var agprod = await _service.GetByIdAsync(id);
            return Ok(agprod); // se não existir, o middleware vai lançar NotFoundException
        }
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll()
        {
            var produtos = await _service.GetAllAsync();
            var dto = produtos.Select(p => p.ToDto());


            return Ok(dto);
        }

    }
}
