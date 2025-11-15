using AgendaApi.Domain.Models;
using AgendaApi.Models;
using AgendaShared.DTOs;

namespace AgendaApi.Infra.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
        Task<List<Agendamento>> GetAgendamentos(int clienteId);
        Task<PagedResult<ClienteDto>> GetPaginadoAsync(int page, int pageSize, string? nome, int mesRef, int anoRef);
    }
}
