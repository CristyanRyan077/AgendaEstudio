using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Infra.Interfaces
{
    public interface IPagamentoService
    {
        Task<ResumoAgendamentoDto> ObterResumoAgendamentoAsync(int agendamentoId);
        Task<PagamentoDto> AdicionarPagamentoAsync(int agendamentoId, PagamentoCreateDto dto);
        Task AtualizarPagamentoAsync(PagamentoUpdateDto dto);
        Task RemoverPagamentoAsync(int pagamentoID);
        Task<List<HistoricoFinanceiroDto>> ListarHistoricoAsync(int agendamentoId);
        Task<Pagamento> GetPagamentoOrThrowAsync(int id);

        Task<PagamentoDto?> GetByIdAsync(int id);
    }
}
