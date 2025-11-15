using AgendaApi.Application.Services;
using AgendaShared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AgendaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota será "api/Financeiro"
    public class FinanceiroController : ControllerBase
    {
        private readonly IFinanceiroService _service;

        public FinanceiroController(IFinanceiroService service)
        {
            _service = service;
        }

        // 1. Calcular KPIs (POST é recomendado para filtros complexos, mas GET é comum)
        [HttpGet("kpis")]
        public async Task<ActionResult<FinanceiroResumo>> GetKpis([FromQuery] FinanceiroFiltroRequest filtro)
        {
            // Validação básica do filtro aqui (ex: datas)
            if (filtro.Inicio > filtro.Fim)
            {
                return BadRequest("Data de início não pode ser maior que a data de fim.");
            }
            var resultado = await _service.CalcularKpisAsync(filtro);
            return Ok(resultado);
        }

        // 2. Listar Em Aberto
        [HttpGet("recebiveis")]
        public async Task<ActionResult<List<RecebivelDTO>>> ListarEmAberto([FromQuery] FinanceiroFiltroRequest filtro)
        {
            var lista = await _service.ListarEmAbertoAsync(filtro);
            return Ok(lista);
        }

        // 3. Resumo por Produto
        [HttpGet("resumo/produtos")]
        public async Task<ActionResult<List<ProdutoResumoVM>>> GetResumoPorProduto([FromQuery] FinanceiroFiltroRequest filtro)
        {
            var lista = await _service.ResumoPorProdutoAsync(filtro);
            return Ok(lista);
        }

        // 4. Resumo por Serviço
        [HttpGet("resumo/servicos")]
        public async Task<ActionResult<List<ServicoResumoDTO>>> GetResumoPorServico([FromQuery] FinanceiroFiltroRequest filtro)
        {
            var lista = await _service.ResumoPorServicoAsync(filtro);
            return Ok(lista);
        }
    }
}
