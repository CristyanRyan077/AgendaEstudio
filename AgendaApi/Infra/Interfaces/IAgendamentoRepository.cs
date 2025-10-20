using AgendaApi.Models;

namespace AgendaApi.Interfaces
{
    public interface IAgendamentoRepository
    {
        Task<IEnumerable<Agendamento>> GetAllAsync();
        Task<Agendamento?> GetByIdAsync(int id);
        Task AddAsync(Agendamento agendamento);
        Task UpdateAsync(Agendamento agendamento);
        Task DeleteAsync(int id);
    }
}
