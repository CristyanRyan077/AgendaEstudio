using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Infra.Interfaces
{
    public interface IPagamentoRepository
    {
        Task<IEnumerable<Pagamento>> GetAllAsync();
        Task<Pagamento?> GetByIdAsync(int id);
        Task AddAsync(Pagamento pagamento);
        Task UpdateAsync(Pagamento pagamento);
        Task DeleteAsync(int id);
        Task<ResumoAgendamentoDto?> ObterResumoAgendamentoAsync(int agendamentoId);
        Task<List<HistoricoFinanceiroDto>> ListarHistoricoAsync(int agendamentoId);
    }
}
