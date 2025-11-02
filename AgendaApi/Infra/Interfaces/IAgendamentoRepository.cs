using AgendaApi.Domain;
using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Interfaces
{
    public interface IAgendamentoRepository
    {
        Task<IEnumerable<Agendamento>> GetAllAsync();
        Task<Agendamento?> GetByIdAsync(int id);
        Task AddAsync(Agendamento agendamento);
        Task UpdateAsync(Agendamento agendamento);
        Task DeleteAsync(int id);
        Task<IEnumerable<Agendamento>> GetPorPeriodoAsync(DateTime inicio, DateTime fim);
    }
}
